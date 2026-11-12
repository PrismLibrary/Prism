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
            { "value", value }, { KnownNavigationParameters.Animated, false }
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
        var operation = GetService(current).GoBackToRootAsync();
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
        Assert.True((await GetService(current).GoBackToRootAsync()).Success);
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

        Assert.False((await GetService(current).GoBackToRootAsync()).Success);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        viewModel.StopNavigation = false;
        Assert.True((await GetService(current).GoBackToRootAsync()).Success);
    }

    [Fact]
    public async Task FailedModalPop_DoesNotDestroyPage_AndAllowsRetry()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        TestPageNavigationService.ForceNextDoPopToReturnNull = true;

        Assert.False((await GetService(current).GoBackToRootAsync()).Success);
        Assert.Same(current, Assert.Single(window.Navigation.ModalStack));
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await GetService(current).GoBackToRootAsync()).Success);
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
            Assert.False((await service.GoBackToRootAsync()).Success);
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
            Assert.True((await service.GoBackToRootAsync()).Success);
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
        var operation = service.GoBackToRootAsync();
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
        Assert.True((await service.GoBackToRootAsync()).Success);
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

        Assert.False((await service.GoBackToRootAsync()).Success);
        Assert.Equal(original, window.Navigation.ModalStack);
        for (var i = 0; i < pages.Length; i++)
        {
            Assert.Equal(0, pages[i].Destroyed);
            Assert.Equal(fromCounts[i], pages[i].NavigatedFromCount);
            Assert.Same(scopes[i], pages[i].GetContainerProvider());
        }
        Assert.Same(current, window.CurrentPage);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        Assert.True((await service.GoBackToRootAsync()).Success);
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

        Assert.True((await GetService(window.CurrentPage).GoBackToRootAsync()).Success);
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

        Assert.True((await GetService(window.CurrentPage).GoBackToRootAsync()).Success);
        Assert.Empty(window.Navigation.ModalStack);
        for (var i = 0; i < pages.Length; i++)
        {
            Assert.Equal(fromCounts[i] + 1, pages[i].NavigatedFromCount);
            Assert.Equal(1, pages[i].Destroyed);
        }
    }

    [Fact]
    public async Task ExternalModalPushDuringPop_IsNotOverwrittenByRollback()
    {
        var window = CreateWindow("Source");
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Middle?useModalNavigation=true")).Success);
        var retained = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        Assert.True((await GetService(window.CurrentPage).NavigateAsync("Current?useModalNavigation=true")).Success);
        var current = Assert.IsType<MockNavigateFromPage>(window.CurrentPage);
        var unrelated = new ContentPage();
        TestPageNavigationService.AfterNextPop = () => window.Navigation.PushModalAsync(unrelated, false);

        var result = await GetService(current).GoBackToRootAsync();

        Assert.False(result.Success);
        Assert.Collection(window.Navigation.ModalStack,
            page => Assert.Same(retained, page),
            page => Assert.Same(unrelated, page));
        Assert.Equal(0, retained.Destroyed);
        Assert.Equal(0, current.Destroyed);
        Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
    }
    private PrismWindow CreateWindow(string route) => GetWindow(CreateBuilder(prism =>
        prism.RegisterTypes(container => container.RegisterDialog<MockRootNavigationDialog>("RootDialog").RegisterForNavigation<MockRootNavigationFlyout>("RootFlyout").RegisterForNavigation<MockNavigateFromPage>("Source")
            .RegisterForNavigation<MockNavigateFromPage>("Middle")
            .RegisterForNavigation<MockNavigateFromPage>("Current")
            .RegisterForNavigation<MockNavigateFromPage>("Destination"))
        .CreateWindow(route)).Build());

    private static INavigationService GetService(Page page) =>
        page.GetContainerProvider().Resolve<INavigationService>();
}
