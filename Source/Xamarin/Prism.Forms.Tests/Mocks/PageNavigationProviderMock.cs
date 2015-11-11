using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationProviderMock : IPageNavigationProvider
    {
        public Page CreatePageForNavigation(Page sourcePage, Page targetPage)
        {
            return targetPage;
        }
    }
}
