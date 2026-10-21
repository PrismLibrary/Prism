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

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<PlaygroundModule>();
    }
}
