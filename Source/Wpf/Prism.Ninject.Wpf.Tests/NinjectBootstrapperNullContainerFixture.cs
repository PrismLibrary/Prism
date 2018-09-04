using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support;

namespace Prism.Ninject.Wpf.Tests
{
    [TestClass]
    public class NinjectBootstrapperNullKernelFixture : BootstrapperFixtureBase
    {
        [TestMethod]
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