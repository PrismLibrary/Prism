using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Moq;
using Prism.Ioc;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Navigation;
using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class NavigationUriPathFixture : IDisposable
{
    private readonly PageNavigationContainerMock _container = new();
    private readonly ApplicationMock _app = new();

    public NavigationUriPathFixture()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        _ = MauiApp.CreateBuilder().UseMauiApp<Application>().Build();
        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(_container);
    }

    [Fact]
    public void ViewModelCanReadPathFromInterfaceTestDouble()
    {
        var service = new Mock<INavigationService>(MockBehavior.Strict);
        service.Setup(navigation => navigation.GetNavigationUriPath()).Returns("/NavigationPage/Login");
        var model = new Prism.Maui.Tests.Navigation.Mocks.ViewModels.NavigationPathPageMockViewModel(service.Object);

        Assert.Equal("/NavigationPage/Login", model.NavigationService.GetNavigationUriPath());
        service.Verify(navigation => navigation.GetNavigationUriPath(), Times.Once);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PathStopsAtScopedPageAndReflectsBackStackChanges()
    {
        var welcome = Named(new ContentPage(), "Welcome");
        var login = Named(new ContentPage(), "SignIn");
        var navigation = Named(new NavigationPage(welcome), "Nav");
        _app.MainPage = navigation;
        await navigation.PushAsync(login);
        Assert.Equal("/Nav/Welcome", ServiceFor(welcome).GetNavigationUriPath());
        Assert.Equal("/Nav/Welcome/SignIn", ServiceFor(login).GetNavigationUriPath());
        Assert.Equal("/Nav/Welcome/SignIn", ServiceFor(null).GetNavigationUriPath());
        await navigation.PopAsync();
        Assert.Equal("/Nav/Welcome", ServiceFor(null).GetNavigationUriPath());
        Assert.Equal(string.Empty, ServiceFor(login).GetNavigationUriPath());
    }

    [Fact]
    public async Task ModalPathIncludesUnderlyingStackAndModalBoundary()
    {
        var root = Named(new ContentPage(), "Home");
        _app.MainPage = root;
        var first = Named(new ContentPage(), "Busy");
        var login = Named(new ContentPage(), "Login");
        var modal = Named(new NavigationPage(login), "ModalNav");
        await _app.Window.Navigation.PushModalAsync(first);
        await _app.Window.Navigation.PushModalAsync(modal);
        Assert.Equal("/Home/Busy?useModalNavigation=true/ModalNav?useModalNavigation=true/Login",
            ServiceFor(login).GetNavigationUriPath());
        Assert.Equal("/Home", ServiceFor(root).GetNavigationUriPath());
        await _app.Window.Navigation.PopModalAsync();
        Assert.Equal("/Home/Busy?useModalNavigation=true", ServiceFor(null).GetNavigationUriPath());
    }

    [Fact]
    public async Task FlyoutAndInactiveTabUseTheScopedBranchAndAliases()
    {
        var login = Named(new ContentPage(), "Login");
        var privacy = Named(new ContentPage(), "Privacy");
        var navigation = Named(new NavigationPage(privacy), "Nav");
        await navigation.PushAsync(login);
        var home = Named(new ContentPage(), "Home");
        var tabs = Named(new TabbedPage(), "Tabs");
        tabs.Children.Add(home);
        tabs.Children.Add(navigation);
        tabs.CurrentPage = home;
        _app.MainPage = Named(new FlyoutPage { Flyout = new ContentPage { Title = "Menu" }, Detail = tabs }, "Shell");
        Assert.Equal("/Shell/Tabs?selectedTab=Nav%7CPrivacy/Login", ServiceFor(login).GetNavigationUriPath());
        Assert.Equal("/Shell/Tabs?selectedTab=Home", ServiceFor(null).GetNavigationUriPath());
        tabs.CurrentPage = navigation;
        Assert.Equal(ServiceFor(login).GetNavigationUriPath(), ServiceFor(null).GetNavigationUriPath());
    }

    [Fact]
    public async Task FlyoutTabDescendantsAndNavigationTabContainerRootsRemainInPath()
    {
        var leaf = Named(new ContentPage(), "Leaf");
        var start = Named(new ContentPage(), "Start");
        var detailNavigation = Named(new NavigationPage(start), "DetailStack");
        await detailNavigation.PushAsync(leaf);
        var flyout = Named(new FlyoutPage
        {
            Flyout = Named(new ContentPage { Title = "Menu" }, "MenuOnly"),
            Detail = detailNavigation
        }, "FlyoutTab");
        var tabs = Named(new TabbedPage(), "OuterTabs");
        tabs.Children.Add(flyout);
        _app.MainPage = tabs;
        Assert.Equal("/OuterTabs?selectedTab=FlyoutTab/DetailStack/Start/Leaf", ServiceFor(leaf).GetNavigationUriPath());
        Assert.Equal(string.Empty, ServiceFor(flyout.Flyout).GetNavigationUriPath());

        tabs.Children.Clear();
        var tabNavigation = Named(new NavigationPage(flyout), "TabStack");
        tabs.Children.Add(tabNavigation);
        Assert.Equal("/OuterTabs?selectedTab=TabStack%7CFlyoutTab/DetailStack/Start/Leaf", ServiceFor(leaf).GetNavigationUriPath());
    }

    [Fact]
    public async Task NestedTabbedSelectionUnderNavigationTabRootAndModalsUsesEachBranch()
    {
        var leaf = Named(new ContentPage(), "DeepLeaf");
        var innerStack = Named(new NavigationPage(leaf), "InnerStack");
        await innerStack.PushAsync(Named(new ContentPage(), "End"));
        var innerTabs = Named(new TabbedPage(), "InnerTabs");
        innerTabs.Children.Add(Named(new ContentPage(), "Inactive"));
        innerTabs.Children.Add(innerStack);
        innerTabs.CurrentPage = innerStack;
        var outerStack = Named(new NavigationPage(innerTabs), "OuterStack");
        var outerTabs = Named(new TabbedPage(), "OuterTabs");
        outerTabs.Children.Add(outerStack);
        _app.MainPage = outerTabs;
        Assert.Equal("/OuterTabs?selectedTab=OuterStack%7CInnerTabs", ServiceFor(innerTabs).GetNavigationUriPath());
        var path = "/OuterTabs?selectedTab=OuterStack%7CInnerTabs/InnerTabs?selectedTab=InnerStack%7CDeepLeaf/End";
        Assert.Equal(path, ServiceFor(innerStack.CurrentPage).GetNavigationUriPath());
        var modal = Named(new ContentPage(), "TopModal");
        await _app.Window.Navigation.PushModalAsync(modal);
        Assert.Equal(path + "/TopModal?useModalNavigation=true", ServiceFor(modal).GetNavigationUriPath());
        Assert.Equal(path, ServiceFor(innerStack.CurrentPage).GetNavigationUriPath());
    }

    [Fact]
    public void NestedTabContainerScopeStopsAtItsOuterSelection()
    {
        var leaf = Named(new ContentPage(), "Leaf");
        var inner = Named(new TabbedPage(), "Inner");
        inner.Children.Add(leaf);
        var outer = Named(new TabbedPage(), "Outer");
        outer.Children.Add(inner);
        _app.MainPage = outer;
        Assert.Equal("/Outer?selectedTab=Inner", ServiceFor(inner).GetNavigationUriPath());
        Assert.Equal("/Outer?selectedTab=Inner/Inner?selectedTab=Leaf", ServiceFor(leaf).GetNavigationUriPath());
    }

    [Fact]
    public void SelectedTabEscapesNamesWithoutRetainingUserParameters()
    {
        var root = Named(new ContentPage(), "Root & More");
        var stack = Named(new NavigationPage(root), "Nav + More");
        var tabs = Named(new TabbedPage(), "Tabs");
        tabs.Children.Add(stack);
        _app.MainPage = tabs;
        Assert.Equal("/Tabs?selectedTab=Nav%20%2B%20More%7CRoot%20%26%20More", ServiceFor(root).GetNavigationUriPath());
    }

    [Fact]
    public async Task EachWindowKeepsItsOwnNavigationAndModalStackForScopedAndUnscopedReads()
    {
        var firstRoot = Named(new ContentPage(), "FirstRoot");
        var secondRoot = Named(new ContentPage(), "SecondRoot");
        _app.MainPage = Named(new NavigationPage(firstRoot), "FirstStack");
        var second = new PrismWindow { Page = Named(new NavigationPage(secondRoot), "SecondStack") };
        var firstModal = Named(new ContentPage(), "FirstModal");
        var secondModal = Named(new NavigationPage(Named(new ContentPage(), "SecondModalRoot")), "SecondModalStack");
        await _app.Window.Navigation.PushModalAsync(firstModal);
        await second.Navigation.PushModalAsync(secondModal);
        await secondModal.PushAsync(Named(new ContentPage(), "SecondModalEnd"));
        var secondService = ServiceFor(secondModal.CurrentPage);
        Assert.Equal("/FirstStack/FirstRoot", ServiceFor(firstRoot).GetNavigationUriPath());
        Assert.Equal("/SecondStack/SecondRoot", ServiceFor(secondRoot).GetNavigationUriPath());
        Assert.Equal("/FirstStack/FirstRoot/FirstModal?useModalNavigation=true", ServiceFor(firstModal).GetNavigationUriPath());
        Assert.Equal("/FirstStack/FirstRoot/FirstModal?useModalNavigation=true", ServiceFor(null).GetNavigationUriPath());
        Assert.Equal("/SecondStack/SecondRoot/SecondModalStack?useModalNavigation=true/SecondModalRoot/SecondModalEnd", secondService.GetNavigationUriPath());
        await second.Navigation.PopModalAsync();
        Assert.Equal(string.Empty, secondService.GetNavigationUriPath());
        Assert.Equal("/FirstStack/FirstRoot/FirstModal?useModalNavigation=true", ServiceFor(null).GetNavigationUriPath());
    }

    [Fact]
    public void UnattachedScopeDoesNotReadAnotherWindow()
    {
        _app.MainPage = Named(new ContentPage(), "OtherWindow");
        Assert.Equal(string.Empty, ServiceFor(new ContentPage()).GetNavigationUriPath());
        var ownPage = Named(new ContentPage(), "OwnWindow");
        var ownWindow = new PrismWindow { Page = ownPage };
        Assert.Equal("/OwnWindow", ServiceFor(ownPage).GetNavigationUriPath());
        GC.KeepAlive(ownWindow);
    }

    private INavigationService ServiceFor(Page page)
    {
        var service = new PageNavigationServiceMock(_container, _app);
        ((IPageAware)service).Page = page;
        return service;
    }

    private static T Named<T>(T page, string name) where T : Page
    {
        ViewModelLocator.SetNavigationName(page, name);
        return page;
    }

    public void Dispose() => ContainerLocator.ResetContainer();
}
