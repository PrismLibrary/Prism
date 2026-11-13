using Prism.Common;
using Prism.Dialogs;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.DryIoc.Maui.Tests.Mocks.Navigation;
using Prism.Events;
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class GoBackToRootTests : TestBase
{
    public GoBackToRootTests(ITestOutputHelper output) : base(output) { }

    [Theory]
    [InlineData("Source", "Middle?useModalNavigation=true", "Current?useModalNavigation=true")]
    [InlineData("NavigationPage/Source/Destination", "NavigationPage?useModalNavigation=true/Middle/Current", null)]
    [InlineData("NavigationPage/Source/Destination", "Middle?useModalNavigation=true", "NavigationPage/Current")]
    public async Task ModalContext_DismissesAllModals_AndRetainsUnderlyingStack(string route, string first, string second)
    {
        var window = CreateWindow(route);
        var destination = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var underlying = window.Page.Navigation.NavigationStack.ToArray();
        await GetService(destination).NavigateAsync(first);
        if (second is not null)
            await GetService(window.CurrentPage).NavigateAsync(second);
        var modals = window.Navigation.ModalStack.ToArray();
        var removed = modals.SelectMany(p => PageNavigationHistory.GetPages(p, true))
            .OfType<MockNavigateFromPage>().ToArray();
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var confirmations = current.Confirmations;
        var fromCounts = removed.Select(p => p.NavigatedFromCount).ToArray();
        var toCount = destination.NavigatedToCount;
        var value = new object();
        var service = Assert.IsType<TestPageNavigationService>(GetService(current));
        var recorder = service.Recorder;
        var popCount = recorder.Pops.Count;
        var events = new List<NavigationRequestContext>();
        current.GetContainerProvider().Resolve<IEventAggregator>().GetEvent<NavigationRequestEvent>().Subscribe(events.Add);

        var result = await service.GoBackToRootAsync(new NavigationParameters
        {
            { "value", value }, { KnownNavigationParameters.Animated, false },
            { KnownNavigationParameters.GoBackToRootBehavior, GoBackToRootBehavior.WindowContent }
        });

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Equal(underlying, window.Page.Navigation.NavigationStack);
        Assert.Same(destination, window.CurrentPage);
        Assert.Equal(confirmations + 1, current.Confirmations);
        Assert.Equal(toCount + 1, destination.NavigatedToCount);
        Assert.Same(value, destination.ReceivedParameters.GetValue<object>("value"));
        Assert.Equal(Prism.Navigation.NavigationMode.Back, destination.ReceivedParameters.GetNavigationMode());
        Assert.Equal(0, destination.Destroyed);
        for (var i = 0; i < removed.Length; i++)
        {
            Assert.Equal(fromCounts[i] + 1, removed[i].NavigatedFromCount);
            Assert.Equal(1, removed[i].Destroyed);
        }
        Assert.Equal(modals.Length, recorder.Pops.Count - popCount);
        Assert.All(recorder.Pops.Skip(popCount), pop =>
        {
            Assert.True(pop.UseModalNavigation);
            Assert.False(pop.Animated);
        });
        Assert.True(Assert.Single(events).Result.Success);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ModalConfirmation_VetoOrCancellation_PreservesStack_AndAllowsRetry(bool canceled)
    {
        var window = CreateWindow("Source");
        await GetService(window.CurrentPage).NavigateAsync("NavigationPage/Middle/Current");
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var modals = window.Navigation.ModalStack.ToArray();
        current.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var operation = GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));
        await current.ConfirmationStarted.Task;
        Assert.Same(modals.Last(), window.PendingModalConfirmation);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.Equal(modals, window.Navigation.ModalStack);
        if (canceled)
            current.Decision.SetCanceled();
        else
            current.Decision.SetResult(false);

        Assert.False((await operation).Success);
        Assert.Equal(modals, window.Navigation.ModalStack);
        Assert.Equal(0, current.Destroyed);
        Assert.Null(window.PendingModalConfirmation);
        current.Decision = null;
        Assert.True((await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }

    [Theory]
    [InlineData("NavigationPage/Source")]
    [InlineData("NavigationPage/Source/Middle/Current")]
    public async Task NonmodalContext_PreservesExistingRootBehavior(string route)
    {
        var window = CreateWindow(route);
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var root = Assert.IsType<MockNavigateFromPage>(navigation.RootPage);
        var removed = navigation.Navigation.NavigationStack.Skip(1).Cast<MockNavigateFromPage>().ToArray();
        var toCount = root.NavigatedToCount;

        var result = await GetService(window.CurrentPage).GoBackToRootAsync();

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(root, Assert.Single(navigation.Navigation.NavigationStack));
        Assert.Equal(toCount + 1, root.NavigatedToCount);
        Assert.Equal(0, root.Destroyed);
        Assert.All(removed, page => Assert.Equal(1, page.Destroyed));
    }

    [Fact]
    public async Task ModalSynchronousConfirmation_Veto_PreservesModal()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("MockViewB?useModalNavigation=true")).Success);
        var current = window.CurrentPage;
        var viewModel = Assert.IsType<Prism.DryIoc.Maui.Tests.Mocks.ViewModels.MockViewBViewModel>(current.BindingContext);
        viewModel.StopNavigation = true;

        Assert.False((await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        viewModel.StopNavigation = false;
        Assert.True((await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }

    [Fact]
    public async Task FailedModalPop_DoesNotDestroyPage_AndAllowsRetry()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        TestPageNavigationService.ForceNextDoPopToReturnNull = true;

        Assert.False((await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }
    [Fact]
    public async Task DialogAboveModal_IsPreservedUntilItsNormalCloseLifecycle()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var service = GetService(current);
        var resultReceived = new TaskCompletionSource<IDialogResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        current.GetContainerProvider().Resolve<IDialogService>().ShowDialog("RootDialog",
            new DialogParameters(), new DialogCallback().OnClose(result => resultReceived.TrySetResult(result)));
        var dialog = Assert.IsType<DialogContainerPage>(window.Navigation.ModalStack.Last());
        var view = Assert.IsType<MockRootNavigationDialog>(dialog.DialogView);
        var original = window.Navigation.ModalStack.ToArray();
        try
        {
            view.AllowClose = false;
            Assert.False((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
            Assert.Equal(original, window.Navigation.ModalStack);
            Assert.Contains(dialog, IDialogContainer.DialogStack);
            Assert.Equal(0, view.Closed);
            Assert.False(resultReceived.Task.IsCompleted);
            Assert.Equal(0, current.Destroyed);

            view.AllowClose = true;
            view.RequestClose.Invoke(ButtonResult.OK);
            Assert.Equal(ButtonResult.OK, (await resultReceived.Task.WaitAsync(TimeSpan.FromSeconds(5))).Result);
            Assert.Equal(1, view.Closed);
            Assert.DoesNotContain(dialog, IDialogContainer.DialogStack);
            Assert.True((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
            Assert.Empty(window.Navigation.ModalStack);
        }
        finally
        {
            IDialogContainer.DialogStack.Clear();
        }
    }

    [Fact]
    public async Task StackChangedDuringConfirmation_IsPreserved_AndAllowsRetry()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        current.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var service = GetService(current);
        var operation = service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));
        await current.ConfirmationStarted.Task;
        var unrelated = new ContentPage();
        await window.Navigation.PushModalAsync(unrelated, false);
        var original = window.Navigation.ModalStack.ToArray();
        current.Decision.SetResult(true);

        Assert.False((await operation).Success);
        Assert.Equal(original, window.Navigation.ModalStack);
        Assert.Equal(0, current.Destroyed);
        Assert.Null(window.PendingModalConfirmation);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        await window.Navigation.PopModalAsync(false);
        current.Decision = null;
        Assert.True((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LaterModalPopFails_RestoresOriginalStackScopesAndLifecycles(bool mutatedBeforeFailure)
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Middle?useModalNavigation=true")).Success);
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var original = window.Navigation.ModalStack.ToArray();
        var pages = original.Cast<MockNavigateFromPage>().ToArray();
        var scopes = pages.Select(p => p.GetContainerProvider()).ToArray();
        var fromCounts = pages.Select(p => p.NavigatedFromCount).ToArray();
        var current = pages.Last();
        var service = GetService(current);
        TestPageNavigationService.AfterNextPop = () =>
        {
            if (mutatedBeforeFailure)
                TestPageNavigationService.AfterNextPop = () => throw new InvalidOperationException("Second native pop failed.");
            else
                TestPageNavigationService.ForceNextDoPopToReturnNull = true;
            return Task.CompletedTask;
        };

        Assert.False((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
        Assert.Equal(original, window.Navigation.ModalStack);
        for (var i = 0; i < pages.Length; i++)
        {
            Assert.Equal(0, pages[i].Destroyed);
            Assert.Equal(fromCounts[i], pages[i].NavigatedFromCount);
            Assert.Same(scopes[i], pages[i].GetContainerProvider());
        }
        Assert.Same(current, window.CurrentPage);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }

    [Fact]
    public async Task DismissedTabbedModal_NotifiesAndDestroysInactiveChildren()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "TabbedPage?useModalNavigation=true&createTab=Middle&createTab=Current")).Success);
        var modal = Assert.IsAssignableFrom<Microsoft.Maui.Controls.TabbedPage>(window.Navigation.ModalStack.Last());
        var pages = modal.Children.Cast<MockNavigateFromPage>().ToArray();
        var fromCounts = pages.Select(p => p.NavigatedFromCount).ToArray();

        Assert.True((await GetService(window.CurrentPage).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
        Assert.Empty(window.Navigation.ModalStack);
        for (var i = 0; i < pages.Length; i++)
        {
            Assert.Equal(fromCounts[i] + 1, pages[i].NavigatedFromCount);
            Assert.Equal(1, pages[i].Destroyed);
        }
    }
    [Fact]
    public async Task DismissedFlyoutModal_NotifiesAndDestroysInactiveMenu()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "RootFlyout?useModalNavigation=true/NavigationPage/Middle/Current")).Success);
        var modal = Assert.IsType<MockRootNavigationFlyout>(window.Navigation.ModalStack.Last());
        var pages = PageNavigationHistory.GetPages(modal, true).OfType<MockNavigateFromPage>().ToArray();
        var fromCounts = pages.Select(p => p.NavigatedFromCount).ToArray();

        Assert.True((await GetService(window.CurrentPage).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
        Assert.Empty(window.Navigation.ModalStack);
        for (var i = 0; i < pages.Length; i++)
        {
            Assert.Equal(fromCounts[i] + 1, pages[i].NavigatedFromCount);
            Assert.Equal(1, pages[i].Destroyed);
        }
    }

    [Theory]
    [InlineData(1, false, false)]
    [InlineData(1, true, false)]
    [InlineData(1, false, true)]
    [InlineData(1, true, true)]
    [InlineData(2, false, false)]
    [InlineData(2, true, false)]
    [InlineData(2, false, true)]
    [InlineData(2, true, true)]
    public async Task ExternalModalPushDuringPop_PreservesAttachedPagesAndFinalizesDetachedPages(
        int detachedCount, bool nativeFailure, bool lifecycleFailure)
    {
        var window = CreateWindow("Source");
        var root = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Middle?useModalNavigation=true")).Success);
        var retained = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var rootTo = root.NavigatedToCount;
        var removed = new List<MockNavigateFromPage>();
        for (var i = 0; i < detachedCount; i++)
        {
            Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true",
                new NavigationParameters { { "failDeparture", lifecycleFailure && i == detachedCount - 1 },
                    { "failDestroy", lifecycleFailure && i == detachedCount - 1 } })).Success);
            removed.Add(Assert.IsType<MockNavigateFromPage>(window.CurrentPage));
        }
        var retainedFrom = retained.NavigatedFromCount;
        var fromCounts = removed.Select(page => page.NavigatedFromCount).ToArray();
        var unrelated = new ContentPage();
        var pops = 0;
        Func<Task> afterPop = null;
        afterPop = async () =>
        {
            if (++pops < detachedCount)
                TestPageNavigationService.AfterNextPop = afterPop;
            else
            {
                await window.Navigation.PushModalAsync(unrelated, false);
                TestPageNavigationService.ThrowAfterNextPop = nativeFailure;
            }
        };
        TestPageNavigationService.AfterNextPop = afterPop;

        var result = await GetService(removed.Last()).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));

        Assert.False(result.Success);
        Assert.Collection(window.Navigation.ModalStack,
            page => Assert.Same(retained, page),
            page => Assert.Same(unrelated, page));
        Assert.Equal(0, retained.Destroyed);
        Assert.Equal(retainedFrom, retained.NavigatedFromCount);
        Assert.Equal(rootTo, root.NavigatedToCount);
        Assert.Equal(0, root.Destroyed);
        for (var i = 0; i < removed.Count; i++)
        {
            Assert.Equal(fromCounts[i] + 1, removed[i].NavigatedFromCount);
            Assert.Equal(1, removed[i].Destroyed);
        }
        if (nativeFailure)
            Assert.Contains("Native pop failed", result.Exception.ToString());
        if (lifecycleFailure)
        {
            Assert.Contains("Departure failed.", result.Exception.ToString());
            Assert.Contains("Destroy failed.", result.Exception.ToString());
        }
        Assert.Null(window.PendingRootModalConfirmations);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        await window.Navigation.PopModalAsync(false);
        Assert.True((await GetService(retained).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExternallyReattachedModalDuringPop_IsNotFinalized(bool replaceRoot)
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var fromCount = current.NavigatedFromCount;
        var unrelated = new ContentPage();
        Page[] externallyChangedStack = null;
        TestPageNavigationService.AfterNextPop = async () =>
        {
            await window.Navigation.PushModalAsync(unrelated, false);
            if (replaceRoot)
                window.Page = current;
            else
                await window.Navigation.PushModalAsync(current, false);
            externallyChangedStack = window.Navigation.ModalStack.ToArray();
        };

        Assert.False((await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);

        Assert.Equal(fromCount, current.NavigatedFromCount);
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(externallyChangedStack, window.Navigation.ModalStack);
        Assert.Same(current, replaceRoot ? window.Page : window.Navigation.ModalStack.Last());
        Assert.Null(window.PendingRootModalConfirmations);
    }
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public async Task ModalTransferredDuringPop_PreservesOtherWindowsPagesAndScopes(
        bool wrapped, bool replaceRoot, bool changeOriginStack)
    {
        var window = CreateWindow("Source");
        var root = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(root).NavigateAsync("Middle?useModalNavigation=true")).Success);
        var retained = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(retained).NavigateAsync(wrapped
            ? "NavigationPage?useModalNavigation=true/Current"
            : "Current?useModalNavigation=true")).Success);
        var modal = window.Navigation.ModalStack.Last();
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var fromCount = current.NavigatedFromCount;
        var retainedFrom = retained.NavigatedFromCount;
        var rootTo = root.NavigatedToCount;
        var provider = current.GetContainerProvider();
        var service = GetService(current);
        var model = new DestructionRecorder();
        current.BindingContext = model;
        var otherRoot = new ContentPage();
        var secondWindow = new PrismWindow("second") { Page = otherRoot };
        var unrelated = new ContentPage();
        TestPageNavigationService.AfterNextPop = async () =>
        {
            if (changeOriginStack)
                await window.Navigation.PushModalAsync(unrelated, false);
            if (replaceRoot)
                secondWindow.Page = modal;
            else
                await secondWindow.Navigation.PushModalAsync(modal, false);
            Assert.Same(secondWindow, current.GetParentWindow());
        };

        var result = await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));

        Assert.False(result.Success);
        if (changeOriginStack)
            Assert.Collection(window.Navigation.ModalStack,
                page => Assert.Same(retained, page), page => Assert.Same(unrelated, page));
        else
            Assert.Same(retained, Assert.Single(window.Navigation.ModalStack));
        Assert.Same(root, window.Page);
        Assert.Same(modal, replaceRoot ? secondWindow.Page : Assert.Single(secondWindow.Navigation.ModalStack));
        if (!replaceRoot)
            Assert.Same(otherRoot, secondWindow.Page);
        Assert.Same(secondWindow, current.GetParentWindow());
        Assert.Equal(fromCount, current.NavigatedFromCount);
        Assert.Equal(retainedFrom, retained.NavigatedFromCount);
        Assert.Equal(rootTo, root.NavigatedToCount);
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(0, model.Destroyed);
        Assert.Equal(0, retained.Destroyed);
        Assert.Same(provider, current.GetContainerProvider());
        Assert.Same(service, GetService(current));
        Assert.Null(window.PendingRootModalConfirmations);
        Assert.Null(secondWindow.PendingRootModalConfirmations);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EarlierModalTransferredDuringLaterPop_PreservesOwnerAndCleansDetachedSibling(bool nativeFailure)
    {
        var window = CreateWindow("Source");
        var root = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(root).NavigateAsync("Middle?useModalNavigation=true")).Success);
        var middle = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(middle).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var middleFrom = middle.NavigatedFromCount;
        var currentFrom = current.NavigatedFromCount;
        var rootTo = root.NavigatedToCount;
        var provider = current.GetContainerProvider();
        var service = GetService(current);
        var secondWindow = new PrismWindow("second") { Page = new ContentPage() };
        TestPageNavigationService.AfterNextPop = () =>
        {
            TestPageNavigationService.AfterNextPop = () =>
            {
                secondWindow.Page = current;
                TestPageNavigationService.ThrowAfterNextPop = nativeFailure;
                return Task.CompletedTask;
            };
            return Task.CompletedTask;
        };

        var result = await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));

        Assert.False(result.Success);
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Same(root, window.Page);
        Assert.Same(current, secondWindow.Page);
        Assert.Same(secondWindow, current.GetParentWindow());
        Assert.Equal(currentFrom, current.NavigatedFromCount);
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(middleFrom + 1, middle.NavigatedFromCount);
        Assert.Equal(1, middle.Destroyed);
        Assert.Equal(rootTo, root.NavigatedToCount);
        Assert.Same(provider, current.GetContainerProvider());
        Assert.Same(service, GetService(current));
        if (nativeFailure)
            Assert.Contains("Native pop failed", result.Exception.ToString());
        Assert.Null(window.PendingRootModalConfirmations);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task InactiveFlyoutBranchChangedDuringConfirmation_IsPreserved(bool replaceMenu)
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "RootFlyout?useModalNavigation=true/NavigationPage/Middle/Current")).Success);
        var modal = Assert.IsType<MockRootNavigationFlyout>(window.Navigation.ModalStack.Last());
        var menuRoot = new MockNavigateFromPage { Title = "Menu" };
        var menu = new NavigationPage(menuRoot) { Title = "Menu" };
        modal.Flyout = menu;
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        current.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var service = GetService(current);
        var operation = service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));
        await current.ConfirmationStarted.Task;
        var unrelated = new MockNavigateFromPage { Title = "External menu" };
        if (replaceMenu)
            modal.Flyout = unrelated;
        else
            await menu.Navigation.PushAsync(unrelated, false);
        current.Decision.SetResult(true);

        Assert.False((await operation).Success);
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(0, unrelated.Destroyed);
        Assert.Equal(0, menuRoot.Destroyed);
        Assert.Equal(0, current.Destroyed);
        if (replaceMenu)
            Assert.Same(unrelated, modal.Flyout);
        else
            Assert.Same(unrelated, menu.CurrentPage);
        Assert.Null(window.PendingModalConfirmation);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);

        modal.Flyout = menu;
        if (!replaceMenu)
            await menu.Navigation.PopAsync(false);
        current.Decision = null;
        Assert.True((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task LifecycleFailure_StillDestroysEveryRemovedModal(bool failDeparture, bool failDestroy)
    {
        var window = CreateWindow("Source");
        var root = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(root).NavigateAsync("Middle?useModalNavigation=true")).Success);
        var middle = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(middle).NavigateAsync("Current?useModalNavigation=true", new NavigationParameters
        {
            { "failDeparture", failDeparture }, { "failDestroy", failDestroy }
        })).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);

        var result = await GetService(current).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));

        Assert.False(result.Success);
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Same(root, window.CurrentPage);
        Assert.Equal(1, current.Destroyed);
        Assert.Equal(1, middle.Destroyed);
        Assert.Equal(0, root.Destroyed);
        if (failDeparture)
            Assert.Contains("Departure failed.", result.Exception.ToString());
        if (failDestroy)
            Assert.Contains("Destroy failed.", result.Exception.ToString());
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await GetService(root).NavigateAsync("Destination?useModalNavigation=true")).Success);
    }
    [Theory]
    [InlineData("add")]
    [InlineData("remove")]
    [InlineData("reorder")]
    public async Task InactiveTabMembershipChangedDuringConfirmation_IsPreserved(string mutation)
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "TabbedPage?useModalNavigation=true&createTab=Middle&createTab=Current&createTab=Destination")).Success);
        var modal = Assert.IsAssignableFrom<Microsoft.Maui.Controls.TabbedPage>(window.Navigation.ModalStack.Last());
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var original = modal.Children.Cast<MockNavigateFromPage>().ToArray();
        var inactive = original.Last();
        var fromCounts = original.Select(page => page.NavigatedFromCount).ToArray();
        var toCounts = original.Select(page => page.NavigatedToCount).ToArray();
        var unrelated = new MockNavigateFromPage { Title = "External tab" };
        current.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var service = GetService(current);
        var operation = service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));
        await current.ConfirmationStarted.Task;
        if (mutation == "remove")
            modal.Children.Remove(inactive);
        else if (mutation == "reorder")
        {
            modal.Children.Remove(inactive);
            modal.Children.Insert(1, inactive);
        }
        else
            modal.Children.Add(unrelated);
        Assert.Same(current, modal.CurrentPage);
        var changed = modal.Children.ToArray();
        current.Decision.SetResult(true);

        Assert.False((await operation).Success);
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(changed, modal.Children);
        for (var i = 0; i < original.Length; i++)
        {
            Assert.Equal(0, original[i].Destroyed);
            Assert.Equal(fromCounts[i], original[i].NavigatedFromCount);
            Assert.Equal(toCounts[i], original[i].NavigatedToCount);
        }
        Assert.Equal(0, unrelated.Destroyed);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);

        if (mutation == "add")
            modal.Children.Remove(unrelated);
        else
        {
            modal.Children.Remove(inactive);
            modal.Children.Add(inactive);
        }
        current.Decision = null;
        Assert.True((await service.GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent))).Success);
    }
    [Theory]
    [InlineData("NavigationPage?useModalNavigation=true/Middle/Current")]
    [InlineData("TabbedPage?useModalNavigation=true&createTab=Middle&createTab=Current")]
    public async Task NestedDestructionFailure_StillDestroysSiblingsViewModelsAndContainer(string route)
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(route)).Success);
        var modal = Assert.Single(window.Navigation.ModalStack);
        var pages = PageNavigationHistory.GetPages(modal, true).OfType<MockNavigateFromPage>().ToArray();
        pages.Last().ThrowOnDestroy = true;
        var models = pages.Select(page => new DestructionRecorder()).ToArray();
        for (var i = 0; i < pages.Length; i++)
            pages[i].BindingContext = models[i];
        var containerModel = new DestructionRecorder();
        modal.BindingContext = containerModel;

        var result = await GetService(window.CurrentPage).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));

        Assert.False(result.Success);
        Assert.Empty(window.Navigation.ModalStack);
        Assert.All(pages, page => Assert.Equal(1, page.Destroyed));
        Assert.All(models, model => Assert.Equal(1, model.Destroyed));
        Assert.Equal(1, containerModel.Destroyed);
        Assert.Contains("Destroy failed.", result.Exception.ToString());
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
    }

    [Theory]
    [InlineData(false, "allow")]
    [InlineData(false, "veto")]
    [InlineData(false, "cancel")]
    [InlineData(false, "exception")]
    [InlineData(true, "allow")]
    [InlineData(true, "veto")]
    [InlineData(true, "cancel")]
    [InlineData(true, "exception")]
    public async Task RootRequestFromLowerModal_DoesNotQueueUpperDeviceBack(bool wrapped, string outcome)
    {
        var window = CreateWindow("NavigationPage/Source");
        var root = ((NavigationPage)window.Page).RootPage;
        Assert.True((await GetService(root).NavigateAsync("Pending?useModalNavigation=true")).Success);
        var lower = Assert.IsType<MockPendingPage>(window.CurrentPage);
        lower.Decision.SetResult(true);
        Assert.True((await GetService(lower).NavigateAsync(wrapped
            ? "NavigationPage?useModalNavigation=true/Current"
            : "Current?useModalNavigation=true")).Success);
        var original = window.Navigation.ModalStack.ToArray();
        var upper = original.Last();
        var upperPage = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var upperConfirmations = upperPage.Confirmations;
        lower.ResetConfirmation();
        var lowerConfirmations = lower.Confirmations;
        var events = new List<NavigationRequestContext>();
        var aggregator = root.GetContainerProvider().Resolve<IEventAggregator>().GetEvent<NavigationRequestEvent>();
        var token = aggregator.Subscribe(events.Add);
        try
        {
            var operation = GetService(lower).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowContent));
            await lower.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            var deviceRequests = new List<Task>();
            if (wrapped)
            {
                deviceRequests.Add(MvvmHelpers.HandleNavigationPageGoBack((NavigationPage)upper));
                deviceRequests.Add(MvvmHelpers.HandleNavigationPageGoBack((NavigationPage)upper));
            }
            else
            {
                await window.Navigation.PopModalAsync(false);
                await window.Navigation.PopModalAsync(false);
            }
            var queued = deviceRequests.Any(task => !task.IsCompleted);
            switch (outcome)
            {
                case "allow": lower.Decision.SetResult(true); break;
                case "veto": lower.Decision.SetResult(false); break;
                case "cancel": lower.Decision.SetCanceled(); break;
                default: lower.Decision.SetException(new InvalidOperationException("Confirmation failed.")); break;
            }
            var result = await operation.WaitAsync(TimeSpan.FromSeconds(5));
            await Task.WhenAll(deviceRequests).WaitAsync(TimeSpan.FromSeconds(5));
            // An unrelated request provides a semaphore barrier for raw ModalPopping handlers.
            Assert.True((await GetService(root).GoBackToRootAsync()).Success);

            Assert.False(queued);
            Assert.Empty(events.Where(context => context.Type == NavigationRequestType.GoBack));
            Assert.Equal(outcome == "allow", result.Success);
            Assert.Equal(lowerConfirmations + 1, lower.Confirmations);
            Assert.Equal(upperConfirmations, upperPage.Confirmations);
            Assert.Equal(outcome == "allow" ? 1 : 0, upperPage.Destroyed);
            if (outcome == "allow")
                Assert.Empty(window.Navigation.ModalStack);
            else
                Assert.Equal(original, window.Navigation.ModalStack);
            Assert.Null(window.PendingModalConfirmation);
            Assert.Null(window.PendingRootModalConfirmations);
            Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
            if (outcome != "allow")
            {
                Assert.True((await GetService(upperPage).GoBackAsync()).Success);
                Assert.Same(lower, Assert.Single(window.Navigation.ModalStack));
            }
        }
        finally
        {
            aggregator.Unsubscribe(token);
        }
    }

    private static NavigationParameters RootParameters(GoBackToRootBehavior behavior) => new()
    {
        { KnownNavigationParameters.GoBackToRootBehavior, behavior }
    };

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ModalNavigation_DefaultRetainsModalAndPopsItsOwnStack(bool explicitDefault)
    {
        var window = CreateWindow("Source");
        var home = window.Page;
        Assert.True((await GetService(home).NavigateAsync("NavigationPage/Middle/Current/Destination")).Success);
        var navigation = Assert.IsAssignableFrom<NavigationPage>(Assert.Single(window.Navigation.ModalStack));
        var root = Assert.IsType<MockNavigateFromPage>(navigation.RootPage);
        var caller = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var removed = navigation.Navigation.NavigationStack.Skip(1).Cast<MockNavigateFromPage>().ToArray();
        var confirmations = caller.Confirmations;
        var arrivals = root.NavigatedToCount;
        var parameters = explicitDefault ? RootParameters(GoBackToRootBehavior.Default) : new NavigationParameters();
        var value = new object();
        parameters.Add("value", value);

        var result = await GetService(caller).GoBackToRootAsync(parameters);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(home, window.Page);
        Assert.Same(navigation, Assert.Single(window.Navigation.ModalStack));
        Assert.Same(root, Assert.Single(navigation.Navigation.NavigationStack));
        Assert.Same(root, window.CurrentPage);
        Assert.Equal(confirmations + 1, caller.Confirmations);
        Assert.Equal(arrivals + 1, root.NavigatedToCount);
        Assert.Same(value, root.ReceivedParameters.GetValue<object>("value"));
        Assert.Equal(Prism.Navigation.NavigationMode.Back, root.ReceivedParameters.GetNavigationMode());
        Assert.Equal(0, root.Destroyed);
        Assert.All(removed, page => Assert.Equal(1, page.Destroyed));
    }

    [Theory]
    [InlineData(GoBackToRootBehavior.Default)]
    [InlineData(GoBackToRootBehavior.WindowContent)]
    [InlineData(GoBackToRootBehavior.WindowNavigationRoot)]
    public async Task PlainModalAboveModalNavigation_AllBehaviorsReturnToHome(GoBackToRootBehavior behavior)
    {
        var window = CreateWindow("Source");
        var home = Assert.IsType<MockNavigateFromPage>(window.Page);
        Assert.True((await GetService(home).NavigateAsync("NavigationPage/Middle/Current/Destination")).Success);
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Modal?useModalNavigation=true")).Success);
        var departing = window.Navigation.ModalStack.SelectMany(p => PageNavigationHistory.GetPages(p, true))
            .OfType<MockNavigateFromPage>().ToArray();
        var caller = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var confirmations = caller.Confirmations;
        var arrivals = home.NavigatedToCount;

        var result = await GetService(caller).GoBackToRootAsync(RootParameters(behavior));

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Same(home, window.CurrentPage);
        Assert.Equal(arrivals + 1, home.NavigatedToCount);
        Assert.Equal(confirmations + 1, caller.Confirmations);
        Assert.Equal(0, home.Destroyed);
        Assert.All(departing, page => Assert.Equal(1, page.Destroyed));
    }

    public static IEnumerable<object[]> WindowNavigationCases()
    {
        foreach (var root in new[] { "navigation", "flyout", "tabbed" })
        foreach (var behavior in new[] { GoBackToRootBehavior.Default, GoBackToRootBehavior.WindowContent, GoBackToRootBehavior.WindowNavigationRoot })
            yield return new object[] { root, behavior };
    }

    [Theory]
    [MemberData(nameof(WindowNavigationCases))]
    public async Task PlainModal_RespectsWindowScopeAndPreservesInactiveBranches(string rootKind, GoBackToRootBehavior behavior)
    {
        var route = rootKind switch
        {
            "flyout" => "RootFlyout/NavigationPage/Source/Middle/Current/Destination",
            "tabbed" => "TabbedPage?createTab=NavigationPage|Middle|Current&createTab=NavigationPage|Source|Middle|Current|Destination",
            _ => "NavigationPage/Source/Middle/Current/Destination"
        };
        var window = CreateWindow(route);
        var tabs = window.Page as Microsoft.Maui.Controls.TabbedPage;
        if (tabs is not null)
            tabs.CurrentPage = tabs.Children[1];
        var activeNavigation = window.Page switch
        {
            Microsoft.Maui.Controls.TabbedPage tabbed => (NavigationPage)tabbed.CurrentPage,
            FlyoutPage flyout => (NavigationPage)flyout.Detail,
            _ => (NavigationPage)window.Page
        };
        var original = activeNavigation.Navigation.NavigationStack.Cast<MockNavigateFromPage>().ToArray();
        var inactive = tabs is null ? Array.Empty<Page>() : ((NavigationPage)tabs.Children[0]).Navigation.NavigationStack.ToArray();
        var selected = tabs?.CurrentPage;
        var menu = (window.Page as FlyoutPage)?.Flyout;
        var priorCurrent = original.Last();
        Assert.True((await GetService(priorCurrent).NavigateAsync("Modal?useModalNavigation=true")).Success);
        var caller = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var callerConfirmations = caller.Confirmations;
        var priorArrivals = original.Select(p => p.NavigatedToCount).ToArray();
        var priorConfirmations = original.Select(p => p.Confirmations).ToArray();
        var value = new object();
        var parameters = RootParameters(behavior);
        parameters.Add("value", value);
        parameters.Add(KnownNavigationParameters.Animated, false);

        var result = await GetService(caller).GoBackToRootAsync(parameters);

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        var popToRoot = behavior == GoBackToRootBehavior.WindowNavigationRoot;
        var destination = popToRoot ? original[0] : priorCurrent;
        Assert.Same(destination, window.CurrentPage);
        Assert.Equal(popToRoot ? original.Take(1) : original, activeNavigation.Navigation.NavigationStack);
        Assert.Equal(callerConfirmations + 1, caller.Confirmations);
        Assert.Equal(1, caller.Destroyed);
        Assert.Equal(0, destination.Destroyed);
        Assert.Same(value, destination.ReceivedParameters.GetValue<object>("value"));
        Assert.Equal(Prism.Navigation.NavigationMode.Back, destination.ReceivedParameters.GetNavigationMode());
        for (var i = 0; i < original.Length; i++)
        {
            Assert.Equal(priorConfirmations[i], original[i].Confirmations);
            Assert.Equal(priorArrivals[i] + (original[i] == destination ? 1 : 0), original[i].NavigatedToCount);
            Assert.Equal(popToRoot && i > 0 ? 1 : 0, original[i].Destroyed);
        }
        if (tabs is not null)
        {
            Assert.Same(selected, tabs.CurrentPage);
            Assert.Equal(inactive, ((NavigationPage)tabs.Children[0]).Navigation.NavigationStack);
            Assert.All(inactive.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
        }
        if (menu is not null)
        {
            Assert.Same(menu, ((FlyoutPage)window.Page).Flyout);
            Assert.Equal(0, ((MockNavigateFromPage)menu).Destroyed);
        }
    }

    [Theory]
    [InlineData(GoBackToRootBehavior.Default)]
    [InlineData(GoBackToRootBehavior.WindowContent)]
    [InlineData(GoBackToRootBehavior.WindowNavigationRoot)]
    public async Task WindowScope_ConfirmationVetoLeavesBothStacksUntouched(GoBackToRootBehavior behavior)
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current/Destination");
        var navigation = (NavigationPage)window.Page;
        var stack = navigation.Navigation.NavigationStack.ToArray();
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Modal?useModalNavigation=true")).Success);
        var modal = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        modal.AllowNavigation = false;

        var result = await GetService(modal).GoBackToRootAsync(RootParameters(behavior));

        Assert.False(result.Success);
        Assert.Equal(stack, navigation.Navigation.NavigationStack);
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(0, modal.Destroyed);
        Assert.All(stack.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
    }

    [Theory]
    [InlineData(GoBackToRootBehavior.Default)]
    [InlineData(GoBackToRootBehavior.WindowContent)]
    [InlineData(GoBackToRootBehavior.WindowNavigationRoot)]
    public async Task NonmodalWindowNavigation_ExplicitBehaviorSelectsExpectedDestination(GoBackToRootBehavior behavior)
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current/Destination");
        var navigation = (NavigationPage)window.Page;
        var stack = navigation.Navigation.NavigationStack.Cast<MockNavigateFromPage>().ToArray();

        var result = await GetService(window.CurrentPage).GoBackToRootAsync(RootParameters(behavior));

        Assert.True(result.Success, result.Exception?.ToString());
        var retain = behavior == GoBackToRootBehavior.WindowContent;
        Assert.Same(retain ? stack.Last() : stack[0], window.CurrentPage);
        Assert.Equal(retain ? stack : stack.Take(1), navigation.Navigation.NavigationStack);
        Assert.All(stack.Skip(1), page => Assert.Equal(retain ? 0 : 1, page.Destroyed));
    }

    [Fact]
    public async Task InvalidRootBehavior_DoesNotMutateStacksOrDestroyPages()
    {
        var window = CreateWindow("NavigationPage/Source/Middle");
        var stack = window.Page.Navigation.NavigationStack.ToArray();
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Modal?useModalNavigation=true")).Success);
        var modal = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);

        var result = await GetService(modal).GoBackToRootAsync(RootParameters((GoBackToRootBehavior)999));

        Assert.False(result.Success);
        Assert.Equal(stack, window.Page.Navigation.NavigationStack);
        Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(0, modal.Destroyed);
    }

    [Theory]
    [InlineData(GoBackToRootBehavior.WindowContent)]
    [InlineData(GoBackToRootBehavior.WindowNavigationRoot)]
    public async Task ModalNavigation_ExplicitWindowScopeEscapesLocalNavigation(GoBackToRootBehavior behavior)
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current/Destination");
        var underlying = (NavigationPage)window.Page;
        var stack = underlying.Navigation.NavigationStack.ToArray();
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "NavigationPage?useModalNavigation=true/Middle/Current")).Success);
        var modal = Assert.IsAssignableFrom<NavigationPage>(Assert.Single(window.Navigation.ModalStack));
        var removed = modal.Navigation.NavigationStack.Cast<MockNavigateFromPage>().ToArray();

        var result = await GetService(window.CurrentPage).GoBackToRootAsync(RootParameters(behavior));

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        var popToRoot = behavior == GoBackToRootBehavior.WindowNavigationRoot;
        Assert.Same(popToRoot ? stack[0] : stack.Last(), window.CurrentPage);
        Assert.Equal(popToRoot ? stack.Take(1) : stack, underlying.Navigation.NavigationStack);
        Assert.All(removed, page => Assert.Equal(1, page.Destroyed));
    }

    [Theory]
    [InlineData(GoBackToRootBehavior.Default)]
    [InlineData(GoBackToRootBehavior.WindowContent)]
    [InlineData(GoBackToRootBehavior.WindowNavigationRoot)]
    public async Task PlainActiveTab_AllBehaviorsKeepSelectedTab(GoBackToRootBehavior behavior)
    {
        var window = CreateWindow("TabbedPage?createTab=Source&createTab=Destination");
        var tabs = Assert.IsAssignableFrom<Microsoft.Maui.Controls.TabbedPage>(window.Page);
        tabs.CurrentPage = tabs.Children[1];
        var selected = Assert.IsType<MockNavigateFromPage>(tabs.CurrentPage);
        var other = Assert.IsType<MockNavigateFromPage>(tabs.Children[0]);
        Assert.True((await GetService(selected).NavigateAsync("Modal?useModalNavigation=true")).Success);

        var result = await GetService(window.CurrentPage).GoBackToRootAsync(RootParameters(behavior));

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Same(selected, tabs.CurrentPage);
        Assert.Same(selected, window.CurrentPage);
        Assert.Equal(0, selected.Destroyed);
        Assert.Equal(0, other.Destroyed);
    }

    [Fact]
    public async Task NestedModalNavigation_DefaultLeavesOuterModalHistoryUntouched()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("NavigationPage/Middle/Current")).Success);
        var outer = Assert.IsAssignableFrom<NavigationPage>(Assert.Single(window.Navigation.ModalStack));
        var outerStack = outer.Navigation.NavigationStack.ToArray();
        Assert.True((await GetService(window.CurrentPage).NavigateAsync(
            "NavigationPage?useModalNavigation=true/Source/Middle/Current")).Success);
        var inner = Assert.IsAssignableFrom<NavigationPage>(window.Navigation.ModalStack.Last());
        var root = inner.RootPage;

        var result = await GetService(window.CurrentPage).GoBackToRootAsync();

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal(new Page[] { outer, inner }, window.Navigation.ModalStack);
        Assert.Equal(outerStack, outer.Navigation.NavigationStack);
        Assert.Same(root, Assert.Single(inner.Navigation.NavigationStack));
        Assert.Same(root, window.CurrentPage);
        Assert.All(outerStack.Cast<MockNavigateFromPage>(), page => Assert.Equal(0, page.Destroyed));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WindowNavigationRoot_FailedUnderlyingPopRestoresBothStacks(bool failAfterMutation)
    {
        var window = CreateWindow("NavigationPage/Source/Middle/Current/Destination");
        var navigation = (NavigationPage)window.Page;
        var stack = navigation.Navigation.NavigationStack.Cast<MockNavigateFromPage>().ToArray();
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Modal?useModalNavigation=true")).Success);
        var modal = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var pages = stack.Append(modal).ToArray();
        var fromCounts = pages.Select(p => p.NavigatedFromCount).ToArray();
        var toCounts = pages.Select(p => p.NavigatedToCount).ToArray();
        TestPageNavigationService.AfterNextPop = () =>
        {
            if (failAfterMutation)
                TestPageNavigationService.AfterNextPop = () => throw new InvalidOperationException("Underlying pop failed after mutation.");
            else
                TestPageNavigationService.ForceNextDoPopToReturnNull = true;
            return Task.CompletedTask;
        };
        try
        {
            var result = await GetService(modal).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowNavigationRoot));

            Assert.False(result.Success);
            Assert.Equal(stack, navigation.Navigation.NavigationStack);
            Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
            Assert.Same(modal, window.CurrentPage);
            for (var index = 0; index < pages.Length; index++)
            {
                Assert.Equal(0, pages[index].Destroyed);
                Assert.Equal(fromCounts[index], pages[index].NavigatedFromCount);
                Assert.Equal(toCounts[index], pages[index].NavigatedToCount);
            }
            Assert.True((await GetService(modal).GoBackToRootAsync(RootParameters(GoBackToRootBehavior.WindowNavigationRoot))).Success);
            Assert.Empty(window.Navigation.ModalStack);
            Assert.Same(stack[0], Assert.Single(navigation.Navigation.NavigationStack));
        }
        finally
        {
            TestPageNavigationService.AfterNextPop = null;
            TestPageNavigationService.ForceNextDoPopToReturnNull = false;
        }
    }

    [Theory]
    [InlineData(GoBackToRootBehavior.Default, "success")]
    [InlineData(GoBackToRootBehavior.WindowContent, "success")]
    [InlineData(GoBackToRootBehavior.WindowNavigationRoot, "success")]
    [InlineData(GoBackToRootBehavior.Default, "veto")]
    [InlineData(GoBackToRootBehavior.Default, "cancel")]
    [InlineData(GoBackToRootBehavior.Default, "native-before")]
    [InlineData(GoBackToRootBehavior.Default, "native-after")]
    [InlineData(GoBackToRootBehavior.Default, "destroy")]
    public async Task PlainModalAboveFourPageNavigation_DestroysEachViewModelExactlyOnce(
        GoBackToRootBehavior behavior, string outcome)
    {
        var window = CreateWindow("Source");
        var home = Assert.IsType<MockNavigateFromPage>(window.Page);
        Assert.True((await GetService(home).NavigateAsync("NavigationPage/Source/Middle/Current/Destination")).Success);
        var navigation = Assert.IsAssignableFrom<NavigationPage>(Assert.Single(window.Navigation.ModalStack));
        var stack = navigation.Navigation.NavigationStack.Cast<MockNavigateFromPage>().ToArray();
        Assert.Equal(4, stack.Length);
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Modal?useModalNavigation=true")).Success);
        var modal = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var originalModals = window.Navigation.ModalStack.ToArray();
        var homeViewModel = new DestructionRecorder();
        var containerViewModel = new DestructionRecorder();
        home.BindingContext = homeViewModel;
        navigation.BindingContext = containerViewModel;
        var removedPages = stack.Append(modal).ToArray();
        var viewModels = removedPages.Select(page =>
        {
            var viewModel = new DestructionRecorder();
            page.BindingContext = viewModel;
            return viewModel;
        }).ToArray();
        if (outcome == "veto")
            modal.AllowNavigation = false;
        if (outcome == "cancel")
        {
            modal.Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
            modal.Decision.SetCanceled();
        }
        if (outcome == "native-before")
            TestPageNavigationService.AfterNextPop = () =>
            {
                TestPageNavigationService.ForceNextDoPopToReturnNull = true;
                return Task.CompletedTask;
            };
        if (outcome == "native-after")
            TestPageNavigationService.AfterNextPop = () =>
            {
                TestPageNavigationService.AfterNextPop = () => throw new InvalidOperationException("Container pop failed.");
                return Task.CompletedTask;
            };
        if (outcome == "destroy")
            viewModels[2].ThrowOnDestroy = true;
        try
        {
            var result = await GetService(modal).GoBackToRootAsync(RootParameters(behavior));

            Assert.Equal(outcome == "success", result.Success);
            Assert.Equal(0, home.Destroyed);
            Assert.Equal(0, homeViewModel.Destroyed);
            var removed = outcome is "success" or "destroy";
            Assert.All(removedPages, page => Assert.Equal(removed ? 1 : 0, page.Destroyed));
            Assert.All(viewModels, viewModel => Assert.Equal(removed ? 1 : 0, viewModel.Destroyed));
            Assert.Equal(removed ? 1 : 0, containerViewModel.Destroyed);
            if (removed)
            {
                Assert.Empty(window.Navigation.ModalStack);
                Assert.Same(home, window.CurrentPage);
                if (outcome == "destroy")
                    Assert.Contains("View model destruction failed.", result.Exception.ToString());
            }
            else
            {
                Assert.Equal(originalModals, window.Navigation.ModalStack);
                Assert.Equal(stack, navigation.Navigation.NavigationStack);
                Assert.Same(modal, window.CurrentPage);
            }
        }
        finally
        {
            TestPageNavigationService.AfterNextPop = null;
            TestPageNavigationService.ForceNextDoPopToReturnNull = false;
        }
    }

    private sealed class DestructionRecorder : IDestructible
    {
        public int Destroyed { get; private set; }
        public bool ThrowOnDestroy { get; set; }
        public void Destroy()
        {
            Destroyed++;
            if (ThrowOnDestroy)
                throw new InvalidOperationException("View model destruction failed.");
        }
    }
    private PrismWindow CreateWindow(string route) => GetWindow(CreateBuilder(prism =>
        prism.RegisterTypes(container => container.RegisterForNavigation<MockPendingPage>("Pending").RegisterDialog<MockRootNavigationDialog>("RootDialog").RegisterForNavigation<MockRootNavigationFlyout>("RootFlyout").RegisterForNavigation<MockNavigateFromPage>("Source")
            .RegisterForNavigation<MockNavigateFromPage>("Middle")
            .RegisterForNavigation<MockNavigateFromPage>("Current")
            .RegisterForNavigation<MockNavigateFromPage>("Destination")
            .RegisterForNavigation<MockNavigateFromPage>("Modal"))
        .CreateWindow(route)).Build());

    private static INavigationService GetService(Page page) =>
        page.GetContainerProvider().Resolve<INavigationService>();
}
