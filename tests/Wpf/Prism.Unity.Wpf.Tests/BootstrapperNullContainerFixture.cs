using System;
using Prism.Container.Wpf.Mocks;
using Prism.IocContainer.Wpf.Tests.Support;
using Xunit;

namespace Prism.Unity.Wpf.Tests
{
    public class BootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IUnityContainer");
        }
    }
}