using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.Views;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class PrismWindowTests : TestBase
{
    public PrismWindowTests(ITestOutputHelper testOutputHelper) 
        : base(testOutputHelper)
    {
    }

    [Fact]
    public void CurrentPageEqualsRootPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewA>(window.CurrentPage);
        Assert.IsType<MockViewA>(window.Page);
        Assert.True(window.IsRootPage);
    }

    [Fact]
    public void CurrentPage_FromNavigationPage_EqualsRootPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewA>(window.CurrentPage);
        Assert.IsType<PrismNavigationPage>(window.Page);
        Assert.True(window.IsRootPage);
    }

    [Fact]
    public void CurrentPage_FromNavigationPage_IsNotRootPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewB>(window.CurrentPage);
        Assert.IsType<PrismNavigationPage>(window.Page);
        Assert.False(window.IsRootPage);
    }

    [Fact]
    public void CurrentPage_IsRoot_FromTabbedPage_WithNavigationPageTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(n =>
            n.CreateBuilder()
             .AddTabbedSegment(b => b.CreateTab(t => t.AddNavigationPage().AddSegment("MockViewA"))
                .CreateTab("MockViewB")).NavigateAsync()))
             .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewA>(window.CurrentPage);
        Assert.IsType<TabbedPage>(window.Page);
        Assert.True(window.IsRootPage);
    }

    [Fact]
    public void CurrentPage_IsNotRoot_FromTabbedPage_WithDeepLinkedNavigationPageTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(n =>
            n.CreateBuilder()
             .AddTabbedSegment(b => b.CreateTab(t => 
                    t.AddNavigationPage()
                     .AddSegment("MockViewA")
                     .AddSegment("MockViewC"))
                .CreateTab("MockViewB")).NavigateAsync()))
             .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewC>(window.CurrentPage);
        Assert.IsType<TabbedPage>(window.Page);
        Assert.False(window.IsRootPage);
    }

    [Fact]
    public void CurrentPage_IsRoot_WithFlyoutPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(n =>
            n.CreateBuilder()
             .AddSegment("MockHome")
             .AddNavigationPage()
             .AddSegment("MockViewA")
             .NavigateAsync()))
            .Build();

        var window = GetWindow(mauiApp);

        Assert.IsType<MockViewA>(window.CurrentPage);
        Assert.IsType<MockHome>(window.Page);
        Assert.True(window.IsRootPage);
    }
}
