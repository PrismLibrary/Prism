using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Prism.Ioc;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Maui.Tests.Navigation;
using Prism.Navigation;
using Xunit;
using NavigationMode = Prism.Navigation.NavigationMode;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class RelativeModalNavigationFixture : IDisposable
{
    private readonly PageNavigationContainerMock _container = new();
    private readonly ApplicationMock _app = new();

    public RelativeModalNavigationFixture()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        _ = MauiApp.CreateBuilder().UseMauiApp<Application>().Build();
        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(_container);
        _container.RegisterForNavigation<RelativeNavigationPageMock>("Next");
        _container.RegisterForNavigation<NavigationPageEmptyMock>("NavigationPage");
    }

    [Fact]
    public async Task RemoveAndPush_CrossesTwoModalsIntoUnderlyingNavigationPage()
    {
        var home = new RelativeNavigationPageMock();
        var settings = new RelativeNavigationPageMock { CanNavigate = false };
        var navigationPage = new NavigationPage(home);
        await navigationPage.PushAsync(settings);
        _app.MainPage = navigationPage;
        var setup = await PushModal();
        var confirmation = await PushModal();
        var parameters = new NavigationParameters { { "device", 42 } };

        var result = await ServiceFor(confirmation).NavigateAsync("../../Next?source=setup", parameters);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        Assert.Equal(3, navigationPage.Navigation.NavigationStack.Count);
        Assert.Same(home, navigationPage.RootPage);
        var next = Assert.IsType<RelativeNavigationPageMock>(navigationPage.CurrentPage);
        Assert.NotSame(settings, next);
        Assert.Equal(1, next.Initialized);
        Assert.Equal(1, next.NavigatedToCount);
        Assert.Equal(42, next.ToParameters.GetValue<int>("device"));
        Assert.Equal("setup", next.ToParameters.GetValue<string>("source"));
        Assert.Equal(NavigationMode.New, next.ToParameters.GetNavigationMode());
        AssertRemoved(setup, NavigationMode.New);
        AssertRemoved(confirmation, NavigationMode.New);
        Assert.Equal(1, confirmation.Confirmations);
        Assert.Equal(0, setup.Confirmations);
        AssertRetained(settings);
    }

    [Fact]
    public async Task RemoveAndPush_UsesModalNavigationAboveContentRoot()
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = root;
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        var next = Assert.IsType<RelativeNavigationPageMock>(Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.NotSame(modal, next);
        Assert.Equal(1, next.NavigatedToCount);
        AssertRemoved(modal, NavigationMode.New);
        AssertRetained(root);
    }

    [Fact]
    public async Task RemoveAndBack_NotifiesOnlyFinalDestination()
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = new NavigationPage(root);
        var first = await PushModal();
        var last = await PushModal();
        var parameters = new NavigationParameters { { "device", 42 } };

        var result = await ServiceFor(last).NavigateAsync("../../", parameters);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        AssertRemoved(last, NavigationMode.Back);
        AssertRemoved(first, NavigationMode.Back);
        Assert.Equal(0, first.NavigatedToCount);
        Assert.Equal(1, root.NavigatedToCount);
        Assert.Equal(42, root.ToParameters.GetValue<int>("device"));
        Assert.Equal(NavigationMode.Back, root.ToParameters.GetNavigationMode());
        Assert.Equal(0, root.Initialized);
        Assert.Equal(0, root.Destroyed);
    }

    [Fact]
    public async Task RemoveAndPush_RetainsEarlierModalNavigationPage()
    {
        _app.MainPage = new RelativeNavigationPageMock();
        var retained = new RelativeNavigationPageMock();
        var modalNavigation = new NavigationPage(retained);
        await _app.Window.Navigation.PushModalAsync(modalNavigation);
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(modalNavigation, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Equal(2, modalNavigation.Navigation.NavigationStack.Count);
        Assert.NotSame(retained, modalNavigation.CurrentPage);
        AssertRetained(retained);
        AssertRemoved(modal, NavigationMode.New);
    }

    [Theory]
    [InlineData("../../", false)]
    [InlineData("../../Next", true)]
    public async Task RelativeNavigation_CrossesModalNavigationRoot(string route, bool forward)
    {
        var root = new RelativeNavigationPageMock();
        var mainNavigation = new NavigationPage(root);
        _app.MainPage = mainNavigation;
        var modalRoot = new RelativeNavigationPageMock();
        var modalNavigation = new NavigationPageMock(null, modalRoot);
        var modalTop = new RelativeNavigationPageMock();
        await modalNavigation.PushAsync(modalTop);
        await _app.Window.Navigation.PushModalAsync(modalNavigation);

        var result = await ServiceFor(modalTop).NavigateAsync(route);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        Assert.True(modalNavigation.DestroyCalled);
        AssertRemoved(modalTop, forward ? NavigationMode.New : NavigationMode.Back);
        AssertRemoved(modalRoot, forward ? NavigationMode.New : NavigationMode.Back);
        Assert.Equal(forward ? 2 : 1, mainNavigation.Navigation.NavigationStack.Count);
        Assert.Equal(forward ? 0 : 1, root.NavigatedToCount);
    }

    [Fact]
    public async Task RemoveAndPush_CrossesMixedModalAndNavigationEntries()
    {
        var root = new RelativeNavigationPageMock();
        var removed = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(root);
        await navigation.PushAsync(removed);
        _app.MainPage = navigation;
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync("../../Next/Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        Assert.Equal(3, navigation.Navigation.NavigationStack.Count);
        Assert.Same(root, navigation.RootPage);
        AssertRemoved(modal, NavigationMode.New);
        AssertRemoved(removed, NavigationMode.New);
        AssertRetained(root);
    }

    [Fact]
    public async Task RemoveAndPush_RespectsExplicitModalDestination()
    {
        var root = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(root);
        _app.MainPage = navigation;
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync($"../Next?{KnownNavigationParameters.UseModalNavigation}=true");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Single(navigation.Navigation.NavigationStack);
        Assert.IsType<RelativeNavigationPageMock>(Assert.Single(_app.Window.Navigation.ModalStack));
        AssertRemoved(modal, NavigationMode.New);
    }

    [Theory]
    [InlineData("../../")]
    [InlineData("../../Next")]
    public async Task ConfirmationVeto_LeavesAllStacksAndLifecyclesUntouched(string route)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = new NavigationPage(root);
        var first = await PushModal();
        var last = await PushModal();
        last.CanNavigate = false;

        var result = await ServiceFor(last).NavigateAsync(route);

        Assert.False(result.Success);
        Assert.Equal(NavigationException.IConfirmNavigationReturnedFalse, result.Exception.Message);
        Assert.Equal(new Page[] { first, last }, _app.Window.Navigation.ModalStack);
        AssertRetained(root);
        AssertRetained(first);
        Assert.Equal(1, last.Confirmations);
        Assert.Equal(0, last.NavigatedFromCount);
        Assert.Equal(0, last.Destroyed);
        Assert.Null(_app.Window.PendingModalConfirmation);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PendingConfirmation_CoalescesDeviceModalBack(bool allow)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = new NavigationPage(root);
        var modal = await PushModal();
        modal.Confirmation = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var navigation = ServiceFor(modal).NavigateAsync("../Next");
        await modal.ConfirmationStarted.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Same(modal, _app.Window.PendingModalConfirmation);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        await _app.Window.Navigation.PopModalAsync();
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Equal(0, modal.Destroyed);

        modal.Confirmation.SetResult(allow);
        var result = await navigation;

        Assert.Equal(allow, result.Success);
        Assert.Equal(1, modal.Confirmations);
        Assert.Equal(allow ? 1 : 0, modal.Destroyed);
        Assert.Null(_app.Window.PendingModalConfirmation);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
    }

    [Theory]
    [InlineData("../../../")]
    [InlineData("../../../Next")]
    [InlineData("../Unregistered")]
    public async Task InvalidRoute_DoesNotMutateStacks(string route)
    {
        _app.MainPage = new RelativeNavigationPageMock();
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync(route);

        Assert.False(result.Success);
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        AssertRetained(modal);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RelativeNavigation_UsesSelectedContainerBranch(bool flyout)
    {
        var retained = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(retained);
        var inactive = new RelativeNavigationPageMock();
        _app.MainPage = flyout
            ? new FlyoutPage { Flyout = new ContentPage { Title = "Menu" }, Detail = navigation }
            : new TabbedPage { Children = { inactive, navigation }, CurrentPage = navigation };
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(2, navigation.Navigation.NavigationStack.Count);
        AssertRetained(retained);
        AssertRetained(inactive);
    }

    [Fact]
    public async Task RelativeNavigation_UsesOwningWindow()
    {
        _app.MainPage = new RelativeNavigationPageMock();
        var otherRoot = new RelativeNavigationPageMock();
        var otherWindow = new PrismWindow("other") { Page = new NavigationPage(otherRoot) };
        var modal = new RelativeNavigationPageMock();
        await otherWindow.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(modal).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(otherWindow.Navigation.ModalStack);
        Assert.Equal(2, otherWindow.Page.Navigation.NavigationStack.Count);
        AssertRetained(Assert.IsType<RelativeNavigationPageMock>(_app.MainPage));
    }

    [Fact]
    public async Task RemoveAndPush_CanReplaceUnderlyingNavigationRoot()
    {
        var root = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(root);
        _app.MainPage = navigation;
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync("../../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        Assert.NotSame(root, Assert.Single(navigation.Navigation.NavigationStack));
        AssertRemoved(root, NavigationMode.New);
        AssertRemoved(modal, NavigationMode.New);
    }

    [Theory]
    [InlineData("../../")]
    [InlineData("../../Next?useModalNavigation=true")]
    public async Task RelativeNavigation_CannotLeaveUnderlyingNavigationPageEmpty(string route)
    {
        var root = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(root);
        _app.MainPage = navigation;
        var modal = await PushModal();

        var result = await ServiceFor(modal).NavigateAsync(route);

        Assert.False(result.Success);
        Assert.Same(root, Assert.Single(navigation.Navigation.NavigationStack));
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        AssertRetained(root);
        AssertRetained(modal);
    }

    [Fact]
    public async Task RemoveAndBack_DestroysModalContainerAndInactiveChildrenOnce()
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = new NavigationPage(root);
        var active = new RelativeNavigationPageMock();
        var inactive = new RelativeNavigationPageMock();
        var tabs = new TabbedPage
        {
            Children = { inactive, new NavigationPage(active) }
        };
        tabs.CurrentPage = tabs.Children[1];
        await _app.Window.Navigation.PushModalAsync(tabs);

        var result = await ServiceFor(active).NavigateAsync("../");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        AssertRemoved(active, NavigationMode.Back);
        Assert.Equal(1, inactive.Destroyed);
        Assert.Equal(0, inactive.NavigatedFromCount);
        Assert.Equal(1, root.NavigatedToCount);
    }

    private PageNavigationServiceMock ServiceFor(Page page)
    {
        var service = new PageNavigationServiceMock(_container, _app);
        ((IPageAware)service).Page = page;
        return service;
    }

    private async Task<RelativeNavigationPageMock> PushModal()
    {
        var page = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(page);
        return page;
    }

    private static void AssertRemoved(RelativeNavigationPageMock page, NavigationMode mode)
    {
        Assert.Equal(1, page.NavigatedFromCount);
        Assert.Equal(1, page.Destroyed);
        Assert.Equal(0, page.NavigatedToCount);
        Assert.Equal(mode, page.FromParameters.GetNavigationMode());
    }

    private static void AssertRetained(RelativeNavigationPageMock page)
    {
        Assert.Equal(0, page.Confirmations);
        Assert.Equal(0, page.Initialized);
        Assert.Equal(0, page.NavigatedFromCount);
        Assert.Equal(0, page.NavigatedToCount);
        Assert.Equal(0, page.Destroyed);
    }

    public void Dispose()
    {
        ContainerLocator.ResetContainer();
        PageNavigationService.NavigationSource = PageNavigationSource.Device;
    }
}
