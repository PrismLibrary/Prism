using Microsoft.UI.Xaml;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Modularity;

namespace NativeWinUISmoke;

public partial class App : Prism.DryIoc.PrismApplication
{
    private readonly SmokeState _state = new();

    public App()
    {
        InitializeComponent();
        UnhandledException += (_, args) => _state.Check(false, args.Exception.ToString());
    }

    protected override Window CreateWindow() => new() { Title = "Prism native WinUI smoke: primary" };

    protected override UIElement CreateShell() => Container.Resolve<SmokeShell>();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(_state);
        containerRegistry.RegisterDialogWindow<ObservedDialogWindow>(nameof(ObservedDialogWindow));
        SmokeNavigationChecks.RegisterTypes(containerRegistry);
        SmokeAdapterChecks.RegisterTypes(containerRegistry);
        containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
        containerRegistry.RegisterForNavigation<DefaultView, DefaultViewModel>();
        containerRegistry.RegisterForNavigation<DefaultView, DefaultViewModel>("LateDefault");
        containerRegistry.RegisterDialog<SmokeDialog, SmokeDialogViewModel>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) =>
        moduleCatalog.AddModule<SmokeModule>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var window = Container.Resolve<Window>();
        var shell = (SmokeShell)window.Content;
        _state.Check(_state.ModuleInitializedCount == 1,
            "Module initialized exactly once before App.OnInitialized.");
        SmokeChecks.CheckInitialized(_state, RegionManager, shell);

        // Let OnInitialized return and FinishLoading finish before starting checks.
        if (!shell.DispatcherQueue.TryEnqueue(async () =>
            await new SmokeChecks(Container, window, _state).RunAsync()))
        {
            _state.Check(false, "Unable to queue smoke checks.");
            Environment.ExitCode = 1;
        }
    }
}
