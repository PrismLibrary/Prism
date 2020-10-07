using System.Windows;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Container.Wpf.Mocks
{
    internal partial class NullModuleCatalogBootstrapper
    {
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return null;
        }

        protected override DependencyObject CreateShell()
        {
            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
