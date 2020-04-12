using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Prism.Container.Wpf.Tests
{
    public class ContainerExtension { }

    [CollectionDefinition(nameof(ContainerExtension), DisableParallelization = true)]
    public class ContainerExtensionCollection : ICollectionFixture<ContainerExtension>
    {
    }
}
