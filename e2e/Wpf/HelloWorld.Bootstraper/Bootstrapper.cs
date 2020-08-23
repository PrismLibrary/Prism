using HelloWorld.Modules.ModuleA;
using HelloWorld.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;
using Unity;

namespace HelloWorld
{
    class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSharedSamples();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<ModuleAModule>();
        }
    }
}