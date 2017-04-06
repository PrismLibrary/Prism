using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.Modularity;

namespace Prism.StructureMap.Wpf.Tests
{
    [TestClass]
    public class StructureMapBootstrapperNullModuleCatalogFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void NullModuleCatalogThrowsOnDefaultModuleInitialization()
        {
            var bootstrapper = new NullModuleCatalogBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IModuleCatalog");
        }

        private class NullModuleCatalogBootstrapper : StructureMapBootstrapper
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
}