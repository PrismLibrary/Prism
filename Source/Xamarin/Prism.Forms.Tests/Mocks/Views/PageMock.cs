using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class PageMock : Page
    {
    }

    [PageNavigationOptions()]
    public class PageWithDefaultNavigationOptionsMock
    {
    }

    [PageNavigationOptions(UseModalNavigation = false, Animated = false, PageNavigationProviderType = typeof(PageNavigationProviderMock))]
    public class PageWithAllPageNavigationOptionsMock
    {
    }

    [PageNavigationOptions(PageNavigationProviderType = typeof(PageNavigationProviderMock))]
    public class PageWithNavigationProviderMock
    {
    }

    [PageNavigationOptions(PageNavigationProviderType = typeof(PageMock))]
    public class PageWithInvalidPageNavigationProviderMock
    {
    }
}
