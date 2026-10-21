using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Windows;

namespace NativeWinUISmoke;

/// <summary>Automatic regressions requiring a real, interactive WinUI desktop.</summary>
public sealed class SmokeChecks
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private readonly IContainerProvider _root;
    private readonly IWindowManager _windows;
    private readonly SmokeWindow _primary;
    private readonly SmokeState _state;
    private readonly List<SmokeDialogRequest> _requests = new();

    public SmokeChecks(IContainerProvider root, IWindowManager windows,
        IWindowContext primary, SmokeState state)
    {
        _root = root;
        _windows = windows;
        _primary = new SmokeWindow(primary);
        _state = state;
    }

    public static void CheckInitialized(SmokeState state, IWindowContext context, SmokeShell shell)
    {
        state.Check(shell.IsLoaded, "Native shell is loaded before its initialization callback.");
        state.Check(ReferenceEquals(context.Window.Content, shell), "Window context owns its native shell.");
        state.Check(context.RegionManager.Regions.ContainsRegionWithName(SmokeShell.RegionName),
            "ContentControl region registered before the window initialization callback.");
        state.Check(shell.FinishLoadingCalls == 0, "FinishLoading has not run before the initialization callback.");
        shell.OnInitializedCompleted = true;
    }

    public async Task RunAsync()
    {
        try
        {
            CheckShell(_primary);
            _state.Check(ReferenceEquals(_windows, _root.Resolve<IWindowManager>()),
                "Window manager is a shared application service.");
            _state.Check(Throws(() => _root.Resolve<IDialogService>()),
                "Application-root dialog resolution fails rather than silently selecting a window.");
            await CheckUiThreadEventAsync();
            await NavigateAsync<HomeView>(_primary);
            await CheckHomeViewAsync(_primary);
            CheckForeignShellRejected();
            await CheckUiThreadBoundaryAsync();

            var secondary = await OpenWindowAsync("secondary");
            CheckWindowIsolation(secondary);
            await NavigateAsync<HomeView>(secondary);
            await CheckHomeViewAsync(secondary);
            CheckLateDiscovery(secondary);
            await CheckIndependentNavigationAsync(secondary);
            await CheckOriginAndExplicitDialogsAsync(secondary);
            await CheckQueuedDialogsAsync();
            await CheckUnloadedTargetAsync();
            await CheckCloseCancelsDialogsAsync(secondary);
            await CheckSurvivingWindowAsync(secondary);
            _state.Check(_requests.All(request => request.CallbackCalls == 1),
                "All dialog callbacks remain single-completion after later queue and window activity.");
        }
        catch (Exception exception)
        {
            _state.Check(false, exception.ToString());
        }
        finally
        {
            // A failed assertion or timeout must not leave additional smoke windows alive.
            foreach (var window in _windows.Windows.Where(window => !ReferenceEquals(window, _primary.Context)).Reverse())
            {
                try { window.Window.Close(); }
                catch (Exception exception) { _state.Check(false, $"Smoke cleanup failed: {exception}"); }
            }

            _state.Write(_state.Failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({_state.Failures} checks)");
            Environment.ExitCode = _state.Failures == 0 ? 0 : 1;
            _primary.Shell.EnableManualChecks();
            if (Environment.GetEnvironmentVariable("PRISM_WINUI_SMOKE_AUTOCLOSE") == "1")
                _primary.Context.Window.Close();
        }
    }

    private async Task<SmokeWindow> OpenWindowAsync(string name)
    {
        var initialized = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var activated = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var nativeWindow = new Window { Title = $"Prism native WinUI smoke: {name}" };
        void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState != WindowActivationState.Deactivated)
                activated.TrySetResult(true);
        }
        nativeWindow.Activated += OnActivated;
        try
        {
            var context = _windows.OpenWindow(nativeWindow, container => container.Resolve<SmokeShell>(), window =>
            {
                CheckInitialized(_state, window, (SmokeShell)window.Window.Content);
                initialized.TrySetResult(true);
            });
            var result = new SmokeWindow(context);
            await Task.WhenAll(initialized.Task, activated.Task).WaitAsync(Timeout);
            await Task.Yield();
            CheckShell(result);
            CheckModulesOnce();
            return result;
        }
        finally
        {
            nativeWindow.Activated -= OnActivated;
        }
    }

    private void CheckShell(SmokeWindow window)
    {
        var container = window.Context.Container;
        _state.Check(window.Shell.FinishLoadingCalls == 1,
            "Each shell completes ILoadableShell.FinishLoading exactly once.");
        _state.Check(ReferenceEquals(window.Shell.Lifetime, container.Resolve<WindowLifetime>()) &&
            ReferenceEquals(window.Shell.Container, container) && !ReferenceEquals(container, _root),
            "Shell constructor and injected provider use the window scope, not the application root.");
        _state.Check(ReferenceEquals(container.Resolve<IWindowContext>(), window.Context) &&
            ReferenceEquals(_windows.GetWindow(window.Context.Window), window.Context),
            "Scoped context resolution and native-window lookup return the same context.");
    }

    private void CheckForeignShellRejected()
    {
        var originalRegion = _primary.Regions.Regions[SmokeShell.RegionName];
        var rejected = Throws(() => _windows.OpenWindow(
            new Window { Title = "Prism smoke: rejected foreign shell" }, _ => _primary.Shell));
        _state.Check(rejected && _windows.Windows.Count == 1 && !_primary.Context.IsClosed &&
            _primary.Shell.IsLoaded && _primary.Shell.Lifetime.DisposeCalls == 0 &&
            ReferenceEquals(_primary.Context.Window.Content, _primary.Shell) &&
            ReferenceEquals(_primary.Regions.Regions[SmokeShell.RegionName], originalRegion),
            "Rejecting a shell already owned by another window does not clean up or dispose its original scope.");
    }

    private async Task CheckUiThreadBoundaryAsync()
    {
        var rejected = await Task.Run(() =>
            Throws(() => _windows.Windows) && Throws(() => _primary.Context.Container)).WaitAsync(Timeout);
        _state.Check(rejected, "Window manager and scoped context access reject a non-owning thread.");
    }

    private void CheckWindowIsolation(SmokeWindow secondary)
    {
        _state.Check(_windows.Windows.Count == 2 && _windows.Windows.Contains(_primary.Context) &&
            _windows.Windows.Contains(secondary.Context), "Window manager tracks both live windows.");
        _state.Check(!ReferenceEquals(_primary.Context.Container, secondary.Context.Container) &&
            !ReferenceEquals(_primary.Regions, secondary.Regions) &&
            !ReferenceEquals(_primary.Shell.Lifetime, secondary.Shell.Lifetime),
            "Two windows have separate providers, region managers, and scoped dependencies.");
        _state.Check(ReferenceEquals(_primary.Shell.Lifetime.Root, secondary.Shell.Lifetime.Root) &&
            ReferenceEquals(_root.Resolve<RootLifetime>(), secondary.Shell.Lifetime.Root),
            "Window scopes share the root singleton without replacing it.");
        _state.Check(!ReferenceEquals(_primary.Regions.Regions[SmokeShell.RegionName],
            secondary.Regions.Regions[SmokeShell.RegionName]),
            "Identically named shell regions are independent objects.");
    }

    private async Task NavigateAsync<TView>(SmokeWindow window)
    {
        var completion = new TaskCompletionSource<NavigationResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        window.Regions.RequestNavigate(SmokeShell.RegionName, typeof(TView).Name,
            result => completion.TrySetResult(result));
        var result = await completion.Task.WaitAsync(Timeout);
        _state.Check(result.Success && window.Shell.RegionHost.Content is TView,
            $"Scoped region navigation to {typeof(TView).Name}: {result.Exception?.Message ?? "view activated"}.");
    }

    private async Task CheckHomeViewAsync(SmokeWindow window)
    {
        if (window.Shell.RegionHost.Content is not HomeView home)
            throw new InvalidOperationException("Home view is unavailable for XAML and scope checks.");

        _state.Check(ViewModelLocator.GetAutowireViewModel(home) == true &&
            home.DataContext is HomeViewModel viewModel &&
            ReferenceEquals(viewModel.Lifetime, window.Shell.Lifetime) &&
            ReferenceEquals(viewModel.Container, window.Context.Container),
            "XAML ViewModelLocator autowiring resolves the registered VM from the owning window scope.");
        _state.Check(ReferenceEquals(home.Lifetime, window.Shell.Lifetime) &&
            ReferenceEquals(home.Container, window.Context.Container),
            "Navigation view and its injected provider retain their owning scope.");
        _state.Check(RegionManager.GetRegionName(home.NestedRegionHost) == SmokeShell.NestedRegionName,
            "XAML RegionManager.RegionName metadata applies to the named nested control.");
        await WaitUntilAsync(() => home.NestedRegionHost.IsLoaded &&
            window.Regions.Regions.ContainsRegionWithName(SmokeShell.NestedRegionName));
        var defaults = window.Regions.Regions[SmokeShell.NestedRegionName].Views.OfType<DefaultView>().ToArray();
        _state.Check(defaults.Length >= 1, "Nested region discovers its default registered view.");
        foreach (var view in defaults)
            CheckDiscoveredView(view, window);
    }

    private void CheckDiscoveredView(DefaultView view, SmokeWindow window)
    {
        _state.Check(ReferenceEquals(view.Lifetime, window.Shell.Lifetime) &&
            ReferenceEquals(view.Container, window.Context.Container) &&
            view.DataContext is DefaultViewModel viewModel &&
            ReferenceEquals(viewModel.Lifetime, window.Shell.Lifetime) &&
            ReferenceEquals(viewModel.Container, window.Context.Container),
            "Discovered view and its VM resolve through the region owner's provider, not the shared registry's root.");
    }

    private void CheckLateDiscovery(SmokeWindow secondary)
    {
        var primaryRegion = _primary.Regions.Regions[SmokeShell.NestedRegionName];
        var secondaryRegion = secondary.Regions.Regions[SmokeShell.NestedRegionName];
        _state.Check(!ReferenceEquals(primaryRegion, secondaryRegion),
            "Identically named nested XAML regions remain isolated across windows.");
        var previousPrimary = primaryRegion.Views.Cast<object>().ToArray();
        var previousSecondary = secondaryRegion.Views.Cast<object>().ToArray();
        _root.Resolve<IRegionViewRegistry>().RegisterViewWithRegion(SmokeShell.NestedRegionName, "LateDefault");
        var addedPrimary = primaryRegion.Views.OfType<DefaultView>().Where(view => !previousPrimary.Contains(view)).ToArray();
        var addedSecondary = secondaryRegion.Views.OfType<DefaultView>().Where(view => !previousSecondary.Contains(view)).ToArray();
        _state.Check(addedPrimary.Length == 1 && addedSecondary.Length == 1,
            "A late named discovery registration populates both existing regions once.");
        foreach (var view in addedPrimary)
            CheckDiscoveredView(view, _primary);
        foreach (var view in addedSecondary)
            CheckDiscoveredView(view, secondary);
    }

    private async Task CheckIndependentNavigationAsync(SmokeWindow secondary)
    {
        var originalPrimary = _primary.Shell.RegionHost.Content;
        await NavigateAsync<ModuleView>(secondary);
        _state.Check(ReferenceEquals(originalPrimary, _primary.Shell.RegionHost.Content) &&
            secondary.Shell.RegionHost.Content is ModuleView module &&
            ReferenceEquals(module.Lifetime, secondary.Shell.Lifetime) &&
            ReferenceEquals(module.Container, secondary.Context.Container),
            "Module navigation uses the target scope and leaves the other window's active view unchanged.");
        await NavigateAsync<HomeView>(secondary);
        await CheckHomeViewAsync(secondary);
    }

    private async Task CheckUiThreadEventAsync()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        // First application-level resolution is deliberately off the UI thread.
        var smokeEvent = await Task.Run(() => _root.Resolve<IEventAggregator>().GetEvent<SmokeEvent>()).WaitAsync(Timeout);
        var completion = new TaskCompletionSource<(bool HasThreadAccess, int PublisherThread)>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var subscription = smokeEvent.Subscribe(publisherThread =>
            completion.TrySetResult((_primary.Shell.DispatcherQueue.HasThreadAccess, publisherThread)),
            ThreadOption.UIThread, keepSubscriberReferenceAlive: true);
        try
        {
            await Task.Run(() => smokeEvent.Publish(Environment.CurrentManagedThreadId)).WaitAsync(Timeout);
            var result = await completion.Task.WaitAsync(Timeout);
            _state.Check(result.HasThreadAccess && result.PublisherThread != uiThreadId,
                "Background event publication reaches the captured UI dispatcher.");
        }
        finally
        {
            smokeEvent.Unsubscribe(subscription);
        }
    }

    private async Task CheckOriginAndExplicitDialogsAsync(SmokeWindow secondary)
    {
        // The primary shell received this service before the secondary window existed.
        secondary.Context.Window.Activate();
        await Task.Yield();
        var origin = ShowDialog(_primary.Shell.DialogService, "origin-after-activate", true);
        await CheckSuccessfulDialogAsync(origin, "origin-after-activate", _primary);
        _state.Check(!ReferenceEquals(_state.GetDialog("origin-after-activate").HostRoot, secondary.Shell.XamlRoot),
            "Activating another window cannot retarget an already injected default dialog service.");

        var explicitTarget = ShowDialog(secondary.Context.Container.Resolve<IDialogService>(),
            "explicit-secondary", true);
        await CheckSuccessfulDialogAsync(explicitTarget, "explicit-secondary", secondary);
    }

    private async Task CheckSuccessfulDialogAsync(SmokeDialogRequest request, string id, SmokeWindow expected)
    {
        var result = await request.Completion.WaitAsync(Timeout);
        var observation = _state.GetDialog(id);
        var expectedRoot = expected.Context.Window.Content.XamlRoot;
        _state.Check(result.Exception is null && result.Result == ButtonResult.OK && request.CallbackCalls == 1 &&
            observation.OpenedCount == 1 && observation.ClosedCount == 1,
            $"Dialog {id} runs its lifecycle and successful callback exactly once.");
        _state.Check(observation.LoadedCount > 0 && observation.Host is DialogWindow &&
            expectedRoot is not null &&
            ReferenceEquals(observation.HostRoot, expectedRoot) &&
            ReferenceEquals(observation.ViewRoot, expectedRoot),
            $"Dialog {id} is observed inside the actual default ContentDialog with its owner's XamlRoot.");
        _state.Check(ReferenceEquals(observation.ViewLifetime, expected.Shell.Lifetime) &&
            ReferenceEquals(observation.ViewModel?.Lifetime, expected.Shell.Lifetime) &&
            ReferenceEquals(observation.ViewModel?.Container, expected.Context.Container),
            $"Dialog {id} view and view model use the target window's scoped dependencies.");
    }

    private async Task CheckQueuedDialogsAsync()
    {
        var first = ShowDialog(_primary.Shell.DialogService, "queue-first", true);
        var second = ShowDialog(_primary.Shell.DialogService, "queue-second", true);
        await CheckSuccessfulDialogAsync(first, "queue-first", _primary);
        await CheckSuccessfulDialogAsync(second, "queue-second", _primary);
        _state.Check(_state.GetDialog("queue-second").OpenSequence > _state.GetDialog("queue-first").CloseSequence,
            "Concurrent dialog calls in one window serialize instead of colliding in ContentDialog.ShowAsync.");
    }

    private async Task CheckUnloadedTargetAsync()
    {
        var target = await OpenWindowAsync("unloaded target");
        var unloaded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        void OnUnloaded(object sender, RoutedEventArgs args) => unloaded.TrySetResult(true);
        target.Shell.Unloaded += OnUnloaded;
        try
        {
            target.Context.Window.Content = null;
            await unloaded.Task.WaitAsync(Timeout);
        }
        finally
        {
            target.Shell.Unloaded -= OnUnloaded;
        }
        _state.Check(!target.Context.IsClosed && target.Shell.Lifetime.DisposeCalls == 0,
            "Unloading a shell does not dispose its still-open window scope.");
        var request = ShowDialog(target.Shell.DialogService, "unloaded-target", true);
        await CheckFailedDialogAsync(request, "unloaded-target");
        _state.Check(_state.GetDialog("unloaded-target").OpenedCount == 0,
            "A window without loaded content is rejected before opening a dialog.");
        var replacement = new Grid();
        target.Context.Window.Content = replacement;
        await WaitUntilAsync(() => replacement.IsLoaded && replacement.XamlRoot is not null);
        var liveContent = ShowDialog(target.Shell.DialogService, "replacement-content", true);
        await CheckSuccessfulDialogAsync(liveContent, "replacement-content", target);
        _state.Check(target.Shell.Lifetime.DisposeCalls == 0,
            "The original service uses replacement live window content without replacing or disposing its scope.");
        await CloseWindowAsync(target);
    }

    private async Task CheckCloseCancelsDialogsAsync(SmokeWindow secondary)
    {
        var primaryRegion = _primary.Regions.Regions[SmokeShell.RegionName];
        var primaryNestedRegion = _primary.Regions.Regions[SmokeShell.NestedRegionName];
        // A malformed attachment must never make secondary cleanup traverse another
        // window's region tree. This inert child deliberately has no RegionName.
        var foreignManagerChild = new Grid();
        RegionManager.SetRegionManager(foreignManagerChild, _primary.Regions);
        secondary.Shell.Children.Add(foreignManagerChild);
        await WaitUntilAsync(() => foreignManagerChild.IsLoaded);

        var active = ShowDialog(secondary.Shell.DialogService, "close-active", false);
        await _state.GetDialog("close-active").Loaded.Task.WaitAsync(Timeout);
        var independent = ShowDialog(_primary.Shell.DialogService, "independent-primary", true);
        await CheckSuccessfulDialogAsync(independent, "independent-primary", _primary);
        _state.Check(!active.Completion.IsCompleted,
            "A dialog in another window can complete while the secondary window's dialog remains open.");
        var queued = ShowDialog(secondary.Shell.DialogService, "close-queued", true);
        await CloseWindowAsync(secondary);
        _state.Check(_primary.Regions.Regions.ContainsRegionWithName(SmokeShell.RegionName) &&
            _primary.Regions.Regions.ContainsRegionWithName(SmokeShell.NestedRegionName) &&
            ReferenceEquals(_primary.Regions.Regions[SmokeShell.RegionName], primaryRegion) &&
            ReferenceEquals(_primary.Regions.Regions[SmokeShell.NestedRegionName], primaryNestedRegion),
            "Secondary cleanup ignores a foreign manager attached beneath its shell and preserves primary regions.");
        await CheckFailedDialogAsync(active, "close-active");
        await CheckFailedDialogAsync(queued, "close-queued");
        _state.Check(_state.GetDialog("close-active").OpenedCount == 1 &&
            _state.GetDialog("close-active").ClosedCount == 1 &&
            _state.GetDialog("close-queued").OpenedCount == 0,
            "Window closure cleans the active dialog once and cancels queued dialogs before opening them.");
        var closed = ShowDialog(secondary.Shell.DialogService, "closed-target", true);
        await CheckFailedDialogAsync(closed, "closed-target");
        _state.Check(_state.GetDialog("closed-target").OpenedCount == 0,
            "A retained dialog service cannot display after its origin window closes.");
    }

    private async Task CheckFailedDialogAsync(SmokeDialogRequest request, string id)
    {
        var result = await request.Completion.WaitAsync(Timeout);
        _state.Check(result.Exception is not null && request.CallbackCalls == 1,
            $"Dialog {id} reports the unavailable target through its callback exactly once.");
    }

    private async Task CloseWindowAsync(SmokeWindow window)
    {
        window.Context.Window.Close();
        await WaitUntilAsync(() => window.Context.IsClosed);
        _state.Check(window.Shell.Lifetime.DisposeCalls == 1 && window.Shell.Lifetime.Root.DisposeCalls == 0,
            "Window.Closed disposes its scoped dependency exactly once without disposing the root singleton.");
        _state.Check(!_windows.Windows.Contains(window.Context) &&
            Throws(() => _windows.GetWindow(window.Context.Window)) &&
            Throws(() => window.Context.Container) && Throws(() => window.Context.RegionManager),
            "Closed windows leave the manager and their context rejects scoped service access.");
        CheckModulesOnce();
    }

    private async Task CheckSurvivingWindowAsync(SmokeWindow closed)
    {
        var factoryProviders = new List<IContainerProvider>();
        _root.Resolve<IRegionViewRegistry>().RegisterViewWithRegion(SmokeShell.NestedRegionName, provider =>
        {
            factoryProviders.Add(provider);
            return provider.Resolve<DefaultView>();
        });
        _state.Check(factoryProviders.Count == 1 && ReferenceEquals(factoryProviders[0], _primary.Context.Container),
            "Late discovery after closure visits only the surviving window's region and provider.");
        await CheckHomeViewAsync(_primary);
        await NavigateAsync<ModuleView>(_primary);
        _state.Check(_primary.Shell.RegionHost.Content is ModuleView module &&
            ReferenceEquals(module.Lifetime, _primary.Shell.Lifetime),
            "Surviving-window navigation still uses its original live scope after another window closes.");
        await NavigateAsync<HomeView>(_primary);
        await CheckHomeViewAsync(_primary);
        _state.Check(_windows.Windows.Count == 1 && !_primary.Context.IsClosed &&
            _primary.Shell.Lifetime.DisposeCalls == 0 && closed.Shell.Lifetime.DisposeCalls == 1 &&
            _primary.Shell.Lifetime.Root.DisposeCalls == 0,
            "Additional activity neither re-disposes the closed scope nor disposes surviving/root dependencies.");
        CheckModulesOnce();
    }

    private SmokeDialogRequest ShowDialog(IDialogService service, string id, bool autoClose)
    {
        var request = new SmokeDialogRequest(service, id, autoClose);
        _requests.Add(request);
        return request;
    }

    private void CheckModulesOnce() => _state.Check(
        _state.ModuleRegisterTypesCount == 1 && _state.ModuleInitializedCount == 1,
        "Creating, using, and closing secondary windows does not repeat module registration or initialization.");

    private static bool Throws(Func<object> action)
    {
        try { _ = action(); return false; }
        catch (Exception) { return true; }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var watch = Stopwatch.StartNew();
        while (!condition())
        {
            if (watch.Elapsed >= Timeout)
                throw new TimeoutException("Timed out waiting for the native WinUI lifecycle condition.");
            await Task.Delay(25);
        }
    }
}
