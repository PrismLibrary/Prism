using Xunit;

namespace Prism.Forms.Tests
{
    public class ContainerExtension { }

    [CollectionDefinition(nameof(ContainerExtension), DisableParallelization = true)]
    public class ContainerExtensionCollection : ICollectionFixture<ContainerExtension>
    {
    }
}
