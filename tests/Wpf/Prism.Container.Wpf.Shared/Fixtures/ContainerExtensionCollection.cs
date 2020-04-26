using Xunit;

namespace Prism.Container.Wpf.Tests
{
    public class ContainerExtension { }

    [CollectionDefinition(nameof(ContainerExtension), DisableParallelization = true)]
    public class ContainerExtensionCollection : ICollectionFixture<ContainerExtension>
    {
    }
}
