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
        _container.RegisterForNavigation<InitializingRelativeTabbedPageMock>("DynamicTabs");
        _container.RegisterForNavigation<InitializingRelativeNavigationPageMock>("AsyncNavigation");
        _container.RegisterForNavigation<RelativeFlyoutPageMock>("RelativeFlyout");
        _container.RegisterForNavigation<RelativeNavigationFlyoutPageMock>("NavigationFlyout");
        _container.RegisterForNavigation<ContentPage>("Other");
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

    [Theory]
    [InlineData("tabbed")]
    [InlineData("flyout")]
    public async Task ActiveBranchChangedDuringConfirmation_IsNotRemoved(string kind)
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var original = new RelativeNavigationPageMock
        {
            Confirmation = new(TaskCreationOptions.RunContinuationsAsynchronously)
        };
        var later = new RelativeNavigationPageMock { CanNavigate = false };
        var container = MakeContainer(kind, original);
        if (container is TabbedPage tabs)
            tabs.Children.Add(later);
        await _app.Window.Navigation.PushModalAsync(container);
        var request = ServiceFor(original).NavigateAsync("../Next");
        await original.ConfirmationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        if (container is TabbedPage tabbed)
            tabbed.CurrentPage = later;
        else
            ((FlyoutPage)container).Detail = later;
        original.Confirmation.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        Assert.Same(container, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Equal(0, original.Destroyed);
        Assert.Equal(0, later.Destroyed);
        Assert.Equal(0, original.NavigatedFromCount);
        Assert.Equal(0, later.NavigatedFromCount);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task Initialization_DoesNotOverwriteUnrelatedPagePushedWhileAwaiting(bool modalPush, bool failInitialization)
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        var initialized = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var finish = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var created = new List<RelativeNavigationPageMock>();
        var parameters = new NavigationParameters
        {
            { "initializeObserver", (Action<RelativeNavigationPageMock>)created.Add },
            { "initializeAsync", (Func<Task>)(async () => { initialized.SetResult(true); await finish.Task; }) }
        };
        var request = ServiceFor(original).NavigateAsync("../Next", parameters);
        await initialized.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var unrelated = new RelativeNavigationPageMock();
        if (modalPush)
            await _app.Window.Navigation.PushModalAsync(unrelated);
        else
            await _app.MainPage.Navigation.PushAsync(unrelated);
        if (failInitialization)
            finish.SetException(new InvalidOperationException("Destination initialization failed."));
        else
            finish.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        Assert.Same(unrelated, modalPush ? _app.Window.Navigation.ModalStack.Last() : _app.MainPage.Navigation.NavigationStack.Last());
        Assert.Equal(0, unrelated.Destroyed);
        Assert.Equal(0, unrelated.NavigatedFromCount);
        Assert.Equal(0, original.Destroyed);
        Assert.Equal(0, original.NavigatedFromCount);
        Assert.Equal(1, Assert.Single(created).Destroyed);
        Assert.IsType<AggregateException>(result.Exception.InnerException);
    }

    [Theory]
    [InlineData("navigation", false)]
    [InlineData("navigation", true)]
    [InlineData("tabbed", false)]
    public async Task EmptyRetainedContainer_RemainsTheNavigationHost(string kind, bool modalHost)
    {
        Page host = kind switch
        {
            "navigation" => new NavigationPage(),
            _ => new TabbedPage()
        };
        _app.MainPage = modalHost ? new RelativeNavigationPageMock() : host;
        var applicationRoot = _app.MainPage;
        if (modalHost)
            await _app.Window.Navigation.PushModalAsync(host);
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);

        var result = await ServiceFor(original).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(applicationRoot, _app.MainPage);
        if (modalHost)
            Assert.Same(host, Assert.Single(_app.Window.Navigation.ModalStack));
        if (host is NavigationPage navigation)
            Assert.IsType<RelativeNavigationPageMock>(Assert.Single(navigation.Navigation.NavigationStack));
        else
            Assert.IsType<RelativeNavigationPageMock>(Assert.Single(_app.Window.Navigation.ModalStack));
    }

    [Theory]
    [InlineData("navigation", "../")]
    [InlineData("navigation", "../Next")]
    [InlineData("tabbed", "../")]
    [InlineData("tabbed", "../Next")]
    public async Task EmptyInitiatingModal_IsOneRelativeBackStep(string kind, string route)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = new NavigationPage(root);
        Page modal = kind == "navigation" ? new NavigationPage() : new TabbedPage();
        await _app.Window.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(modal).NavigateAsync(route);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(_app.Window.Navigation.ModalStack);
        Assert.Equal(route == "../" ? 1 : 2, _app.MainPage.Navigation.NavigationStack.Count);
        Assert.Same(root, _app.MainPage.Navigation.NavigationStack[0]);
    }

    [Fact]
    public async Task FailedRouteIntoEmptyNavigation_RestoresTheEmptyHostAndModal()
    {
        var host = new NavigationPage();
        _app.MainPage = host;
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(modal).NavigateAsync("../Next?throwOnInitialize=true");

        Assert.False(result.Success);
        Assert.Same(host, _app.MainPage);
        Assert.Empty(host.Navigation.NavigationStack);
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Equal(0, modal.Destroyed);
    }

    [Fact]
    public async Task DestinationInitialization_CanComposeItsDetachedChildren()
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(modal).NavigateAsync("../DynamicTabs");

        Assert.True(result.Success, result.Exception?.ToString());
        var destination = Assert.IsType<InitializingRelativeTabbedPageMock>(_app.MainPage.Navigation.NavigationStack.Last());
        Assert.Equal(2, destination.Children.Count);
        Assert.Empty(_app.Window.Navigation.ModalStack);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RollbackPop_DoesNotRemoveForeignNavigationFromItsNativeCallback(bool navigationRoot)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = navigationRoot ? new NavigationPage(root) : root;
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        var foreign = new RelativeNavigationPageMock();
        var service = new FailingPageNavigationServiceMock(_container, _app)
        {
            FailNextPush = true,
            AfterPop = call => call == 2 ? _app.Window.Navigation.PushModalAsync(foreign) : Task.CompletedTask
        };
        ((IPageAware)service).Page = original;

        var result = await service.NavigateAsync("../Next");

        Assert.False(result.Success);
        Assert.Same(foreign, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Equal(0, foreign.Destroyed);
        Assert.Equal(0, foreign.NavigatedFromCount);
        Assert.Equal(0, original.Destroyed);
        Assert.IsType<AggregateException>(result.Exception.InnerException);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task Initialization_DoesNotUndoAnUnrelatedPop(bool failInitialization, bool createTab)
    {
        var root = new RelativeNavigationPageMock();
        var retained = new RelativeNavigationPageMock();
        var navigation = new NavigationPage(root);
        await navigation.PushAsync(retained);
        _app.MainPage = navigation;
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        var initialized = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var finish = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var parameters = new NavigationParameters
        {
            { "initializeAsync", (Func<Task>)(async () => { initialized.SetResult(true); await finish.Task; }) }
        };
        var request = ServiceFor(original).NavigateAsync(createTab ? "../TabbedPage?createTab=AsyncNavigation|Next" : "../Next", parameters);
        await initialized.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await navigation.PopAsync();
        if (failInitialization)
            finish.SetException(new InvalidOperationException("Destination initialization failed."));
        else
            finish.SetResult(true);

        var result = await request;

        Assert.False(result.Success);
        Assert.Same(root, Assert.Single(navigation.Navigation.NavigationStack));
        Assert.Empty(_app.Window.Navigation.ModalStack);
        Assert.Equal(0, original.Destroyed);
        Assert.Equal(0, retained.Destroyed);
    }

    [Fact]
    public async Task ConflictCleanupFailure_PreservesTheOriginalErrorsAndDestroysOnce()
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        var created = new List<RelativeNavigationPageMock>();
        var parameters = new NavigationParameters
        {
            { "initializeObserver", (Action<RelativeNavigationPageMock>)(page => { page.ThrowOnDestroy = true; created.Add(page); }) },
            { "initializeAsync", (Func<Task>)(async () =>
                {
                    await _app.Window.Navigation.PushModalAsync(new RelativeNavigationPageMock());
                    throw new InvalidOperationException("Original initialization failure.");
                }) }
        };

        var result = await ServiceFor(original).NavigateAsync("../Next", parameters);

        Assert.False(result.Success);
        var errors = Assert.IsType<AggregateException>(result.Exception.InnerException).Flatten().InnerExceptions;
        Assert.Contains(errors, error => error.Message == "Original initialization failure.");
        Assert.Contains(errors, error => error.InnerException?.Message == "Destination cleanup failed.");
        Assert.Equal(1, Assert.Single(created).Destroyed);
        Assert.Equal(0, original.Destroyed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NativePush_DoesNotCommitADestinationRemovedByItsCallback(bool navigationRoot)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = navigationRoot ? new NavigationPage(root) : root;
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        var created = new List<RelativeNavigationPageMock>();
        var parameters = new NavigationParameters
        {
            { "initializeObserver", (Action<RelativeNavigationPageMock>)created.Add }
        };
        var service = new FailingPageNavigationServiceMock(_container, _app)
        {
            AfterPush = async page =>
            {
                if (navigationRoot)
                    await page.Navigation.PopAsync();
                else
                    await _app.Window.Navigation.PopModalAsync();
            }
        };
        ((IPageAware)service).Page = original;

        var result = await service.NavigateAsync("../Next", parameters);

        Assert.False(result.Success);
        Assert.Empty(_app.Window.Navigation.ModalStack);
        if (navigationRoot)
            Assert.Same(root, Assert.Single(_app.MainPage.Navigation.NavigationStack));
        Assert.Equal(0, original.Destroyed);
        Assert.Equal(0, original.NavigatedFromCount);
        Assert.Equal(0, Assert.Single(created).NavigatedToCount);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task NewFlyoutDetail_DestructionFollowsCommittedDepartureAndRunsOnce(bool failInitialization, bool navigationDetail)
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        RelativeFlyoutPageMock flyout = null;
        var parameters = new NavigationParameters
        {
            { "flyoutObserver", (Action<RelativeFlyoutPageMock>)(page => flyout = page) },
            { "failFlyoutInitialize", failInitialization }
        };

        var result = await ServiceFor(original).NavigateAsync(navigationDetail ? "../NavigationFlyout/Other" : "../RelativeFlyout/Other", parameters);

        Assert.Equal(!failInitialization, result.Success);
        Assert.NotNull(flyout);
        Assert.Equal(failInitialization ? new[] { "Destroy" } : new[] { "From", "Destroy" }, flyout.OriginalDetail.Lifecycle);
    }

    [Fact]
    public async Task FailedRouteCleanup_ContinuesAfterEachRootFailureAndPreservesInitializationError()
    {
        _app.MainPage = new NavigationPage(new RelativeNavigationPageMock());
        var original = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(original);
        var created = new List<RelativeNavigationPageMock>();
        var parameters = new NavigationParameters
        {
            { "initializeObserver", (Action<RelativeNavigationPageMock>)created.Add },
            { "throwOnDestroy", true }
        };

        var result = await ServiceFor(original).NavigateAsync("../Next?throwOnInitialize=true/Next", parameters);

        Assert.False(result.Success);
        Assert.Equal(2, created.Count);
        Assert.All(created, page => Assert.Equal(1, page.Destroyed));
        Assert.Contains("Destination initialization failed.", result.Exception.ToString());
        Assert.Contains("Destination cleanup failed.", result.Exception.ToString());
        Assert.Same(original, Assert.Single(_app.Window.Navigation.ModalStack));
    }

    [Theory]
    [InlineData("../../")]
    [InlineData("../../Next")]
    public async Task NestedModalNavigationHistory_RejectsAmbiguousRemovalBeforeMutation(string route)
    {
        var root = new RelativeNavigationPageMock();
        var main = new NavigationPage(root);
        _app.MainPage = main;
        var first = new RelativeNavigationPageMock();
        var current = new RelativeNavigationPageMock();
        var inner = new NavigationPage(first);
        await inner.PushAsync(current);
        var tabs = new TabbedPage { Children = { inner } };
        var modal = new NavigationPage(tabs);
        await _app.Window.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(current).NavigateAsync(route);

        _output.WriteLine($"Success={result.Success}; RootDestroyed={root.Destroyed}; RootRetained={main.Navigation.NavigationStack.Contains(root)}; Error={result.Exception}");
        Assert.False(result.Success);
        Assert.Contains("nested navigation stack", result.Exception.Message);
        Assert.Same(root, main.Navigation.NavigationStack.First());
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Same(tabs, Assert.Single(modal.Navigation.NavigationStack));
        Assert.Equal(new Page[] { first, current }, inner.Navigation.NavigationStack);
        Assert.All(new[] { root, first, current }, page =>
        {
            Assert.Equal(0, page.Destroyed);
            Assert.Equal(0, page.NavigatedFromCount);
            Assert.Equal(0, page.Confirmations);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RetainedNestedNavigationStack_StillAcceptsRelativeForwardNavigation(bool modalHost)
    {
        var first = new RelativeNavigationPageMock();
        var retained = new RelativeNavigationPageMock();
        var inner = new NavigationPage(first);
        await inner.PushAsync(retained);
        var tabs = new TabbedPage { Children = { inner } };
        var host = new NavigationPage(tabs);
        _app.MainPage = modalHost ? new NavigationPage(new RelativeNavigationPageMock()) : host;
        if (modalHost)
            await _app.Window.Navigation.PushModalAsync(host);
        var current = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(current);

        var result = await ServiceFor(current).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(3, inner.Navigation.NavigationStack.Count);
        Assert.Same(first, inner.Navigation.NavigationStack[0]);
        Assert.Same(retained, inner.Navigation.NavigationStack[1]);
        Assert.Equal(0, first.Destroyed);
        Assert.Equal(0, retained.Destroyed);
        Assert.Equal(modalHost ? 1 : 0, _app.Window.Navigation.ModalStack.Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NavigationTabbedModal_WithOneLogicalPage_CanBeRemoved(bool navigationTab)
    {
        var root = new RelativeNavigationPageMock();
        _app.MainPage = new NavigationPage(root);
        var current = new RelativeNavigationPageMock();
        var tabs = new TabbedPage { Children = { navigationTab ? new NavigationPage(current) : current } };
        await _app.Window.Navigation.PushModalAsync(new NavigationPage(tabs));

        var result = await ServiceFor(current).NavigateAsync("../Next");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(root, _app.MainPage.Navigation.NavigationStack.First());
        Assert.Equal(2, _app.MainPage.Navigation.NavigationStack.Count);
        Assert.Empty(_app.Window.Navigation.ModalStack);
    }

    [Fact]
    public async Task NestedApplicationRoot_CannotBeReplacedThroughItsInnerStack()
    {
        var root = new RelativeNavigationPageMock();
        var inner = new NavigationPage(root);
        var tabs = new TabbedPage { Children = { inner } };
        var outer = new NavigationPage(tabs);
        _app.MainPage = outer;
        var modal = new RelativeNavigationPageMock();
        await _app.Window.Navigation.PushModalAsync(modal);

        var result = await ServiceFor(modal).NavigateAsync("../../Next");

        Assert.False(result.Success);
        Assert.Contains("different navigation stack", result.Exception.Message);
        Assert.Same(modal, Assert.Single(_app.Window.Navigation.ModalStack));
        Assert.Same(tabs, Assert.Single(outer.Navigation.NavigationStack));
        Assert.Same(root, Assert.Single(inner.Navigation.NavigationStack));
        Assert.Equal(0, root.Destroyed);
        Assert.Equal(0, modal.Destroyed);
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
