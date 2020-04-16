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
            object myInstance = new object();
            var mockContainer = new Mock<IContainer>();
            mockContainer.Setup(c => c.Resolve(typeof(object), IfUnresolved.Throw)).Returns(myInstance);

            var containerExtension = new DryIocContainerExtension(mockContainer.Object);
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => containerExtension);

            var resolved = ContainerLocator.Container.Resolve(typeof(object));
            mockContainer.Verify(c => c.Resolve(typeof(object), IfUnresolved.Throw));
            Assert.Same(myInstance, resolved);
        }
    }
}