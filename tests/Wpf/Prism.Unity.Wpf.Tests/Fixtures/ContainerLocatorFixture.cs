using System;
using System.Collections.Generic;
using Moq;
using Prism.Ioc;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;
using Xunit;

namespace Prism.Unity.Wpf.Tests
{
    public class ContainerLocatorFixture
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
    }
}
