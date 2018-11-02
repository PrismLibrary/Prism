using System;
using System.Windows;
using Autofac;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;

namespace Prism.Autofac.Wpf.Tests
{
    
    public class AutofacBootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void RunThrowsWhenNullContainerBuilderCreated()
        {
            var bootstrapper = new NullContainerBuilderBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "ContainerBuilder");
        }

        [Fact]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IContainer");
        }

        private class NullContainerBuilderBootstrapper : AutofacBootstrapper
        {
            protected override ContainerBuilder CreateContainerBuilder()
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

        private class NullContainerBootstrapper : AutofacBootstrapper
        {
            protected override IContainer CreateContainer(ContainerBuilder builder)
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
