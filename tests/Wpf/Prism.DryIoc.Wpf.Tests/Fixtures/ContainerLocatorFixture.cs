using DryIoc;
using Moq;
using Prism.Ioc;
using Xunit;

namespace Prism.DryIoc.Wpf.Tests
{
    public class ContainerLocatorFixture
    {
        [Fact]
        public void ShouldForwardResolveToInnerContainer()
        {
            var mockContainer = new Mock<IContainer>();

            Assert.Empty(mockContainer.Invocations);
            var containerExtension = new DryIocContainerExtension(mockContainer.Object);

            // We register the IContainerExtension and IContainerProvider directly with the container in the ctor.
            Assert.Equal(2, mockContainer.Invocations.Count);
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => containerExtension);

            var resolved = ContainerLocator.Container.Resolve(typeof(object));
            Assert.Equal(3, mockContainer.Invocations.Count);
        }
    }
}
