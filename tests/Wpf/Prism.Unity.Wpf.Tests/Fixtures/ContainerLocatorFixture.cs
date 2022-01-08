using System;
using Moq;
using Prism.Container.Wpf.Tests;
using Prism.Ioc;
using Unity;
using Unity.Resolution;
using Xunit;

namespace Prism.Unity.Wpf.Tests
{
    [Collection(nameof(ContainerExtension))]
    public class ContainerLocatorFixture : IDisposable
    {
        [Fact]
        public void ShouldForwardResolveToInnerContainer()
        {
            object myInstance = new object();

            var mockContainer = new Mock<IUnityContainer>();
            mockContainer.Setup(c => c.Resolve(typeof(object), null, It.IsAny<ResolverOverride[]>())).Returns(myInstance);
            var containerExtension = new UnityContainerExtension(mockContainer.Object);
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => containerExtension);
            var resolved = ContainerLocator.Container.Resolve(typeof(object));
            mockContainer.Verify(c => c.Resolve(typeof(object), null, It.IsAny<ResolverOverride[]>()), Times.Once);
            Assert.Same(myInstance, resolved);
        }

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
        }
    }
}
