using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Moq;
using Prism.Common;
using Prism.Ioc;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Maui.Tests.Navigation;
using Prism.Navigation;
using Xunit.Abstractions;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class RelativeModalNavigationFailureTests : IDisposable
{
    private readonly PageNavigationContainerMock _container = new();
    private readonly ApplicationMock _app = new();
    private readonly ITestOutputHelper _output;

    public RelativeModalNavigationFailureTests(ITestOutputHelper output)
    {
        _output = output;
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        _ = MauiApp.CreateBuilder().UseMauiApp<Application>().Build();
        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(_container);
        _container.RegisterForNavigation<RelativeNavigationPageMock>("Next");
        _container.RegisterForNavigation<TabbedPage>("TabbedPage");
        _container.RegisterForNavigation<ThrowingRelativePageMock>("ThrowingPage");
    }

    [Theory]
    [InlineData("../TabbedPage?createTab=Next&selectedTab=Missing", true)]
    [InlineData("../Next?throwOnInitialize=true", true)]
    [InlineData("../ThrowingPage", true)]
    [InlineData("../Next?useModalNavigation=false", false)]
    public async Task FailedDestination_MustKeepOriginalModal(string route, bool navigationRoot)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = navigationRoot ? new NavigationPage(root) : root;
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(modal).NavigateAsync(route);

        _output.WriteLine($"Success={result.Success}; ModalCount={_app.Window.Navigation.ModalStack.Count}; Destroyed={modal.Destroyed}; From={modal.NavigatedFromCount}; Error={result.Exception?.Message}");
        Assert.False(result.Success);
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Equal(0, modal.Destroyed);
        Assert.Equal(0, modal.NavigatedFromCount);
    }

    [Theory]
    [InlineData("navigation")]
    [InlineData("tabbed")]
    [InlineData("flyout")]
    public async Task InitiatingContainer_MustBeAbleToVeto(string kind)
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var leaf = new RelativeNavigationPageMock { BindingContext = new object() };
        var container = MakeContainer(kind, leaf);
        var veto = new Mock<IConfirmNavigationAsync>();
        veto.Setup(x => x.CanNavigateAsync(It.IsAny<INavigationParameters>())).ReturnsAsync(false);
        container.BindingContext = veto.Object;
        await _app.Window.Navigation.PushModalAsync(container);

        var result = await ServiceFor(container).NavigateAsync("../");

        _output.WriteLine($"Success={result.Success}; ContainerConfirmations={veto.Invocations.Count}; LeafConfirmations={leaf.Confirmations}; ModalCount={_app.Window.Navigation.ModalStack.Count}");
        Assert.False(result.Success);
        veto.Verify(x => x.CanNavigateAsync(It.IsAny<INavigationParameters>()), Times.Once);
        Assert.Same(container, Assert.Single(_app.Window.Navigation.ModalStack));
    }

    [Theory]
    [InlineData("navigation")]
    [InlineData("tabbed")]
    [InlineData("flyout")]
    public async Task RemovedContainer_MustReceiveDepartureOnce(string kind)
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var leaf = new RelativeNavigationPageMock { BindingContext = new object() };
        var container = MakeContainer(kind, leaf);
        var aware = new Mock<INavigationAware>();
        container.BindingContext = aware.Object;
        await _app.Window.Navigation.PushModalAsync(container);

        var result = await ServiceFor(leaf).NavigateAsync("../");

        _output.WriteLine($"Success={result.Success}; ContainerDepartures={aware.Invocations.Count}; LeafDepartures={leaf.NavigatedFromCount}; Destroyed={leaf.Destroyed}");
        Assert.True(result.Success, result.Exception?.ToString());
        aware.Verify(x => x.OnNavigatedFrom(It.IsAny<INavigationParameters>()), Times.Once);
        Assert.Equal(1, leaf.NavigatedFromCount);
    }

    [Fact]
    public async Task FailedMixedRoute_RestoresEveryOriginalPage()
    {
        var home = new RelativeNavigationPageMock();
        var settings = new RelativeNavigationPageMock();
        var mainNavigation = new NavigationPage(home);
        await mainNavigation.PushAsync(settings);
        _app.MainPage = mainNavigation;
        var modalRoot = new RelativeNavigationPageMock();
        var modalTop = new RelativeNavigationPageMock();
        var modalNavigation = new NavigationPage(modalRoot);
        await modalNavigation.PushAsync(modalTop);
        await _app.Window.Navigation.PushModalAsync(modalNavigation);
        var current = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(current);

        var result = await ServiceFor(current).NavigateAsync("../../../../Next?throwOnInitialize=true");

        Assert.False(result.Success);
        Assert.Equal(new Page[] { home, settings }, mainNavigation.Navigation.NavigationStack);
        Assert.Equal(new Page[] { modalNavigation, current }, _app.Window.Navigation.ModalStack);
        Assert.Equal(new Page[] { modalRoot, modalTop }, modalNavigation.Navigation.NavigationStack);
        foreach (var page in new[] { home, settings, modalRoot, modalTop, current })
        {
            Assert.Equal(0, page.Destroyed);
            Assert.Equal(0, page.NavigatedFromCount);
            Assert.Equal(0, page.NavigatedToCount);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FailedMultiSegmentRoute_DestroysNewPagesWithoutPublishingTheirCallbacks(bool navigationRoot)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = navigationRoot ? new NavigationPage(root) : root;
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);
        var initialized = new List<RelativeNavigationPageMock>();
        var parameters = new NavigationParameters
        {
            { "initializeObserver", (Action<RelativeNavigationPageMock>)initialized.Add }
        };
        var service = ServiceFor(modal);

        var result = await service.NavigateAsync("../Next?throwOnInitialize=true/Next", parameters);

        Assert.False(result.Success);
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        if (navigationRoot)
            Assert.Same(root, Assert.Single(_app.MainPage.Navigation.NavigationStack));
        Assert.Equal(2, initialized.Count);
        Assert.All(initialized, page =>
        {
            Assert.Equal(1, page.Destroyed);
            Assert.Equal(0, page.NavigatedFromCount);
            Assert.Equal(0, page.NavigatedToCount);
        });
        Assert.Equal(0, modal.Destroyed);
        Assert.Equal(0, modal.NavigatedFromCount);
        Assert.True((await service.NavigateAsync("../Next")).Success);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PushFailureAfterMutation_RestoresOriginalStack(bool navigationRoot)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = navigationRoot ? new NavigationPage(root) : root;
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);
        var initialized = new List<RelativeNavigationPageMock>();
        var parameters = new NavigationParameters
        {
            { "initializeObserver", (Action<RelativeNavigationPageMock>)initialized.Add }
        };
        var service = new FailingPageNavigationServiceMock(_container, _app);
        ((IPageAware)service).Page = modal;
        service.FailNextPush = true;

        var result = await service.NavigateAsync("../Next", parameters);

        Assert.False(result.Success);
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        if (navigationRoot)
            Assert.Same(root, Assert.Single(_app.MainPage.Navigation.NavigationStack));
        var destination = Assert.Single(initialized);
        Assert.Equal(1, destination.Destroyed);
        Assert.Equal(0, destination.NavigatedToCount);
        Assert.Equal(0, modal.Destroyed);
        Assert.Equal(0, modal.NavigatedFromCount);
    }

    [Theory]
    [InlineData("navigation")]
    [InlineData("tabbed")]
    [InlineData("flyout")]
    public async Task SharedContainerAndLeafViewModel_ReceivesOneDeparture(string kind)
    {
        _app.MainPage = new RelativeNavigationPageMock();
        var leaf = new RelativeNavigationPageMock();
        var container = MakeContainer(kind, leaf);
        var aware = new Mock<INavigationAware>();
        container.BindingContext = aware.Object;
        await _app.Window.Navigation.PushModalAsync(container);

        var result = await ServiceFor(leaf).NavigateAsync("../");

        Assert.True(result.Success, result.Exception?.ToString());
        aware.Verify(x => x.OnNavigatedFrom(It.IsAny<INavigationParameters>()), Times.Once);
        Assert.Equal(1, leaf.NavigatedFromCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PartialPopFailure_RestoresEntireModalHistory(bool afterMutation)
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var first = new RelativeNavigationPageMock();
        var second = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(first);
        await _app.Window.Navigation.PushModalAsync(second);
        var service = new FailingPageNavigationServiceMock(_container, _app)
        {
            FailPopCall = 2,
            ThrowAfterPop = afterMutation
        };
        ((IPageAware)service).Page = second;

        var result = await service.NavigateAsync("../../Next");

        Assert.False(result.Success);
        Assert.Equal(new Page[] { first, second }, _app.Window.Navigation.ModalStack);
        Assert.Equal(0, first.Destroyed);
        Assert.Equal(0, second.Destroyed);
        Assert.Equal(0, first.NavigatedFromCount);
        Assert.Equal(0, second.NavigatedFromCount);
        Assert.True((await service.NavigateAsync("../../Next")).Success);
    }

    [Fact]
    public async Task PartialNavigationPopFailure_RestoresModalContainerAndItsChildren()
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var root = new RelativeNavigationPageMock();
        var top = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(root);
        await navigation.PushAsync(top);
        await _app.Window.Navigation.PushModalAsync(navigation);
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);
        var service = new FailingPageNavigationServiceMock(_container, _app)
        {
            FailPopCall = 2,
            ThrowAfterPop = true
        };
        ((IPageAware)service).Page = modal;

        var result = await service.NavigateAsync("../../../Next");

        Assert.False(result.Success);
        Assert.Equal(new Page[] { navigation, modal }, _app.Window.Navigation.ModalStack);
        Assert.Equal(new Page[] { root, top }, navigation.Navigation.NavigationStack);
        Assert.All(new[] { root, top, modal }, page => Assert.Equal(0, page.Destroyed));
    }

    [Fact]
    public async Task StackChangedDuringConfirmation_IsNotOverwritten()
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var modal = new RelativeNavigationPageMock
        {
            Confirmation = new(TaskCreationOptions.RunContinuationsAsynchronously)
        };
        await _app.Window.Navigation.PushModalAsync(modal);
        var request = ServiceFor(modal).NavigateAsync("../Next");
        await modal.ConfirmationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var laterModal = new ContentPage();
        await _app.Window.Navigation.PushModalAsync(laterModal);
        modal.Confirmation.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        Assert.Equal(new Page[] { modal, laterModal }, _app.Window.Navigation.ModalStack);
        Assert.Equal(0, modal.Destroyed);
        Assert.Equal(0, modal.NavigatedFromCount);
        Assert.Null(_app.Window.PendingModalConfirmation);
    }

    [Fact]
    public async Task RemovedNestedContainers_NotifyTheActiveBranchOnce()
    {
        _app.MainPage = new RelativeNavigationPageMock();
        var leaf = new RelativeNavigationPageMock { BindingContext = new object() };
        var inner = new NavigationPage(leaf);
        var innerAware = new Mock<INavigationAware>();
        inner.BindingContext = innerAware.Object;
        var inactive = new RelativeNavigationPageMock();
        var outer = new TabbedPage { Children = { inner, inactive }, CurrentPage = inner };
        var outerAware = new Mock<INavigationAware>();
        outer.BindingContext = outerAware.Object;
        await _app.Window.Navigation.PushModalAsync(outer);

        var result = await ServiceFor(leaf).NavigateAsync("../");

        Assert.True(result.Success, result.Exception?.ToString());
        outerAware.Verify(x => x.OnNavigatedFrom(It.IsAny<INavigationParameters>()), Times.Once);
        innerAware.Verify(x => x.OnNavigatedFrom(It.IsAny<INavigationParameters>()), Times.Once);
        Assert.Equal(1, leaf.NavigatedFromCount);
        Assert.Equal(0, inactive.NavigatedFromCount);
    }

    private static Page MakeContainer(string kind, Page leaf) => kind switch
    {
        "navigation" => new NavigationPage(leaf),
        "tabbed" => new TabbedPage { Children = { leaf } },
        "flyout" => new FlyoutPage { Flyout = new ContentPage { Title = "Menu" }, Detail = leaf },
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private PageNavigationServiceMock ServiceFor(Page page)
    {
        var service = new PageNavigationServiceMock(_container, _app);
        ((IPageAware)service).Page = page;
        return service;
    }

    public void Dispose()
    {
        ContainerLocator.ResetContainer();
        PageNavigationService.NavigationSource = PageNavigationSource.Device;
    }
}
