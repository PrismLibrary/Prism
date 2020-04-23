using Prism.Navigation;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigation { }

    [CollectionDefinition(nameof(PageNavigation), DisableParallelization = true)]
    public class PageNavigationCollection : ICollectionFixture<PageNavigation>
    {
    }
}
