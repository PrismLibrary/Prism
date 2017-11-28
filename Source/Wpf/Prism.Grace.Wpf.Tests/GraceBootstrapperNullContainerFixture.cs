using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support;
using Grace.DependencyInjection;

namespace Prism.Grace.Wpf.Tests
{
    [TestClass]
    public class GraceBootstrapperNullContainerFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "DependencyInjectionContainer");
        }

        private class NullContainerBootstrapper : GraceBootstrapper
        {
            protected override DependencyInjectionContainer CreateContainer()
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