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
            Prism.Ioc.ContainerLocator.ResetContainer();
            var container = new Mock<IContainerExtension>().Object;
            var container2 = new Mock<IContainerExtension>().Object;

            Prism.Ioc.ContainerLocator.SetContainerExtension(() => container);
            Assert.Same(container, Prism.Ioc.ContainerLocator.Container);

            Prism.Ioc.ContainerLocator.SetContainerExtension(() => container2);
            Assert.NotSame(container2, Prism.Ioc.ContainerLocator.Container);
            Assert.Same(container, Prism.Ioc.ContainerLocator.Container);
        }
    }
}
