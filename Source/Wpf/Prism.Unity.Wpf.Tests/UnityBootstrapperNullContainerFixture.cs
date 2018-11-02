

using System;
using System.Windows;
using Unity;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;

namespace Prism.Unity.Wpf.Tests
{
    
    public class UnityBootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IUnityContainer");
        }

        private class NullContainerBootstrapper : UnityBootstrapper
        {
            protected override IUnityContainer CreateContainer()
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