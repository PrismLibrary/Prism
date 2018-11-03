using System;
using System.Windows;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;
using StructureMap;

namespace Prism.StructureMap.Wpf.Tests
{
    
    public class StructureMapBootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IContainer");
        }

        private class NullContainerBootstrapper : StructureMapBootstrapper
        {
            protected override IContainer CreateContainer()
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