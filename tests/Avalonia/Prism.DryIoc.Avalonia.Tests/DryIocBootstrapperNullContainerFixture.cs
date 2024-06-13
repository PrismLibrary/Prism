using System;
using Avalonia;
using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Avalonia.Tests.Support;

namespace Prism.DryIoc.Avalonia.Tests
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

            protected override IStyledProperty CreateShell()
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
