using HelloWorld.Views;
using Playground.Module;

namespace HelloWorld;

public partial class App : PrismApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override UIElement CreateShell(IContainerProvider container)
    {
        return container.Resolve<Shell>();
    }

    protected override void OnInitialized(Prism.Windows.IWindowContext window)
    {
        window.Window.Title = "Prism native WinUI playground";
        window.RegionManager.RequestNavigate("ContentRegion", "HomeView");
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<PlaygroundModule>();
    }
}
