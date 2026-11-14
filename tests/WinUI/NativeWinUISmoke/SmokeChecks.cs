using System.Diagnostics;
using Microsoft.UI.Xaml;
using Prism.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

/// <summary>Checks the application lifecycle on a real WinUI desktop.</summary>
public sealed class SmokeChecks(IContainerProvider container, Window window, SmokeState state)
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private SmokeShell Shell => (SmokeShell)window.Content;
    private IRegionManager Regions => container.Resolve<IRegionManager>();

    public static void CheckInitialized(SmokeState state, IRegionManager regions, SmokeShell shell)
    {
        state.Check(shell.IsLoaded, "Shell is loaded before OnInitialized.");
        state.Check(regions.Regions.ContainsRegionWithName(SmokeShell.RegionName),
            "Shell region registered before OnInitialized.");
        state.Check(shell.FinishLoadingCalls == 0, "FinishLoading follows OnInitialized.");
        shell.OnInitializedCompleted = true;
    }

    public async Task RunAsync()
    {
        try
        {
            state.Check(Shell.FinishLoadingCalls == 1, "FinishLoading runs exactly once.");
            state.Check(ReferenceEquals(Shell.Container, container), "Shell uses the application container.");
            state.Check(ReferenceEquals(Shell.DialogService, container.Resolve<IDialogService>()),
                "Dialog service is available from the application container.");
            await CheckUiThreadEventAsync();
            await NavigateAsync<HomeView>();
            await CheckHomeViewAsync();
            await NavigateAsync<ModuleView>();
            state.Check(Shell.RegionHost.Content is ModuleView module && ReferenceEquals(module.Container, container),
                "Module view resolves through the application container.");
            await NavigateAsync<HomeView>();
            await CheckHomeViewAsync();
            await new SmokeNavigationChecks(container, window, state).RunAsync();
            await SmokeAdapterChecks.RunAsync(container, window, state);
            await CheckDialogAsync("first");
            await CheckDialogAsync("second");
            await CheckDialogVetoAsync();
            await CheckMissingDialogAsync();
            state.Check(state.ModuleRegisterTypesCount == 1 && state.ModuleInitializedCount == 1,
                "Navigation and dialogs do not repeat module initialization.");
        }
        catch (Exception exception)
        {
            state.Check(false, exception.ToString());
        }
        finally
        {
            state.Write(state.Failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({state.Failures} checks)");
            Environment.ExitCode = state.Failures == 0 ? 0 : 1;
            Shell.EnableManualChecks();
            if (Environment.GetEnvironmentVariable("PRISM_WINUI_SMOKE_AUTOCLOSE") == "1")
                window.Close();
        }
    }

    private async Task NavigateAsync<TView>()
    {
        var completion = new TaskCompletionSource<NavigationResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        Regions.RequestNavigate(SmokeShell.RegionName, typeof(TView).Name, result => completion.TrySetResult(result));
        var result = await completion.Task.WaitAsync(Timeout);
        state.Check(result.Success && Shell.RegionHost.Content is TView,
            $"Navigation to {typeof(TView).Name}: {result.Exception?.Message ?? "view activated"}.");
    }

    private async Task CheckHomeViewAsync()
    {
        if (Shell.RegionHost.Content is not HomeView home)
            throw new InvalidOperationException("Home view is unavailable.");
        state.Check(ViewModelLocator.GetAutowireViewModel(home) == true &&
            home.DataContext is HomeViewModel viewModel && ReferenceEquals(viewModel.Container, container),
            "XAML ViewModelLocator resolves the registered view model from the application container.");
        state.Check(ReferenceEquals(home.Container, container), "Navigation view uses the application container.");
        state.Check(RegionManager.GetRegionName(home.NestedRegionHost) == SmokeShell.NestedRegionName,
            "XAML region metadata applies to the nested control.");
        await WaitUntilAsync(() => home.NestedRegionHost.IsLoaded &&
            Regions.Regions.ContainsRegionWithName(SmokeShell.NestedRegionName));
        var defaults = Regions.Regions[SmokeShell.NestedRegionName].Views.OfType<DefaultView>().ToArray();
        state.Check(defaults.Length >= 1, "Nested region discovers its registered default view.");
        foreach (var view in defaults)
            state.Check(ReferenceEquals(view.Container, container) && view.DataContext is DefaultViewModel vm &&
                ReferenceEquals(vm.Container, container), "Discovered view and view model use the application provider.");
    }

    private async Task CheckDialogAsync(string id)
    {
        var request = new SmokeDialogRequest(Shell.DialogService, id, true);
        var result = await request.Completion.WaitAsync(Timeout);
        var observation = state.GetDialog(id);
        state.Check(result.Exception is null && result.Result == ButtonResult.OK && request.CallbackCalls == 1 &&
            observation.OpenedCount == 1 && observation.ClosedCount == 1,
            $"Dialog {id} opens, closes, and completes its callback once.");
        state.Check(observation.Host is DialogWindow && observation.LoadedCount > 0 &&
            ReferenceEquals(observation.HostRoot, Shell.XamlRoot) && ReferenceEquals(observation.ViewRoot, Shell.XamlRoot),
            $"Dialog {id} uses the native application's XamlRoot.");
        state.Check(ReferenceEquals(observation.ViewModel?.Container, container),
            $"Dialog {id} resolves its view model from the application container.");
    }

    private async Task CheckDialogVetoAsync()
    {
        const string id = "cancel-veto";
        var request = new SmokeDialogRequest(Shell.DialogService, id, false);
        var observation = state.GetDialog(id);
        await observation.Loaded.Task.WaitAsync(Timeout);
        await observation.Presented.Task.WaitAsync(Timeout);
        var viewModel = observation.ViewModel!;
        viewModel.AllowClose = false;
        viewModel.RequestClose.Invoke(ButtonResult.Cancel);
        await WaitUntilAsync(() => viewModel.CloseAttempts > 0);
        state.Check(!request.Completion.IsCompleted && observation.ClosedCount == 0,
            "CanCloseDialog=false keeps the native dialog open without completing its callback.");
        viewModel.AllowClose = true;
        viewModel.RequestClose.Invoke(ButtonResult.Cancel);
        var result = await request.Completion.WaitAsync(Timeout);
        state.Check(result.Exception is null && result.Result == ButtonResult.Cancel &&
            request.CallbackCalls == 1 && observation.ClosedCount == 1 && viewModel.CloseAttempts == 2,
            "Allowing a later close returns Cancel and completes the dialog lifecycle exactly once.");
        await WaitUntilAsync(() => observation.Host!.Content is null && observation.Host.DataContext is null);
        state.Check(observation.Host!.Content is null && observation.Host.DataContext is null,
            "Closed dialog host releases its content and view model.");
    }

    private async Task CheckMissingDialogAsync()
    {
        var request = new SmokeDialogRequest(Shell.DialogService, "missing", false, "UnregisteredSmokeDialog");
        var result = await request.Completion.WaitAsync(Timeout);
        state.Check(result.Exception is not null && request.CallbackCalls == 1,
            "An unregistered dialog returns an error through its callback exactly once.");
    }
    private async Task CheckUiThreadEventAsync()
    {
        var smokeEvent = container.Resolve<IEventAggregator>().GetEvent<SmokeEvent>();
        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var subscription = smokeEvent.Subscribe(_ => completion.TrySetResult(Shell.DispatcherQueue.HasThreadAccess),
            ThreadOption.UIThread, keepSubscriberReferenceAlive: true);
        try
        {
            await Task.Run(() => smokeEvent.Publish(Environment.CurrentManagedThreadId)).WaitAsync(Timeout);
            state.Check(await completion.Task.WaitAsync(Timeout), "Background event reaches the captured UI dispatcher.");
        }
        finally { smokeEvent.Unsubscribe(subscription); }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var watch = Stopwatch.StartNew();
        while (!condition())
        {
            if (watch.Elapsed >= Timeout)
                throw new TimeoutException("Timed out waiting for the WinUI lifecycle.");
            await Task.Delay(25);
        }
    }
}
