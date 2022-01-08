using System;
using Prism.Ioc;
using Xunit;

namespace Prism.Container.Wpf.Tests
{
    public class ContainerExtension : IDisposable
    {
        public ContainerExtension()
        {
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(ContainerHelper.CreateContainerExtension());
        }

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
        }
    }

    [CollectionDefinition(nameof(ContainerExtension), DisableParallelization = true)]
    public class ContainerExtensionCollection : ICollectionFixture<ContainerExtension>
    {
    }
}
