using System;
using DryIoc;
using Moq;
using Prism.Container.Wpf.Tests;
using Prism.Ioc;
using Xunit;

namespace Prism.DryIoc.Wpf.Tests
{
    [Collection(nameof(ContainerExtension))]
    public class ContainerLocatorFixture : IDisposable
    {
        [Fact(Skip = "We need to redo this")]
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

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
        }
    }
}
