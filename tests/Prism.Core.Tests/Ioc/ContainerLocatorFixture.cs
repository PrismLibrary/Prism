using System;
using System.Collections.Generic;
using System.Text;
using Prism.Core.Tests.Mocks.Ioc;
using Prism.Ioc;
using Xunit;

namespace Prism.Core.Tests.Ioc
{
    public class ContainerLocator { }

    [CollectionDefinition(nameof(ContainerLocator), DisableParallelization = true)]
    public class ContainerLocatorCollection : ICollectionFixture<ContainerLocator>
    {
    }

    [Collection(nameof(ContainerLocator))]
    public class ContainerLocatorFixture
    {
        [Fact]
        public void FactoryCreatesContainerExtension()
        {
            Assert.Null(Prism.Ioc.ContainerLocator.Current);
            Prism.Ioc.ContainerLocator.SetContainerExtension(() => new MockContainerExtension());
            Assert.NotNull(Prism.Ioc.ContainerLocator.Current);
        }

        [Fact]
        public void ResetNullsCurrentContainer()
        {
            Prism.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Prism.Ioc.ContainerLocator.Current);
            Prism.Ioc.ContainerLocator.SetContainerExtension(() => new MockContainerExtension());
            Assert.NotNull(Prism.Ioc.ContainerLocator.Current);
            Prism.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Prism.Ioc.ContainerLocator.Current);
        }

        [Fact]
        public void FactoryOnlySetsContainerOnce()
        {
            Prism.Ioc.ContainerLocator.ResetContainer();
            var container = new MockContainerExtension();
            var container2 = new Mock2ContainerExtension();

            Prism.Ioc.ContainerLocator.SetContainerExtension(() => container);
            Assert.Same(container, Prism.Ioc.ContainerLocator.Container);

            Prism.Ioc.ContainerLocator.SetContainerExtension(() => container2);
            Assert.IsNotType<Mock2ContainerExtension>(Prism.Ioc.ContainerLocator.Container);
            Assert.Same(container, Prism.Ioc.ContainerLocator.Container);
        }
    }
}
