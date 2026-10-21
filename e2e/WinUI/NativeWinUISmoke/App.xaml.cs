using Microsoft.UI.Xaml;
using Prism.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public partial class App : Prism.DryIoc.PrismApplication
{
    private readonly SmokeState _state = new();
    private SmokeShell? _shell;

    public App()
    {
        InitializeComponent();
        UnhandledException += (_, args) => _state.Check(false, args.Exception.ToString());
    }

    protected override Window CreateWindow() => new() { Title = "Prism native WinUI smoke" };

    protected override UIElement CreateShell() => _shell = Container.Resolve<SmokeShell>();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(_state);
        containerRegistry.RegisterForNavigation<HomeView>();
        containerRegistry.RegisterDialog<SmokeDialog, SmokeDialogViewModel>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) =>
        moduleCatalog.AddModule<SmokeModule>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _state.Check(_state.ModuleInitialized, "Module initialized before App.OnInitialized.");
        _state.Check(_shell is { IsLoaded: true }, "Native shell is loaded before App.OnInitialized.");
        _state.Check(MainWindow.Content == _shell, "MainWindow owns the native shell.");
        _state.Check(RegionManager.Regions.ContainsRegionWithName(SmokeShell.RegionName),
            "ContentControl region registered before App.OnInitialized.");
        _state.OnInitializedCompleted = true;

        // Let the synchronous OnInitialized/FinishLoading sequence finish first.
        if (!_shell!.DispatcherQueue.TryEnqueue(async () => await RunChecksAsync()))
        {
            _state.Check(false, "Unable to queue smoke checks.");
            Environment.ExitCode = 1;
        }
    }

    private async Task RunChecksAsync()
    {
        try
        {
            _state.Check(_state.FinishLoadingCalls == 1,
                "ILoadableShell.FinishLoading called exactly once after OnInitialized.");
            await NavigateAsync<HomeView>();
            await NavigateAsync<ModuleView>();
            await CheckUiThreadEventAsync();
            await CheckDialogAsync();
        }
        catch (Exception exception)
        {
            _state.Check(false, exception.ToString());
        }
        finally
        {
            _state.Write(_state.Failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({_state.Failures} checks)");
            Environment.ExitCode = _state.Failures == 0 ? 0 : 1;
            _shell!.EnableManualChecks();
            if (Environment.GetEnvironmentVariable("PRISM_WINUI_SMOKE_AUTOCLOSE") == "1")
                MainWindow.Close();
        }
    }

    private async Task NavigateAsync<TView>()
    {
        var completion = new TaskCompletionSource<NavigationResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        RegionManager.RequestNavigate(SmokeShell.RegionName, typeof(TView).Name,
            result => completion.TrySetResult(result));
        var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
        _state.Check(result.Success && _shell!.RegionHost.Content is TView,
            $"Region navigation to {typeof(TView).Name}: {result.Exception?.Message ?? "view activated"}.");
    }

    private async Task CheckUiThreadEventAsync()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        // The first application-level resolution intentionally happens off the UI thread.
        // Prism must have captured the UI synchronization context during bootstrap.
        var smokeEvent = await Task.Run(() => Container.Resolve<IEventAggregator>().GetEvent<SmokeEvent>());
        var completion = new TaskCompletionSource<(bool HasThreadAccess, int PublisherThread)>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var subscription = smokeEvent.Subscribe(publisherThread =>
            completion.TrySetResult((_shell!.DispatcherQueue.HasThreadAccess, publisherThread)),
            ThreadOption.UIThread, keepSubscriberReferenceAlive: true);
        try
        {
            await Task.Run(() => smokeEvent.Publish(Environment.CurrentManagedThreadId));
            var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
            _state.Check(result.HasThreadAccess && result.PublisherThread != uiThreadId,
                "Background publication delivered to a ThreadOption.UIThread subscriber on the UI dispatcher.");
        }
        finally
        {
            smokeEvent.Unsubscribe(subscription);
        }
    }

    private async Task CheckDialogAsync()
    {
        var completion = new TaskCompletionSource<IDialogResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var callback = new DialogCallback()
            .OnClose(result => completion.TrySetResult(result))
            .OnError((Exception exception) => completion.TrySetException(exception));
        Container.Resolve<IDialogService>().ShowDialog(nameof(SmokeDialog),
            new DialogParameters { { "autoClose", true } }, callback);
        var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
        _state.Check(result.Exception is null && result.Result == ButtonResult.OK &&
            _state.DialogOpenedCount == 1 && _state.DialogClosedCount == 1,
            "Prism ContentDialog opened, closed through RequestClose, and completed its callback.");
    }
}
