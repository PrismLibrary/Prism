using Prism.Common;
using Prism.DryIoc.Maui.Tests.Mocks.Navigation;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Events;
using Prism.Navigation.Xaml;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class PendingDeviceModalPopTests : TestBase
{
    public PendingDeviceModalPopTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task RelativeNestedModal_DeviceBackMustNotQueueAnotherNavigation(bool flyout, bool allow)
    {
        var app = CreateBuilder(p => p.RegisterTypes(c => c.RegisterForNavigation<MockPendingPage>())
            .CreateWindow("NavigationPage/MockViewA")).Build();
        var window = GetWindow(app);
        var rootNavigation = window.CurrentPage.GetContainerProvider().Resolve<INavigationService>();
        var route = flyout
            ? "MockHome?useModalNavigation=true/NavigationPage/MockPendingPage"
            : "TabbedPage?useModalNavigation=true&createTab=NavigationPage%7CMockPendingPage";
        Assert.True((await rootNavigation.NavigateAsync(route)).Success);
        var outer = Assert.Single(window.Navigation.ModalStack);
        var inner = Assert.IsAssignableFrom<NavigationPage>(flyout
            ? Assert.IsType<MockHome>(outer).Detail
            : Assert.IsAssignableFrom<Microsoft.Maui.Controls.TabbedPage>(outer).CurrentPage);
        var page = Assert.IsType<MockPendingPage>(inner.CurrentPage);
        var navigation = page.GetContainerProvider().Resolve<INavigationService>();
        var request = navigation.NavigateAsync("../MockViewC?useModalNavigation=true");
        await page.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var firstBack = MvvmHelpers.HandleNavigationPageGoBack(inner);
        var secondBack = MvvmHelpers.HandleNavigationPageGoBack(inner);
        var anotherNavigationQueued = !firstBack.IsCompleted || !secondBack.IsCompleted;
        page.Decision.SetResult(allow);
        var result = await request.WaitAsync(TimeSpan.FromSeconds(5));
        await Task.WhenAll(firstBack, secondBack).WaitAsync(TimeSpan.FromSeconds(5));

        Assert.False(anotherNavigationQueued);
        Assert.Equal(allow, result.Success);
        Assert.Equal(1, page.Confirmations);
        if (allow)
            Assert.IsType<MockViewC>(Assert.Single(window.Navigation.ModalStack));
        else
            Assert.Same(outer, Assert.Single(window.Navigation.ModalStack));
    }

    [Theory]
    [InlineData("allow", false, false)]
    [InlineData("allow", false, true)]
    [InlineData("allow", true, false)]
    [InlineData("allow", true, true)]
    [InlineData("veto", false, false)]
    [InlineData("veto", false, true)]
    [InlineData("veto", true, false)]
    [InlineData("veto", true, true)]
    [InlineData("cancel", false, false)]
    [InlineData("cancel", false, true)]
    [InlineData("cancel", true, false)]
    [InlineData("cancel", true, true)]
    [InlineData("exception", false, false)]
    [InlineData("exception", false, true)]
    [InlineData("exception", true, false)]
    [InlineData("exception", true, true)]
    public async Task RepeatedDeviceBack_WaitsForOneConfirmation_AndRecovers(string outcome, bool programmatic, bool wrapped)
    {
        var app = CreateBuilder(p => p.RegisterTypes(c => c.RegisterForNavigation<MockPendingPage>())
            .CreateWindow("NavigationPage/MockViewA")).Build();
        var window = GetWindow(app);
        var root = ((NavigationPage)window.Page).CurrentPage;
        var rootVm = Assert.IsAssignableFrom<MockViewModelBase>(root.BindingContext);
        var navigation = root.GetContainerProvider().Resolve<INavigationService>();
        Assert.True((await navigation.NavigateAsync(wrapped ? "NavigationPage?useModalNavigation=true/MockPendingPage" : "MockPendingPage?useModalNavigation=true")).Success);
        var modal = Assert.Single(window.Navigation.ModalStack);
        var page = Assert.IsType<MockPendingPage>(MvvmHelpers.GetCurrentPage(modal));
        var modalNavigation = page.GetContainerProvider().Resolve<INavigationService>();
        var rootTo = rootVm.Actions.Count(a => a == nameof(rootVm.OnNavigatedTo));
        var requests = new List<NavigationRequestContext>();
        var completion = new TaskCompletionSource<NavigationRequestContext>(TaskCreationOptions.RunContinuationsAsynchronously);
        var events = page.GetContainerProvider().Resolve<IEventAggregator>().GetEvent<NavigationRequestEvent>();
        var token = events.Subscribe(c =>
        {
            if (c.Type == NavigationRequestType.GoBack)
            {
                requests.Add(c);
                completion.TrySetResult(c);
            }
        });
        try
        {
            Task<INavigationResult> programmaticRequest = null;
            Task deviceRequest = null;
            if (programmatic)
                programmaticRequest = modalNavigation.GoBackAsync();
            else if (wrapped)
                deviceRequest = MvvmHelpers.HandleNavigationPageGoBack((NavigationPage)modal);
            else
                await window.Navigation.PopModalAsync();
            await page.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            if (wrapped)
            {
                await MvvmHelpers.HandleNavigationPageGoBack((NavigationPage)modal).WaitAsync(TimeSpan.FromSeconds(5));
                await MvvmHelpers.HandleNavigationPageGoBack((NavigationPage)modal).WaitAsync(TimeSpan.FromSeconds(5));
            }
            else
            {
                await window.Navigation.PopModalAsync();
                await window.Navigation.PopModalAsync();
            }
            // A raw modal must still be owned by MAUI, even while a Prism modal awaits confirmation.
            var rawModal = new ContentPage();
            await window.Navigation.PushModalAsync(rawModal);
            Assert.Same(rawModal, await window.Navigation.PopModalAsync());
            var otherWindow = new PrismWindow { Page = new ContentPage() };
            await otherWindow.Navigation.PushModalAsync(new ContentPage());
            await otherWindow.Navigation.PopModalAsync();
            Assert.Empty(otherWindow.Navigation.ModalStack);
            Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
            Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
            Assert.Equal(1, page.Confirmations);
            Assert.Empty(requests);
            Assert.Empty(modalNavigation.GetPops());
            Assert.Equal(0, page.NavigatedFrom);
            Assert.Equal(0, page.Destroyed);
            Assert.Equal(rootTo, rootVm.Actions.Count(a => a == nameof(rootVm.OnNavigatedTo)));

            switch (outcome)
            {
                case "allow":
                    page.Decision.SetResult(true);
                    break;
                case "veto":
                    page.Decision.SetResult(false);
                    break;
                case "cancel":
                    page.Decision.SetCanceled();
                    break;
                default:
                    page.Decision.SetException(new InvalidOperationException("confirmation failed"));
                    break;
            }
            var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(5));
            if (programmaticRequest is not null)
                await programmaticRequest;
            if (deviceRequest is not null)
                await deviceRequest;
            Assert.Equal(outcome == "allow", result.Result.Success);
            if (outcome == "veto")
                Assert.True(result.Cancelled);
            if (outcome == "cancel")
                Assert.IsAssignableFrom<OperationCanceledException>(result.Result.Exception);
            if (outcome == "exception")
                Assert.IsType<InvalidOperationException>(result.Result.Exception);
            Assert.Single(requests);
            if (outcome != "allow")
            {
                Assert.Same(modal, Assert.Single(window.Navigation.ModalStack));
                Assert.Equal(0, page.NavigatedFrom);
                Assert.Equal(0, page.Destroyed);
                Assert.Empty(modalNavigation.GetPops());
                Assert.Equal(rootTo, rootVm.Actions.Count(a => a == nameof(rootVm.OnNavigatedTo)));
                // NavigationRequestEvent is raised before the async modal handler completes.
                // Wait one normal UI navigation interval before sending a fresh device request.
                await Task.Delay(TimeSpan.FromMilliseconds(200));
                page.ResetConfirmation();
                completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
                // The handler's finally must release its guard after every outcome.
                await window.Navigation.PopModalAsync();
                await page.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
                page.Decision.SetResult(true);
                Assert.True((await completion.Task.WaitAsync(TimeSpan.FromSeconds(5))).Result.Success);
                Assert.Equal(2, page.Confirmations);
                Assert.Equal(2, requests.Count);
            }
            Assert.Empty(window.Navigation.ModalStack);
            Assert.Single(modalNavigation.GetPops());
            Assert.Equal(1, page.NavigatedFrom);
            Assert.Equal(1, page.Destroyed);
            Assert.Equal(rootTo + 1, rootVm.Actions.Count(a => a == nameof(rootVm.OnNavigatedTo)));

            Assert.True((await navigation.NavigateAsync("MockViewB")).Success);
            Assert.True((await ((NavigationPage)window.Page).CurrentPage.GetContainerProvider().Resolve<INavigationService>().GoBackAsync()).Success);
            Assert.Same(root, ((NavigationPage)window.Page).CurrentPage);
            Assert.Equal(PageNavigationSource.Device, PageNavigationService.NavigationSource);
        }
        finally
        {
            events.Unsubscribe(token);
        }
    }

    [Fact]
    public async Task ProgrammaticConfirmation_MarksTheModalOwningWindow()
    {
        var app = CreateBuilder(p => p.RegisterTypes(c => c.RegisterForNavigation<MockPendingPage>())
            .CreateWindow("NavigationPage/MockViewA")).Build();
        var firstWindow = GetWindow(app);
        var root = ((NavigationPage)firstWindow.Page).CurrentPage;
        var navigation = root.GetContainerProvider().Resolve<INavigationService>();
        Assert.True((await navigation.NavigateAsync("MockPendingPage?useModalNavigation=true")).Success);
        var page = Assert.IsType<MockPendingPage>(Assert.Single(firstWindow.Navigation.ModalStack));
        var modalNavigation = page.GetContainerProvider().Resolve<INavigationService>();
        var secondWindow = new PrismWindow("second") { Page = new ContentPage() };

        // Move an already Prism-owned modal to a different window without destroying its scope.
        PageNavigationService.NavigationSource = PageNavigationSource.NavigationService;
        try
        {
            await firstWindow.Navigation.PopModalAsync();
            await secondWindow.Navigation.PushModalAsync(page);
        }
        finally
        {
            PageNavigationService.NavigationSource = PageNavigationSource.Device;
        }

        Assert.Same(secondWindow, page.GetParentWindow());
        var request = modalNavigation.GoBackAsync();
        await page.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Same(page, secondWindow.PendingModalConfirmation);
        Assert.Null(firstWindow.PendingModalConfirmation);
        await secondWindow.Navigation.PopModalAsync();
        await secondWindow.Navigation.PopModalAsync();
        Assert.Same(page, Assert.Single(secondWindow.Navigation.ModalStack));
        Assert.Equal(1, page.Confirmations);
        page.Decision.SetResult(false);
        Assert.False((await request.WaitAsync(TimeSpan.FromSeconds(5))).Success);
        Assert.Null(secondWindow.PendingModalConfirmation);
        Assert.Equal(0, page.NavigatedFrom);
        Assert.Equal(0, page.Destroyed);
        Assert.Empty(modalNavigation.GetPops());
    }
}
