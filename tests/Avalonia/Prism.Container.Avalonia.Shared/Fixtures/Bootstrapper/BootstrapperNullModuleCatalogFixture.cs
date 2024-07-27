using System;
using Prism.Container.Avalonia.Mocks;
using Prism.IocContainer.Avalonia.Tests.Support;
using Xunit;

namespace Prism.Container.Avalonia.Tests.Bootstrapper
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
