using System;
using Avalonia;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Avalonia.Tests.Support;
using Prism.Modularity;

namespace Prism.DryIoc.Avalonia.Tests
{
    [TestClass]
    public class DryIocBootstrapperNullModuleCatalogFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void NullModuleCatalogThrowsOnDefaultModuleInitialization()
        {
            var bootstrapper = new NullModuleCatalogBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IModuleCatalog");
        }

        private class NullModuleCatalogBootstrapper : DryIocBootstrapper
        {
            protected override IModuleCatalog CreateModuleCatalog()
            {
                return null;
            }

            protected override IStyledProperty CreateShell()
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
