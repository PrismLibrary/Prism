using System;
using Moq;
using Prism.Ioc;
using Xunit;

namespace Prism.Tests.Ioc
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
            Prism.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Prism.Ioc.ContainerLocator.Current);
            Prism.Ioc.ContainerLocator.SetContainerExtension(() => new Mock<IContainerExtension>().Object);
            Assert.NotNull(Prism.Ioc.ContainerLocator.Current);
        }

        [Fact]
        public void ResetNullsCurrentContainer()
        {
            Prism.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Prism.Ioc.ContainerLocator.Current);
            Prism.Ioc.ContainerLocator.SetContainerExtension(() => new Mock<IContainerExtension>().Object);
            Assert.NotNull(Prism.Ioc.ContainerLocator.Current);
            Prism.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Prism.Ioc.ContainerLocator.Current);
        }

        [Fact]
        public void FactoryOnlySetsContainerOnce()
        {
            var container = Mock.Of<IContainerExtension>();
            var container2 = Mock.Of<IContainerExtension>();

            Prism.Ioc.ContainerLocator.SetContainerExtension(() => container);
            Assert.Same(container, Prism.Ioc.ContainerLocator.Container);

            var ex = Record.Exception(() => Prism.Ioc.ContainerLocator.SetContainerExtension(() => container2));
            Assert.NotNull(ex);
            Assert.Contains("The Current container is not null", ex.Message);
        }
    }
}
