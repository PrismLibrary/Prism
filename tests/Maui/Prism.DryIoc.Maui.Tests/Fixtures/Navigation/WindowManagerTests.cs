
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class WindowManagerTests : TestBase
{
    public WindowManagerTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("NavigationPage/MockViewA/MockViewB/MockViewC")]
    [InlineData("MockHome/NavigationPage/MockViewA")]
    public void WindowManagerGetsNavigationServiceFromCurrentPage(string uri)
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(uri))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page;
        var currentPage = rootPage;
        if (rootPage is NavigationPage navigationPage)
        {
            currentPage = navigationPage.CurrentPage;
        }
        if (rootPage is FlyoutPage flyoutPage && flyoutPage.Detail is NavigationPage detailPage)
        {
            currentPage = detailPage.CurrentPage;
        }

        var currentNavigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(currentPage);
        var windowManager = rootPage.GetContainerProvider().Resolve<IWindowManager>();
        Assert.Same(currentNavigationService, windowManager.GetCurrentNavigationService());
    }
}
