using System;
using Prism.Container.Wpf.Mocks;
using Prism.IocContainer.Wpf.Tests.Support;
using Xunit;
using static Prism.Container.Wpf.Tests.ContainerHelper;

namespace Prism.Container.Wpf.Tests.Bootstrapper
{
    public class BootstrapperNullModuleCatalogFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void NullModuleCatalogThrowsOnDefaultModuleInitialization()
        {
            var bootstrapper = new NullModuleCatalogBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IModuleCatalog");
        }
    }
}
