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
    public void UnattachedScopeDoesNotReadAnotherWindow()
    {
        _app.MainPage = Named(new ContentPage(), "OtherWindow");
        Assert.Equal(string.Empty, ServiceFor(new ContentPage()).GetNavigationUriPath());
        var ownPage = Named(new ContentPage(), "OwnWindow");
        var ownWindow = new PrismWindow { Page = ownPage };
        Assert.Equal("/OwnWindow", ServiceFor(ownPage).GetNavigationUriPath());
        GC.KeepAlive(ownWindow);
        Assert.Throws<ArgumentNullException>(() => INavigationServiceExtensions.GetNavigationUriPath(null));
        Assert.Throws<NotSupportedException>(() => new Mock<INavigationService>().Object.GetNavigationUriPath());
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
