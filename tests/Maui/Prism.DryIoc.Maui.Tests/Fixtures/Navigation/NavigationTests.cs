using Prism.Common;
using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.Navigation;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Navigation.Xaml;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigationTests : TestBase
{
    public NavigationTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("NavigationPage/MockViewA/MockViewB/MockViewC")]
    [InlineData("MockHome/NavigationPage/MockViewA")]
    public void PagesInjectScopedInstanceOfIPageAccessor(string uri)
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow(uri))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page;

        if(rootPage is FlyoutPage flyoutPage)
        {
            TestPage(flyoutPage);
            rootPage = flyoutPage.Detail;
        }

        TestPage(rootPage!);

        foreach (var page in rootPage!.Navigation.NavigationStack)
        {
            TestPage(page);
        }
    }

    [Fact]
    public async Task ViewModelLocator_Forced_SetsContainer_ResolvedViewModel()
    {
        var mauiApp = CreateBuilder(prism => prism
            .RegisterTypes(c => c.RegisterForNavigation<ForcedView>())
            .CreateWindow("ForcedView"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsType<ForcedView>(window.Page);
        Assert.IsType<ForcedViewModel>(window.Page.BindingContext);

        var viewModel = (ForcedViewModel)window.Page.BindingContext;
        Assert.NotNull(viewModel.Page);
        Assert.IsType<ForcedView>(viewModel.Page);
    }

    [Fact]
    public async Task AddsPageFromRelativeURI()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as NavigationPage;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        var currentPage = rootPage.CurrentPage;
        Assert.IsType<MockViewA>(currentPage);
        TestPage(currentPage);
        var container = currentPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Single(rootPage.Navigation.NavigationStack);
        await navService.NavigateAsync("MockViewB");
        Assert.IsType<MockViewB>(rootPage.CurrentPage);
        TestPage(rootPage.CurrentPage);
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);

        var pushes = navService.GetPushes();
        Assert.Single(pushes);
        Assert.Equal(currentPage, pushes[0].CurrentPage);
        Assert.Equal(rootPage.CurrentPage, pushes[0].Page);
        Assert.Null(pushes[0].Animated);
    }

    [Fact]
    public async Task RelativeNavigation_RemovesPage_AndNavigates()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as NavigationPage;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        var currentPage = rootPage.CurrentPage;
        Assert.IsType<MockViewB>(currentPage);
        TestPage(currentPage);
        var container = currentPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
        await navService.NavigateAsync("../MockViewC");
        Assert.IsType<MockViewC>(rootPage.CurrentPage);
        TestPage(rootPage.CurrentPage);
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
    }

    [Fact(Timeout = 5000)]
    public async Task Issue3047_RelativeNavigation_RemovesPage_AndGoBack()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as NavigationPage;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        var currentPage = rootPage.CurrentPage;
        Assert.IsType<MockViewB>(currentPage);
        TestPage(currentPage);
        var container = currentPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
        await navService.NavigateAsync("../");
        Assert.IsType<MockViewA>(rootPage.CurrentPage);
        TestPage(rootPage.CurrentPage);
        Assert.Single(rootPage.Navigation.NavigationStack);
    }

    [Fact]
    public async Task AbsoluteNavigation_ResetsWindowPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as MockViewA;
        Assert.NotNull(rootPage);
        var container = rootPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        var result = await navService.NavigateAsync("/MockViewB");
        Assert.True(result.Success);
        Assert.NotEqual(rootPage, window.Page);
    }

    [Fact]
    public async Task AddsModalPageFromRelativeURI()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as MockViewA;
        Assert.NotNull(rootPage);
        Assert.IsType<MockViewA>(rootPage);
        var container = rootPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Empty(rootPage.Navigation.ModalStack);
        var result = await navService.NavigateAsync("MockViewB");
        Assert.True(result.Success);
        Assert.Single(rootPage.Navigation.ModalStack);
        Assert.IsType<MockViewB>(rootPage.Navigation.ModalStack.Last());
    }

    [Fact]
    public async Task FlyoutRelativeNavigation_RemovesPage_AndNavigatesNotModally()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("MockHome/NavigationPage/MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as MockHome;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        Assert.NotNull(rootPage.Detail);
        var detailPage = rootPage.Detail as NavigationPage;
        Assert.NotNull(detailPage);
        TestPage(detailPage);
        var currentPage = detailPage.CurrentPage;
        Assert.IsType<MockViewA>(currentPage);
        TestPage(currentPage);
        var navService = Prism.Navigation.Xaml.Navigation.GetNavigationService(rootPage);
        Assert.Empty(rootPage.Navigation.ModalStack);
        var result = await navService.NavigateAsync("./NavigationPage/MockViewB");
        Assert.True(result.Success);
        Assert.Empty(rootPage.Navigation.ModalStack);
        Assert.NotNull(rootPage.Detail);
        detailPage = rootPage.Detail as NavigationPage;
        Assert.NotNull(detailPage);
        TestPage(detailPage);
        currentPage = detailPage.CurrentPage;
        Assert.IsType<MockViewB>(currentPage);
        TestPage(currentPage);
    }

    [Fact(Skip = "Blocked by dotnet/maui/issues/8157")]
    public async Task RelativeNavigation_RemovesPage_AndNavigatesModally()
    {
        Exception startupEx = null;
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("MockViewA/MockViewB", ex =>
        {
            startupEx = ex;
        }))
            .Build();
        Assert.Null(startupEx);
        var window = GetWindow(mauiApp);

        var rootPage = window.Page as MockViewA;
        Assert.NotNull(rootPage);
        TestPage(rootPage);
        var currentPage = rootPage.Navigation.ModalStack.Last();
        Assert.IsType<MockViewB>(currentPage);
        TestPage(currentPage);
        var container = currentPage.GetContainerProvider();
        var navService = container.Resolve<INavigationService>();
        Assert.Equal(2, rootPage.Navigation.ModalStack.Count);
        await navService.NavigateAsync("../MockViewC");
        var viewC = window.Page.Navigation.ModalStack.Last();
        Assert.IsType<MockViewC>(viewC);
        Assert.Equal(2, rootPage.Navigation.ModalStack.Count);
    }

    [Fact]
    public async Task GoBack_Name_PopsToSpecifiedView()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewB/MockViewC/MockViewD/MockViewE"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navigationPage = (NavigationPage)window.Page;

        Assert.IsType<MockViewA>(navigationPage.RootPage);
        Assert.IsType<MockViewE>(navigationPage.CurrentPage);

        var result = await navigationPage.CurrentPage.GetContainerProvider()
            .Resolve<INavigationService>()
            .GoBackAsync("MockViewC");

        Assert.True(result.Success);

        Assert.IsType<MockViewC>(navigationPage.CurrentPage);
    }

    [Fact]
    public async Task GoBack_ViewModel_PopsToSpecifiedView()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewB/MockViewC/MockViewD/MockViewE"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navigationPage = (NavigationPage)window.Page;

        Assert.IsType<MockViewA>(navigationPage.RootPage);
        Assert.IsType<MockViewE>(navigationPage.CurrentPage);

        var result = await navigationPage.CurrentPage.GetContainerProvider()
            .Resolve<INavigationService>()
            .CreateBuilder()
            .GoBackAsync<MockViewCViewModel>();

        Assert.True(result.Success);

        Assert.IsType<MockViewC>(navigationPage.CurrentPage);
    }

    [Fact]
    public async Task GoBack_Issue2232()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navigationPage = (NavigationPage)window.Page;

        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.NavigateTo("MockViewB"));
        Assert.IsType<MockViewB>(navigationPage.CurrentPage);
        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.NavigateTo("MockViewC"));
        Assert.IsType<MockViewC>(navigationPage.CurrentPage);
        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.NavigateTo("MockViewD/MockViewE"));
        Assert.IsType<MockViewE>(navigationPage.CurrentPage);

        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.GoBack());
        Assert.IsType<MockViewD>(navigationPage.CurrentPage);
        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.GoBack());
        Assert.IsType<MockViewC>(navigationPage.CurrentPage);
        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.GoBack());
        Assert.IsType<MockViewB>(navigationPage.CurrentPage);
        await MvvmHelpers.InvokeViewAndViewModelActionAsync<MockViewModelBase>(navigationPage.CurrentPage, x => x.GoBack());
        Assert.IsType<MockViewA>(navigationPage.CurrentPage);
    }

    [Fact]
    public async Task GoBack_Name_PopsToSpecifiedViewWithoutPoppingEachPage()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewB/MockViewC/MockViewD/MockViewE"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navigationPage = (NavigationPage)window.Page;
        var withoutPoppingPage = (MockViewD)navigationPage.Navigation.NavigationStack.First(p => ViewModelLocator.GetNavigationName(p) == nameof(MockViewD));
        var withoutPoppingPageVm = (MockViewModelBase)withoutPoppingPage.BindingContext;

        Assert.IsType<MockViewA>(navigationPage.RootPage);
        Assert.IsType<MockViewE>(navigationPage.CurrentPage);

        var result = await navigationPage.CurrentPage.GetContainerProvider()
            .Resolve<INavigationService>()
            .GoBackAsync("MockViewC");

        Assert.True(result.Success);

        Assert.IsType<MockViewC>(navigationPage.CurrentPage);

        // In the GoBackAsync method, the OnNavigatedTo method is not called for pages that are not popped.
        Assert.True(withoutPoppingPageVm.Actions.Last() == nameof(MockViewModelBase.OnNavigatedFrom));
    }

    [Fact]
    public async Task GoBack_Name_PopsToSpecifiedViewWithoutPoppingEachPageOfLimitation()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("NavigationPage/MockViewA/MockViewA/MockViewB/MockViewC/MockViewD/MockViewE"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navigationPage = (NavigationPage)window.Page;

        Assert.IsType<MockViewA>(navigationPage.RootPage);
        Assert.IsType<MockViewE>(navigationPage.CurrentPage);

        var result = await navigationPage.CurrentPage.GetContainerProvider()
            .Resolve<INavigationService>()
            .GoBackAsync("MockViewA");

        Assert.True(result.Success);

        Assert.IsType<MockViewA>(navigationPage.CurrentPage);

        // If there are two instances of MockViewA, it will return to the instance closest to the current page.
        // Therefore, the current modal stack will be in the state of NavigationPage/MockViewA/MockViewA.
        Assert.Equal(2, navigationPage.Navigation.NavigationStack.Count);
    }

    [Fact]
    public async Task TabbedPage_SelectTabSets_CurrentTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("TabbedPage?createTab=MockViewA&createTab=MockViewB&selectedTab=MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbedPage = (TabbedPage)window.Page;
        Assert.NotNull(tabbedPage);
        Assert.IsType<MockViewB>(tabbedPage.CurrentPage);
    }

    [Fact]
    public async Task TabbedPage_SelectTab_SetsCurrentTab_WithNavigationPageTab()
    {
        var mauiApp = CreateBuilder(prism => prism.CreateWindow("TabbedPage?createTab=NavigationPage%2FMockViewA&createTab=NavigationPage%2FMockViewB&selectedTab=NavigationPage|MockViewB"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbedPage = (TabbedPage)window.Page;
        Assert.NotNull(tabbedPage);
        var navPage = tabbedPage.CurrentPage as NavigationPage;
        Assert.NotNull(navPage);
        Assert.IsType<MockViewB>(navPage.CurrentPage);
    }

    [Fact]
    public async Task TabbedPage_SelectsNewTab()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(nav => nav.CreateBuilder()
                .AddTabbedSegment(s => s.CreateTab("MockViewA")
                                                       .CreateTab("MockViewB")
                                                       .CreateTab("MockViewC"))
                .NavigateAsync()))
            .Build();
        var window = GetWindow(mauiApp);
        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbed = window.Page as TabbedPage;

        Assert.NotNull(tabbed);

        Assert.IsType<MockViewA>(tabbed.CurrentPage);
        var mockViewA = tabbed.CurrentPage;
        var mockViewANav = Prism.Navigation.Xaml.Navigation.GetNavigationService(mockViewA);

        await mockViewANav.SelectTabAsync("MockViewB");

        Assert.IsNotType<MockViewA>(tabbed.CurrentPage);
        Assert.IsType<MockViewB>(tabbed.CurrentPage);
    }

    [Fact]
    public async Task TabbedPage_SelectsNewTab_WithNavigationParameters()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(nav => nav.CreateBuilder()
                .AddTabbedSegment(s => s.CreateTab("MockViewA")
                                                       .CreateTab("MockViewB")
                                                       .CreateTab("MockViewC"))
                .NavigateAsync()))
            .Build();
        var window = GetWindow(mauiApp);
        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbed = window.Page as TabbedPage;

        Assert.NotNull(tabbed);

        Assert.IsType<MockViewA>(tabbed.CurrentPage);
        var mockViewA = tabbed.CurrentPage;
        var mockViewANav = Prism.Navigation.Xaml.Navigation.GetNavigationService(mockViewA);

        var expectedMessage = nameof(TabbedPage_SelectsNewTab_WithNavigationParameters);
        await mockViewANav.SelectTabAsync("MockViewB", new NavigationParameters { { "Message", expectedMessage } });

        Assert.IsNotType<MockViewA>(tabbed.CurrentPage);
        Assert.IsType<MockViewB>(tabbed.CurrentPage);

        var viewModel = tabbed.CurrentPage.BindingContext as MockViewBViewModel;
        Assert.Equal(expectedMessage, viewModel?.Message);
    }

    [Fact]
    public async Task NavigationPage_DoesNotHave_MauiPage_AsRootPage()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow("NavigationPage/MockViewA"))
            .Build();
        var window = GetWindow(mauiApp);

        Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var navPage = window.Page as NavigationPage;

        Assert.NotNull(navPage);
        Assert.IsNotType<Page>(navPage.RootPage);
        Assert.IsType<MockViewA>(navPage.RootPage);

        Assert.Same(navPage.RootPage, navPage.CurrentPage);
    }

    [Fact]
    public async Task NavigationPage_UsesRootPageTitle_WithTabbedParent()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(n => n.CreateBuilder()
                .AddTabbedSegment(s => s
                    .CreateTab(t => t.AddNavigationPage()
                                                .AddSegment("MockViewA")))
                .NavigateAsync()))
            .Build();
        var window = GetWindow(mauiApp);
        Assert.IsAssignableFrom<TabbedPage>(window.Page);
        var tabbed = window.Page as TabbedPage;

        Assert.NotNull(tabbed);

        Assert.Single(tabbed.Children);
        var child = tabbed.Children[0];
        Assert.IsAssignableFrom<NavigationPage>(child);
        var navPage = child as NavigationPage;

        Assert.Equal(navPage.Title, navPage.RootPage.Title);
        Assert.Equal(MockViewA.ExpectedTitle, navPage.Title);
    }

    [Fact]
    public async Task Navigation_HasDefault_AnimatedIsNull()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(n => n.CreateBuilder()
                .AddNavigationPage()
                .AddSegment("MockViewA")))
            .Build();
        var window = GetWindow(mauiApp);
        var navigationPage = (NavigationPage)window.Page;
        var rootPage = navigationPage.RootPage;

        var navigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(rootPage);
        var pushes = navigationService.GetPushes();

        Assert.Empty(pushes);

        var result = await navigationService.NavigateAsync("MockViewB");

        Assert.True(result.Success);
        Assert.Single(pushes);
        var push = pushes[0];

        Assert.IsType<MockViewA>(push.CurrentPage);
        Assert.IsType<MockViewB>(push.Page);

        Assert.Equal(navigationPage.RootPage, push.CurrentPage);
        Assert.Equal(navigationPage.CurrentPage, push.Page);

        Assert.Null(push.Animated);
    }

    [Fact]
    public async Task Navigation_Animation_IsTrue()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(n => n.CreateBuilder()
                .AddNavigationPage()
                .AddSegment("MockViewA")))
            .Build();
        var window = GetWindow(mauiApp);
        var navigationPage = (NavigationPage)window.Page;
        var rootPage = navigationPage.RootPage;

        var navigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(rootPage);
        var pushes = navigationService.GetPushes();

        Assert.Empty(pushes);

        var result = await navigationService.NavigateAsync($"MockViewB?{KnownNavigationParameters.Animated}=true");

        Assert.True(result.Success);
        Assert.Single(pushes);
        var push = pushes[0];

        Assert.IsType<MockViewA>(push.CurrentPage);
        Assert.IsType<MockViewB>(push.Page);

        Assert.Equal(navigationPage.RootPage, push.CurrentPage);
        Assert.Equal(navigationPage.CurrentPage, push.Page);

        Assert.True(push.Animated);
    }

    [Fact]
    public async Task Navigation_Animation_IsFalse()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(n => n.CreateBuilder()
                .AddNavigationPage()
                .AddSegment("MockViewA")))
            .Build();
        var window = GetWindow(mauiApp);
        var navigationPage = (NavigationPage)window.Page;
        var rootPage = navigationPage.RootPage;

        var navigationService = Prism.Navigation.Xaml.Navigation.GetNavigationService(rootPage);
        var pushes = navigationService.GetPushes();

        Assert.Empty(pushes);

        var result = await navigationService.NavigateAsync($"MockViewB?{KnownNavigationParameters.Animated}=false");

        Assert.True(result.Success);
        Assert.Single(pushes);
        var push = pushes[0];

        Assert.IsType<MockViewA>(push.CurrentPage);
        Assert.IsType<MockViewB>(push.Page);

        Assert.Equal(navigationPage.RootPage, push.CurrentPage);
        Assert.Equal(navigationPage.CurrentPage, push.Page);

        Assert.False(push.Animated);
    }

    [Theory]
    [InlineData("MockViewA", "MockViewB", null)]
    [InlineData("NavigationPage/MockViewA", "MockViewB?useModalNavigation=true", true)]
    public async Task PushesModally(string startUri, string requestUri, bool? expectedUseModal)
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(n => n.NavigateAsync(startUri)))
            .Build();
        var window = GetWindow(mauiApp);
        var page = window.Page;
        if (page is NavigationPage navPage)
            page = navPage.RootPage;

        var navService = Prism.Navigation.Xaml.Navigation.GetNavigationService(page);

        var result = await navService.NavigateAsync(requestUri);
        Assert.True(result.Success);

        var pushes = navService.GetPushes();
        Assert.Single(pushes);
        var push = pushes[0];

        var parameters = UriParsingHelper.GetSegmentParameters(requestUri);
        bool? useModalNavigation = null;
        if (parameters.TryGetValue<bool>(KnownNavigationParameters.UseModalNavigation, out var parameterModal))
            useModalNavigation = parameterModal;

        Assert.Equal(expectedUseModal, push.UseModalNavigation);
        Assert.True(PageNavigationService.UseModalNavigation(push.CurrentPage, useModalNavigation));
    }

    [Fact]
    public async Task PushesModally_From_NavigationParameters()
    {
        var mauiApp = CreateBuilder(prism => prism
            .CreateWindow(n => n.NavigateAsync("NavigationPage/MockViewA")))
            .Build();
        var window = GetWindow(mauiApp);
        var page = window.Page;
        if (page is NavigationPage navPage)
            page = navPage.RootPage;

        var navService = Prism.Navigation.Xaml.Navigation.GetNavigationService(page);

        var result = await navService.NavigateAsync("MockViewB", new NavigationParameters { { KnownNavigationParameters.UseModalNavigation, true } });
        Assert.True(result.Success);

        var pushes = navService.GetPushes();
        Assert.Single(pushes);
        var push = pushes[0];

        Assert.True(push.UseModalNavigation);
    }

    private static void TestPage(Page page)
    {
        Assert.NotNull(page.BindingContext);
        if(page.Parent is not null)
        {
            Assert.False(page.BindingContext == page);
            Assert.False(page.BindingContext == page.Parent);
            Assert.False(page.BindingContext == page.Parent.BindingContext);
        }

        if (page is NavigationPage)
        {
            Assert.IsType<PrismNavigationPage>(page);
            return;
        }

        var viewModel = page.BindingContext as MockViewModelBase;
        Assert.NotNull(viewModel);
        Assert.Same(page, viewModel!.Page);
    }
}
