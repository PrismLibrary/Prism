using System;
using System.Windows;
using Xunit;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support;

namespace Prism.Ninject.Wpf.Tests
{
    
    public class NinjectBootstrapperNullKernelFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void RunThrowsWhenNullKernelCreated()
        {
            var bootstrapper = new NullKernelBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IKernel");
        }

        private class NullKernelBootstrapper : NinjectBootstrapper
        {
            protected override IKernel CreateKernel()
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