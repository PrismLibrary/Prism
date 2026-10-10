
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

    [Fact]
    public async Task NavigationInSecondaryWindowPreservesPrimaryPageScope()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("MockViewA"))
            .Build();
        var primary = GetWindow(mauiApp);
        var primaryPage = primary.Page;
        var primaryScope = primaryPage.GetContainerProvider();
        var secondaryScope = primaryScope.CreateScope();
        var registry = primaryScope.Resolve<INavigationRegistry>();
        var secondaryPage = (Page)registry.CreateView(secondaryScope, "MockViewB");
        var secondary = new PrismWindow("Secondary") { Page = secondaryPage };
        var primaryNavigation = primaryScope.Resolve<INavigationService>();
        var secondaryNavigation = secondaryScope.Resolve<INavigationService>();

        Assert.NotSame(primaryScope, secondaryScope);
        Assert.NotSame(primaryNavigation, secondaryNavigation);
        Assert.Same(primaryPage, primaryScope.Resolve<Prism.Common.IPageAccessor>().Page);
        Assert.Same(secondaryPage, secondaryScope.Resolve<Prism.Common.IPageAccessor>().Page);

        var result = await secondaryNavigation.NavigateAsync("/MockViewC");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(primaryPage, primary.Page);
        Assert.Same(primaryScope, primary.Page.GetContainerProvider());
        Assert.Same(primaryNavigation, primaryScope.Resolve<INavigationService>());
        Assert.Same(primaryPage, primaryScope.Resolve<Prism.Common.IPageAccessor>().Page);
        Assert.IsType<Prism.DryIoc.Maui.Tests.Mocks.Views.MockViewC>(secondary.Page);
        Assert.NotSame(primaryScope, secondary.Page.GetContainerProvider());
    }
}
