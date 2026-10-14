using Xunit;

namespace Prism.Maui.Tests.Fixtures.Regions;

// Keep dispatcher setup isolated from other lifecycle fixtures.
[CollectionDefinition(nameof(AutoPopulateDefaultViewCollection), DisableParallelization = true)]
public class AutoPopulateDefaultViewCollection
{
}
