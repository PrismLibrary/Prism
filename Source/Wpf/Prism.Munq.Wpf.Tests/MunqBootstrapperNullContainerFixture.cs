using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support;

namespace Prism.Munq.Wpf.Tests
{

    [TestClass]
    public class MunqBootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IocContainer");
        }

        private class NullContainerBootstrapper : MunqBootstrapper
        {
            protected override IMunqContainer CreateContainer()
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