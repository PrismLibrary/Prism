using HelloWorld.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using Unity;

namespace HelloWorld
{
    class Bootstrapper : PrismBootstrapper
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}