using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.Views;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class DynamicTabbedPageNavigationFixture : TestBase
{
    public DynamicTabbedPageNavigationFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public void CreatesTabs_WithSingleContentPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(navigation =>
        navigation.CreateBuilder()
            .AddTabbedSegment(t =>
                t.CreateTab("MockViewA")
                    .CreateTab("MockViewB"))
            .NavigateAsync())).Build();
        var window = GetWindow(mauiApp);
        Assert.IsType<TabbedPage>(window.Page);
        var tabbedPage = window.Page as TabbedPage;

        Assert.Equal(2, tabbedPage.Children.Count);
        Assert.IsType<MockViewA>(tabbedPage.Children[0]);
        Assert.IsType<MockViewB>(tabbedPage.Children[1]);

        Assert.Same(tabbedPage.Children[0], tabbedPage.CurrentPage);
    }

    [Fact]
    public void CreatesTabs_WithNavigationPageAndContentPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(navigation =>
            navigation.CreateBuilder()
                .AddTabbedSegment(t =>
                    t.CreateTab(ct => ct.AddNavigationPage().AddSegment("MockViewA"))
                     .CreateTab(ct => ct.AddNavigationPage().AddSegment("MockViewB")))
                .NavigateAsync())).Build();
        var window = GetWindow(mauiApp);
        Assert.IsType<TabbedPage>(window.Page);
        var tabbedPage = window.Page as TabbedPage;

        Assert.Equal(2, tabbedPage.Children.Count);
        Assert.IsType<PrismNavigationPage>(tabbedPage.Children[0]);
        var tab0 = tabbedPage.Children[0] as NavigationPage;
        Assert.IsType<MockViewA>(tab0.CurrentPage);
        Assert.IsType<PrismNavigationPage>(tabbedPage.Children[1]);
        var tab1 = tabbedPage.Children[1] as NavigationPage;
        Assert.IsType<MockViewB>(tab1.CurrentPage);

        Assert.Same(tabbedPage.Children[0], tabbedPage.CurrentPage);
    }

    [Fact]
    public async Task NavigatesModally_FromChild_OfNavigationPageTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(navigation =>
            navigation.CreateBuilder()
                .AddTabbedSegment(t =>
                    t.CreateTab(ct => ct.AddNavigationPage().AddSegment("MockViewA"))
                     .CreateTab(ct => ct.AddNavigationPage().AddSegment("MockViewB")))
                .NavigateAsync())).Build();
        var window = GetWindow(mauiApp);
        Assert.IsType<TabbedPage>(window.Page);
        var tabbedPage = window.Page as TabbedPage;

        Assert.IsType<PrismNavigationPage>(tabbedPage.CurrentPage);
        var navPage = (PrismNavigationPage)tabbedPage.CurrentPage;
        Assert.Empty(navPage.Navigation.ModalStack);

        var navService = Prism.Navigation.Xaml.Navigation.GetNavigationService(navPage.CurrentPage);
        var result = await navService.CreateBuilder()
            .AddSegment("MockViewC", useModalNavigation: true)
            .NavigateAsync();

        Assert.True(result.Success);

        Assert.Single(navPage.Navigation.ModalStack);

        var modalNavService = Prism.Navigation.Xaml.Navigation.GetNavigationService(navPage.Navigation.ModalStack[0]);

        result = await modalNavService.NavigateAsync("MockViewD");
        Assert.True(result.Success);

        Assert.Equal(2, navPage.Navigation.ModalStack.Count);
    }
}
