using Prism.Common;
using Prism.Navigation.Xaml;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.Controls;
using Prism.Mvvm;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigationUriPathTests : TestBase
{
    public NavigationUriPathTests(ITestOutputHelper output) : base(output) { }

    [Theory]
    [InlineData("Stack/Welcome/Privacy/Login/Details/End", "/Stack/Welcome/Privacy/Login/Details/End")]
    [InlineData("Shell/Stack/Welcome/Privacy/Login/Details/End", "/Shell/Stack/Welcome/Privacy/Login/Details/End")]
    [InlineData("Tabs?createTab=Inactive&createTab=Stack%2FWelcome%2FPrivacy%2FLogin%2FDetails%2FEnd&selectedTab=Stack%7CWelcome", "/Tabs?selectedTab=Stack%7CWelcome/Privacy/Login/Details/End")]
    [InlineData("Shell/Tabs?createTab=Inactive&createTab=Stack%2FWelcome%2FPrivacy%2FLogin%2FDetails%2FEnd&selectedTab=Stack%7CWelcome", "/Shell/Tabs?selectedTab=Stack%7CWelcome/Privacy/Login/Details/End")]
    [InlineData("Stack/Welcome/Tabs?createTab=Inactive&createTab=OverlayStack%2FPrivacy%2FLogin%2FDetails&selectedTab=OverlayStack%7CPrivacy", "/Stack/Welcome/Tabs?selectedTab=OverlayStack%7CPrivacy/Login/Details")]
    public void RealDeepLinksPreserveAliasedContainerAndPageOrder(string route, string expected)
    {
        var window = CreateMatrixWindow(route);
        Assert.Equal(expected, ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
        Assert.Equal("/" + ViewModelLocator.GetNavigationName(window.Page), ServiceFor(window.Page).GetNavigationUriPath());
    }

    [Theory]
    [InlineData("Stack/Welcome/Privacy/Login", "/Stack/Welcome/Privacy/Login")]
    [InlineData("Shell/Stack/Welcome/Privacy/Login", "/Shell/Stack/Welcome/Privacy/Login")]
    [InlineData("Tabs?createTab=Inactive&createTab=Stack%2FWelcome%2FPrivacy%2FLogin&selectedTab=Stack%7CWelcome", "/Tabs?selectedTab=Stack%7CWelcome/Privacy/Login")]
    [InlineData("Shell/Tabs?createTab=Inactive&createTab=Stack%2FWelcome%2FPrivacy%2FLogin&selectedTab=Stack%7CWelcome", "/Shell/Tabs?selectedTab=Stack%7CWelcome/Privacy/Login")]
    public async Task RealModalStackAboveComposedRootsCanBePoppedWithoutChangingUnderlyingScopes(string route, string rootPath)
    {
        var window = CreateMatrixWindow(route);
        var underlying = window.CurrentPage;
        var underlyingService = ServiceFor(underlying);
        await Navigate(underlyingService, "OverlayStart?useModalNavigation=true&secret=not-retained");
        var first = window.CurrentPage;
        var firstService = ServiceFor(first);
        Assert.Equal(rootPath + "/OverlayStart?useModalNavigation=true", firstService.GetNavigationUriPath());
        await Navigate(firstService, "OverlayStack/OverlayStart/OverlayEnd");
        var topService = ServiceFor(window.CurrentPage);
        var modalPath = rootPath + "/OverlayStart?useModalNavigation=true/OverlayStack?useModalNavigation=true/OverlayStart/OverlayEnd";
        Assert.Equal(modalPath, topService.GetNavigationUriPath());
        Assert.Equal(rootPath, underlyingService.GetNavigationUriPath());
        Assert.Equal(rootPath + "/OverlayStart?useModalNavigation=true", firstService.GetNavigationUriPath());
        Assert.Equal(2, window.Navigation.ModalStack.Count);

        await Back(topService);
        Assert.Equal(modalPath[..modalPath.LastIndexOf('/')], ServiceFor(window.CurrentPage).GetNavigationUriPath());
        await Back(ServiceFor(window.CurrentPage));
        Assert.Same(first, window.CurrentPage);
        Assert.Equal(rootPath + "/OverlayStart?useModalNavigation=true", firstService.GetNavigationUriPath());
        await Back(firstService);
        Assert.Same(underlying, window.CurrentPage);
        Assert.Equal(rootPath, underlyingService.GetNavigationUriPath());
        Assert.Empty(window.Navigation.ModalStack);
    }

    [Fact]
    public async Task RealTabSelectionAndDetailReplacementReflectLiveStateAndScopedHistory()
    {
        var window = CreateMatrixWindow("Shell/Tabs?createTab=Stack%2FWelcome%2FPrivacy&createTab=OverlayStack%2FSecondRoot%2FDetails&selectedTab=Stack%7CWelcome");
        var tabs = Assert.IsType<TabbedPage>(Assert.IsType<MockHome>(window.Page).Detail);
        var firstNavigation = Assert.IsType<PrismNavigationPage>(tabs.CurrentPage);
        var firstService = ServiceFor(firstNavigation.CurrentPage);
        Assert.Equal("/Shell/Tabs?selectedTab=Stack%7CWelcome/Privacy", firstService.GetNavigationUriPath());
        var result = await firstService.SelectTabAsync("OverlayStack|SecondRoot", "End");
        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal("/Shell/Tabs?selectedTab=OverlayStack%7CSecondRoot/Details/End", ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
        Assert.Equal("/Shell/Tabs?selectedTab=Stack%7CWelcome/Privacy", firstService.GetNavigationUriPath());

        await Navigate(ServiceFor(window.Page), "Stack/Welcome/Login");
        Assert.Equal("/Shell/Stack/Welcome/Login", ServiceFor(window.CurrentPage).GetNavigationUriPath());
        Assert.Equal(string.Empty, firstService.GetNavigationUriPath());
    }

    [Fact]
    public async Task ReturnToRootAndRepeatedPushesUseCurrentHistoryNotTheOriginalDeepLink()
    {
        var window = CreateMatrixWindow("Stack/Welcome/Privacy/Login/Details/End");
        var oldService = ServiceFor(CurrentLeaf(window));
        var result = await oldService.GoBackToRootAsync();
        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal("/Stack/Welcome", ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
        Assert.Equal(string.Empty, oldService.GetNavigationUriPath());
        await Navigate(ServiceFor(CurrentLeaf(window)), "Login/Privacy/End");
        Assert.Equal("/Stack/Welcome/Login/Privacy/End", ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
        await Back(ServiceFor(CurrentLeaf(window)));
        Assert.Equal("/Stack/Welcome/Login/Privacy", ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
    }

    [Fact]
    public async Task FailedAndVetoedNavigationDoesNotInventHistory()
    {
        var window = CreateMatrixWindow("Shell/Stack/Welcome/Privacy/Login");
        var service = ServiceFor(CurrentLeaf(window));
        var model = Assert.IsType<MockViewAViewModel>(CurrentLeaf(window).BindingContext);
        model.StopNavigation = true;
        var result = await service.NavigateAsync("End");
        Assert.False(result.Success);
        Assert.Equal("/Shell/Stack/Welcome/Privacy/Login", service.GetNavigationUriPath());
        model.StopNavigation = false;
        result = await service.NavigateAsync("MissingRegistration");
        Assert.False(result.Success);
        Assert.Equal("/Shell/Stack/Welcome/Privacy/Login", service.GetNavigationUriPath());
    }

    [Fact]
    public void RealAliasSegmentsAreUriEscaped()
    {
        var window = CreateMatrixWindow("Stack/Step%20%2B%20More/Alias%20%26%20More");
        Assert.Equal("/Stack/Step%20%2B%20More/Alias%20%26%20More", ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
    }

    public static IEnumerable<object[]> ModalContainerCases()
    {
        var roots = new[]
        {
            ("Stack/Welcome/Login", "/Stack/Welcome/Login"),
            ("Shell/Stack/Welcome/Login", "/Shell/Stack/Welcome/Login"),
            ("Tabs?createTab=Inactive&createTab=Stack%2FWelcome%2FLogin&selectedTab=Stack%7CWelcome", "/Tabs?selectedTab=Stack%7CWelcome/Login"),
            ("Shell/Tabs?createTab=Inactive&createTab=Stack%2FWelcome%2FLogin&selectedTab=Stack%7CWelcome", "/Shell/Tabs?selectedTab=Stack%7CWelcome/Login")
        };
        var modals = new[]
        {
            ("OverlayStack?useModalNavigation=true/OverlayStart/OverlayEnd", "/OverlayStack?useModalNavigation=true/OverlayStart/OverlayEnd", "/OverlayStack?useModalNavigation=true"),
            ("Shell?useModalNavigation=true/OverlayStack/OverlayStart/OverlayEnd", "/Shell?useModalNavigation=true/OverlayStack/OverlayStart/OverlayEnd", "/Shell?useModalNavigation=true"),
            ("Tabs?useModalNavigation=true&createTab=Inactive&createTab=OverlayStack%2FOverlayStart%2FOverlayEnd&selectedTab=OverlayStack%7COverlayStart", "/Tabs?selectedTab=OverlayStack%7COverlayStart&useModalNavigation=true/OverlayEnd", "/Tabs?useModalNavigation=true")
        };
        foreach (var (route, path) in roots)
            foreach (var (modalRoute, modalPath, modalRootPath) in modals)
                yield return new object[] { route, path, modalRoute, modalPath, modalRootPath };
    }

    [Theory]
    [MemberData(nameof(ModalContainerCases))]
    public async Task ModalContainersAboveEachRootPreserveBoundariesAndScopedPaths(string route, string path, string modalRoute, string modalPath, string modalRootPath)
    {
        var window = CreateMatrixWindow(route);
        var underlying = CurrentLeaf(window);
        var service = ServiceFor(underlying);
        await Navigate(service, modalRoute);
        var modalRoot = Assert.Single(window.Navigation.ModalStack);
        var modalRootService = ServiceFor(modalRoot);
        Assert.Equal(path + modalPath, ServiceFor(CurrentLeaf(window)).GetNavigationUriPath());
        Assert.Equal(path + modalRootPath, modalRootService.GetNavigationUriPath());
        Assert.Equal(path, service.GetNavigationUriPath());
        var result = await modalRootService.GoBackAsync(new NavigationParameters { { KnownNavigationParameters.UseModalNavigation, true } });
        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(window.Navigation.ModalStack);
        Assert.Equal(path, service.GetNavigationUriPath());
        Assert.Equal(string.Empty, modalRootService.GetNavigationUriPath());
    }

    [Fact]
    public async Task ActualAliasWinsWhenOnePageTypeHasManyNamesAndQueriesAreExcluded()
    {
        var window = CreateMatrixWindow("Stack/Welcome?secret=a%2Fb/Privacy?number=42/Login?query=a%26b/Details/End");
        var navigation = Assert.IsType<PrismNavigationPage>(window.Page);
        Assert.All(navigation.Navigation.NavigationStack, page => Assert.IsType<MockViewA>(page));
        Assert.Equal("/Stack/Welcome/Privacy/Login/Details/End", ServiceFor(window.CurrentPage).GetNavigationUriPath());
        Assert.Equal("/Stack/Welcome/Privacy", ServiceFor(navigation.Navigation.NavigationStack[1]).GetNavigationUriPath());
        await Navigate(ServiceFor(window.CurrentPage), "/OverlayStack/Login/Welcome");
        Assert.Equal("/OverlayStack/Login/Welcome", ServiceFor(window.CurrentPage).GetNavigationUriPath());
    }

    private PrismWindow CreateMatrixWindow(string route)
    {
        var app = CreateBuilder(prism => prism.RegisterTypes(container =>
        {
            container.RegisterForNavigation<PrismNavigationPage>("Stack");
            container.RegisterForNavigation<PrismNavigationPage>("OverlayStack");
            container.RegisterForNavigation<TabbedPage>("Tabs");
            container.RegisterForNavigation<MockHome>("Shell");
            container.RegisterForNavigation<MockViewB, MockViewBViewModel>("SecondRoot");
            foreach (var name in new[] { "Welcome", "Privacy", "Login", "Details", "End", "Inactive", "OverlayStart", "OverlayEnd", "Step + More", "Alias & More" })
                container.RegisterForNavigation<MockViewA, MockViewAViewModel>(name);
        }).CreateWindow(route)).Build();
        return GetWindow(app);
    }

    private static INavigationService ServiceFor(Page page) => page.GetContainerProvider().Resolve<INavigationService>();

    private static Page CurrentLeaf(PrismWindow window) =>
        MvvmHelpers.GetTarget(window.Navigation.ModalStack.LastOrDefault() ?? window.Page);

    private static async Task Navigate(INavigationService service, string route)
    {
        var result = await service.NavigateAsync(route);
        Assert.True(result.Success, result.Exception?.ToString());
    }

    private static async Task Back(INavigationService service)
    {
        var result = await service.GoBackAsync();
        Assert.True(result.Success, result.Exception?.ToString());
    }

    [Fact]
    public async Task ConstructorInjectedServiceUsesActualScopedStackAfterNavigation()
    {
        var app = CreateBuilder(prism => prism.RegisterTypes(container =>
                container.RegisterForNavigation<MockViewA, NavigationUriPathViewModel>("Login"))
            .CreateWindow("NavigationPage/MockHome/Login"))
            .Build();
        var window = GetWindow(app);
        var navigation = Assert.IsAssignableFrom<NavigationPage>(window.Page);
        var login = Assert.IsType<NavigationUriPathViewModel>(navigation.CurrentPage.BindingContext);
        Assert.Equal(string.Empty, login.ConstructorPath);
        Assert.Equal("/NavigationPage/MockHome/Login", login.NavigationService.GetNavigationUriPath());

        var result = await login.NavigationService.NavigateAsync("MockViewB");
        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal("/NavigationPage/MockHome/Login", login.NavigationService.GetNavigationUriPath());
        var current = navigation.CurrentPage.GetContainerProvider().Resolve<INavigationService>();
        Assert.Equal("/NavigationPage/MockHome/Login/MockViewB", current.GetNavigationUriPath());
        result = await current.GoBackAsync();
        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Equal("/NavigationPage/MockHome/Login", login.NavigationService.GetNavigationUriPath());
    }
}
