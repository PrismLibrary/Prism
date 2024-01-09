using Microsoft.Maui.Controls;
using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Builder;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Behaviors;

public class NavigationBehaviors : TestBase
{
    public NavigationBehaviors(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public void RootPageIsNotActive()
    {
        var rootPage = StartAndGetRootPage("NavigationPage/MockViewA/MockViewB");

        Assert.IsType<PrismNavigationPage>(rootPage);
        var navPage = (NavigationPage)rootPage;

        var viewA = navPage.Navigation.NavigationStack[0];
        var viewB = navPage.Navigation.NavigationStack[1];

        Assert.IsType<MockViewA>(viewA);
        Assert.IsType<MockViewB>(viewB);

        AssertIsActive(viewA, false);
        AssertIsActive(viewB, true);
    }

    [Fact]
    public void TabPageSetsFirstTabIsActive()
    {
        var rootPage = StartAndGetRootPage(b => b.AddTabbedSegment(t => t.CreateTab("MockViewA").CreateTab("MockViewB")));

        Assert.IsType<TabbedPage>(rootPage);

        var tabbed = (TabbedPage)rootPage;
        AssertIsActive(tabbed.Children[0], true);
        AssertIsActive(tabbed.Children[1], false);
    }

    [Fact]
    public void TabPageSetsSecondTabIsActive()
    {
        var rootPage = StartAndGetRootPage(b => b.AddTabbedSegment(t => t.CreateTab("MockViewA").CreateTab("MockViewB").SelectedTab("MockViewB")));

        Assert.IsType<TabbedPage>(rootPage);

        var tabbed = (TabbedPage)rootPage;
        AssertIsActive(tabbed.Children[0], false);
        AssertIsActive(tabbed.Children[1], true);
    }

    [Fact]
    public void TabPageSetsFirstTabIsActiveWithNavigationPage()
    {
        var rootPage = StartAndGetRootPage(b => b.AddTabbedSegment(t => t.CreateTab(tb => tb.AddNavigationPage().AddSegment("MockViewA")).CreateTab(tb => tb.AddNavigationPage().AddSegment("MockViewB"))));

        Assert.IsType<TabbedPage>(rootPage);

        var tabbed = (TabbedPage)rootPage;

        AssertIsActive(GetTabChild(tabbed, 0), true);
        AssertIsActive(GetTabChild(tabbed, 1), false);
    }

    [Fact]
    public void TabPageSetsSecondTabIsActiveWithNavigationPage()
    {
        var rootPage = StartAndGetRootPage(b =>
            b.AddTabbedSegment(t =>
                t.CreateTab(tb => tb.AddNavigationPage().AddSegment("MockViewA"))
                 .CreateTab(tb => tb.AddNavigationPage().AddSegment("MockViewB"))
                 .SelectedTab("NavigationPage|MockViewB")));

        Assert.IsType<TabbedPage>(rootPage);

        var tabbed = (TabbedPage)rootPage;
        AssertIsActive(GetTabChild(tabbed, 0), false);
        AssertIsActive(GetTabChild(tabbed, 1), true);
    }

    private void AssertIsActive(Page page, bool expected)
    {
        var viewModel = (MockViewModelBase)page.BindingContext;
        Assert.Equal(expected, viewModel.IsActive);
    }

    private Page GetTabChild(TabbedPage tabbed, int index)
    {
        var child = tabbed.Children[index];
        Assert.IsType<PrismNavigationPage>(child);
        var navPage = (NavigationPage)child;
        return navPage.CurrentPage;
    }

    private Page StartAndGetRootPage(Action<INavigationBuilder> initialNav)
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow((_, nav) =>
        {
            var navBuilder = nav.CreateBuilder();
            initialNav(navBuilder);
            return navBuilder.NavigateAsync();
        }))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page;

        Assert.NotNull(rootPage);
        return rootPage;
    }

    private Page StartAndGetRootPage(string uri)
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(uri))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page;

        Assert.NotNull(rootPage);
        return rootPage;
    }
}
