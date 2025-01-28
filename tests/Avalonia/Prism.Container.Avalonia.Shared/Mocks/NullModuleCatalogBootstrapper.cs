using Avalonia;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Container.Avalonia.Mocks
{
    internal partial class NullModuleCatalogBootstrapper
    {
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return null;
        }

        protected override AvaloniaObject CreateShell()
        {
            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
