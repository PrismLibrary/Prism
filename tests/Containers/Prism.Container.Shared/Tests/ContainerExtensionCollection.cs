using Xunit;

namespace Prism.Ioc.Tests
{
    [CollectionDefinition(nameof(ContainerExtension), DisableParallelization = true)]
    public class ContainerExtensionCollection : ICollectionFixture<ContainerExtension>
    {
    }
}
