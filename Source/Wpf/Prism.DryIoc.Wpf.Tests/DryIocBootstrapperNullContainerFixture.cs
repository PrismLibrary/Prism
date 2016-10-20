using System;
using System.Windows;
using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support;

namespace Prism.DryIoc.Wpf.Tests
{
    [TestClass]
    public class DryIocBootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IContainer");
        }

        private class NullContainerBootstrapper : DryIocBootstrapper
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
