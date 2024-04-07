using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigationSelectTabTests : TestBase
{
    public NavigationSelectTabTests(ITestOutputHelper testOutputHelper) 
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task SelectsTab_NavigatesWithinTab_NavigationPage()
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(c => c.RegisterForNavigation<ForcedView>())
            .CreateWindow("TabbedPage?createTab=MockViewA&createTab=NavigationPage%2FMockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewA>(window.CurrentPage);
        var navigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(window.CurrentPage);
        var result = await navigationService.SelectTabAsync("NavigationPage|MockViewB", "MockViewC/MockViewD");

        Assert.True(result.Success);
        Assert.Null(result.Exception);

        Assert.IsType<TabbedPage>(window.Page);
        var tabbedPage = window.Page as TabbedPage;
        Assert.IsType<PrismNavigationPage>(tabbedPage.CurrentPage);
        var navPage = tabbedPage.CurrentPage as PrismNavigationPage;
        Assert.IsType<MockViewB>(navPage.RootPage);
        Assert.IsType<MockViewD>(navPage.CurrentPage);
        Assert.Equal(3, navPage.Navigation.NavigationStack.Count);
    }

    [Fact]
    public async Task TabbedPage_SelectTabSets_CurrentTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("TabbedPage?createTab=MockViewA&createTab=MockViewB&selectedTab=MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbedPage = (TabbedPage)window.Page;
        Assert.NotNull(tabbedPage);
        Assert.IsType<MockViewB>(tabbedPage.CurrentPage);
    }

    [Fact]
    public async Task TabbedPage_SelectTab_SetsCurrentTab_WithNavigationPageTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("TabbedPage?createTab=NavigationPage%2FMockViewA&createTab=NavigationPage%2FMockViewB&selectedTab=NavigationPage|MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbedPage = (TabbedPage)window.Page;
        Assert.NotNull(tabbedPage);
        var navPage = tabbedPage.CurrentPage as NavigationPage;
        Assert.NotNull(navPage);
        Assert.IsType<MockViewB>(navPage.CurrentPage);
    }

    [Fact]
    public async Task TabbedPage_SelectsNewTab()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(nav => nav.CreateBuilder()
                .AddTabbedSegment(s => s.CreateTab("MockViewA")
                                                       .CreateTab("MockViewB")
                                                       .CreateTab("MockViewC"))
                .NavigateAsync()))
            .Build();
        var window = GetWindow(mauiApp);
        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbed = window.Page as TabbedPage;

        Assert.NotNull(tabbed);

        Assert.IsType<MockViewA>(tabbed.CurrentPage);
        var mockViewA = tabbed.CurrentPage;
        var mockViewANav = Prism.Navigation.Xaml.Navigation.GetNavigationService(mockViewA);

        await mockViewANav.SelectTabAsync("MockViewB");

        Assert.IsNotType<MockViewA>(tabbed.CurrentPage);
        Assert.IsType<MockViewB>(tabbed.CurrentPage);
    }

    [Fact]
    public async Task TabbedPage_SelectsNewTab_WithNavigationParameters()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(nav => nav.CreateBuilder()
                .AddTabbedSegment(s => s.CreateTab("MockViewA")
                                                       .CreateTab("MockViewB")
                                                       .CreateTab("MockViewC"))
                .NavigateAsync()))
            .Build();
        var window = GetWindow(mauiApp);
        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbed = window.Page as TabbedPage;

        Assert.NotNull(tabbed);

        Assert.IsType<MockViewA>(tabbed.CurrentPage);
        var mockViewA = tabbed.CurrentPage;
        var mockViewANav = Prism.Navigation.Xaml.Navigation.GetNavigationService(mockViewA);

        var expectedMessage = nameof(TabbedPage_SelectsNewTab_WithNavigationParameters);
        await mockViewANav.SelectTabAsync("MockViewB", new NavigationParameters { { "Message", expectedMessage } });

        Assert.IsNotType<MockViewA>(tabbed.CurrentPage);
        Assert.IsType<MockViewB>(tabbed.CurrentPage);

        var viewModel = tabbed.CurrentPage.BindingContext as MockViewBViewModel;
        Assert.Equal(expectedMessage, viewModel?.Message);
    }
}
