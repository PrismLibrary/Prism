using Prism.Common;
using Prism.Navigation.Xaml;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;
using Prism.DryIoc.Maui.Tests.Mocks.Views;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class NavigationUriPathTests : TestBase
{
    public NavigationUriPathTests(ITestOutputHelper output) : base(output) { }

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
