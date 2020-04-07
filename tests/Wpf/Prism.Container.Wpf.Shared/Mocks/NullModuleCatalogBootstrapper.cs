using System;
using System.Windows;
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
            throw new NotImplementedException();
        }

        protected override void InitializeShell()
        {
            throw new NotImplementedException();
        }
    }
}
