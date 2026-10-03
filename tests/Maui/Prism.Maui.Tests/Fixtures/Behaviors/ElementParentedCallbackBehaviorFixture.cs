using Moq;
using Prism.Behaviors;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation.Xaml;

namespace Prism.Maui.Tests.Fixtures.Behaviors;

public class ElementParentedCallbackBehaviorFixture
{
    public ElementParentedCallbackBehaviorFixture()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FlyoutMenu_InvokesCallbackWhenInheritedScopeBecomesAvailable(bool scopeBeforeParenting)
    {
        var host = new ContentView();
        var callbackCount = 0;
        host.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++));
        var menu = new ContentPage { Title = "Menu", Content = host };
        var flyout = new FlyoutPage();
        var container = Mock.Of<IContainerProvider>();

        if (scopeBeforeParenting)
            flyout.SetContainerProvider(container);

        Assert.Equal(0, callbackCount);
        flyout.Flyout = menu;

        if (!scopeBeforeParenting)
        {
            Assert.Equal(0, callbackCount);
            flyout.SetContainerProvider(container);
        }

        Assert.Equal(1, callbackCount);
        Assert.Same(container, host.GetContainerProvider());

        flyout.SetContainerProvider(Mock.Of<IContainerProvider>());
        menu.Title = "Updated menu";
        Assert.Equal(1, callbackCount);
    }

    [Fact]
    public void FlyoutMenu_AttachedAfterScopeIsAvailable_InvokesCallbackOnce()
    {
        var host = new ContentView();
        var flyout = new FlyoutPage
        {
            Flyout = new ContentPage { Title = "Menu", Content = host }
        };
        flyout.SetContainerProvider(Mock.Of<IContainerProvider>());
        var callbackCount = 0;

        host.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++));

        Assert.Equal(1, callbackCount);
        Assert.Same(flyout.GetContainerProvider(), host.GetContainerProvider());
    }

    [Fact]
    public void DetailPage_WaitsForItsOwnScope()
    {
        var host = new ContentView();
        var callbackCount = 0;
        host.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++));
        var detail = new ContentPage { Content = host };
        var flyout = new FlyoutPage { Detail = detail };

        flyout.SetContainerProvider(Mock.Of<IContainerProvider>());

        Assert.Equal(0, callbackCount);
        var container = Mock.Of<IContainerProvider>();
        detail.SetContainerProvider(container);
        Assert.Equal(1, callbackCount);
        Assert.Same(container, host.GetContainerProvider());
    }

    [Fact]
    public void NestedHost_TracksAncestorsUntilPageIsParented()
    {
        var host = new ContentView();
        var callbackCount = 0;
        host.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++));
        var inner = new VerticalStackLayout { Children = { host } };
        var outer = new VerticalStackLayout { Children = { inner } };
        var page = new ContentPage();
        page.SetContainerProvider(Mock.Of<IContainerProvider>());

        page.Content = outer;

        Assert.Equal(1, callbackCount);
    }

    [Fact]
    public void PendingHost_ReparentedToAnotherMenu_UsesNewFlyoutScope()
    {
        var host = new ContentView();
        var callbackCount = 0;
        host.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++));
        var oldMenu = new ContentPage { Title = "Old menu", Content = host };
        var oldFlyout = new FlyoutPage { Flyout = oldMenu };
        oldMenu.Content = null;
        var newMenu = new ContentPage { Title = "New menu", Content = host };
        var newFlyout = new FlyoutPage { Flyout = newMenu };

        oldFlyout.SetContainerProvider(Mock.Of<IContainerProvider>());
        Assert.Equal(0, callbackCount);
        var container = Mock.Of<IContainerProvider>();
        newFlyout.SetContainerProvider(container);

        Assert.Equal(1, callbackCount);
        Assert.Same(container, host.GetContainerProvider());
    }

    [Fact]
    public void DetachedBehavior_DoesNotInvokeCallbackWhenScopeArrives()
    {
        var host = new ContentView();
        var callbackCount = 0;
        var behavior = new ElementParentedCallbackBehavior(() => callbackCount++);
        host.Behaviors.Add(behavior);
        var menu = new ContentPage { Title = "Menu", Content = host };
        var flyout = new FlyoutPage { Flyout = menu };

        host.Behaviors.Remove(behavior);
        flyout.SetContainerProvider(Mock.Of<IContainerProvider>());
        menu.SetContainerProvider(Mock.Of<IContainerProvider>());

        Assert.Equal(0, callbackCount);
    }

    [Fact]
    public void PageHost_UsesItsOwnScopeWhenIncluded()
    {
        var page = new ContentPage();
        var callbackCount = 0;
        page.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++, includeSelf: true));
        var navigationPage = new NavigationPage(page);
        navigationPage.SetContainerProvider(Mock.Of<IContainerProvider>());

        Assert.Equal(0, callbackCount);
        var container = Mock.Of<IContainerProvider>();
        page.SetContainerProvider(container);

        Assert.Equal(1, callbackCount);
        Assert.Same(container, page.GetContainerProvider());
    }

    [Fact]
    public void PageHost_WaitsForParentPageScopeWhenNotIncluded()
    {
        var page = new ContentPage();
        var callbackCount = 0;
        page.Behaviors.Add(new ElementParentedCallbackBehavior(() => callbackCount++));
        var navigationPage = new NavigationPage(page);

        page.SetContainerProvider(Mock.Of<IContainerProvider>());
        Assert.Equal(0, callbackCount);
        navigationPage.SetContainerProvider(Mock.Of<IContainerProvider>());

        Assert.Equal(1, callbackCount);
    }
}
