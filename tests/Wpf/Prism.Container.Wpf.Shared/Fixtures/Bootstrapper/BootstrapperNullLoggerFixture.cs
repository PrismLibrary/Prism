using System;
using Prism.Container.Wpf.Mocks;
using Prism.IocContainer.Wpf.Tests.Support;
using Xunit;
using static Prism.Container.Wpf.Tests.ContainerHelper;

namespace Prism.Container.Wpf.Tests.Bootstrapper
{
    public class BootstrapperNullLoggerFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void NullLoggerThrows()
        {
            var bootstrapper = new NullLoggerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "ILoggerFacade");
        }
    }
}
