using HelloWorld.Views;
using Playground.Module;

namespace HelloWorld;

public partial class App : PrismApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override UIElement CreateShell()
    {
        return Container.Resolve<Shell>();
    }

    protected override void ConfigureWindow(Window window)
    {
        window.Title = "Prism Sandbox | WinUI";
        window.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "prism-sandbox.ico"));
    }

    protected override void OnInitialized() =>
        RegionManager.RequestNavigate("ContentRegion", "HomeView");

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<PlaygroundModule>();
    }
}
