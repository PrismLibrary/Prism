using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace Prism.Container.Wpf.Mocks
{
    internal partial class NullLoggerBootstrapper : PrismBootstrapper
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }

        protected override DependencyObject CreateShell()
        {
            throw new System.NotImplementedException();
        }
    }
}
