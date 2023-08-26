using System.Linq;
using System.Web;
using Microsoft.Maui.Controls;
using Moq;
using Prism.Common;
using Prism.Maui.Tests.Mocks.Ioc;
using Prism.Maui.Tests.Navigation.Mocks.Views;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class NavigationBuilderFixture
{
    [Fact]
    public void GeneratesRelativeUriWithSingleSegment()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .Uri;

        Assert.Equal("ViewA", uri.ToString());
    }

    [Fact]
    public void GeneratesRelativeUriWithMultipleSegments()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .AddSegment("ViewB")
            .AddSegment("ViewC")
            .Uri;

        Assert.Equal("ViewA/ViewB/ViewC", uri.ToString());
    }

    [Fact]
    public void GeneratesAbsoluteUriWithSingleSegment()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .UseAbsoluteNavigation()
            .Uri;

        Assert.Equal("http://localhost/ViewA", uri.ToString());
    }

    [Fact]
    public void GeneratesAbsoluteUriWithMultipleSegments()
    {
        var uri = Mock.Of<INavigationService>()
            .CreateBuilder()
            .AddSegment("ViewA")
            .AddSegment("ViewB")
            .AddSegment("ViewC")
            .UseAbsoluteNavigation()
            .Uri;

        Assert.Equal("http://localhost/ViewA/ViewB/ViewC", uri.ToString());
    }

    [Fact]
    public void GeneratesTabbedPageUriWithCreatedTabs()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<TabbedPage>();

        var navigationService = new Mock<INavigationService>();
        navigationService
            .As<IRegistryAware>()
            .Setup(x => x.Registry)
            .Returns(container.Resolve<NavigationRegistry>());
        var uri = navigationService.Object
            .CreateBuilder()
            .AddTabbedSegment(b =>
            {
                b.CreateTab("ViewA")
                 .CreateTab("ViewB")
                 .CreateTab("ViewC");
            })
            .Uri;

        Assert.Equal("TabbedPage?createTab=ViewA&createTab=ViewB&createTab=ViewC", uri.ToString());
    }

    [Fact]
    public void GeneratesTabbedPageUriWithCreatedTabsWithParameters()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<TabbedPage>();

        var navigationService = new Mock<INavigationService>();
        navigationService
            .As<IRegistryAware>()
            .Setup(x => x.Registry)
            .Returns(container.Resolve<NavigationRegistry>());
        var uri = navigationService.Object
            .CreateBuilder()
            .AddTabbedSegment(b =>
            {
                b.CreateTab("ViewA", t => t.AddParameter("id", 5))
                 .CreateTab("ViewB", t => t.AddParameter("foo", "bar"))
                 .CreateTab("ViewC");
            })
            .Uri;

        Assert.Equal("TabbedPage?createTab=ViewA%3Fid%3D5&createTab=ViewB%3Ffoo%3Dbar&createTab=ViewC", uri.ToString());

        var parameters = UriParsingHelper.GetSegmentParameters(uri.ToString());
        Assert.Equal(3, parameters.Count);
        Assert.True(parameters.All(x => x.Key == KnownNavigationParameters.CreateTab));

        Assert.Contains(parameters, x => HttpUtility.UrlDecode(x.Value.ToString()) == "ViewA?id=5");
        Assert.Contains(parameters, x => HttpUtility.UrlDecode(x.Value.ToString()) == "ViewB?foo=bar");
    }

    [Fact]
    public void GeneratesCustomTabbedPageUriWithCreatedTabs()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<TabbedPage>();
        container.RegisterForNavigation<TabbedPageEmptyMock>();

        var navigationService = new Mock<INavigationService>();
        navigationService
            .As<IRegistryAware>()
            .Setup(x => x.Registry)
            .Returns(container.Resolve<NavigationRegistry>());
        var uri = navigationService.Object
            .CreateBuilder()
            .AddTabbedSegment("TabbedPageEmptyMock", b =>
            {
                b.CreateTab("ViewA")
                 .CreateTab("ViewB")
                 .CreateTab("ViewC");
            })
            .Uri;

        Assert.Equal("TabbedPageEmptyMock?createTab=ViewA&createTab=ViewB&createTab=ViewC", uri.ToString());
    }

    [Fact]
    public void GeneratesCustomTabbedPageUriWithCreatedTabsWithParameters()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<TabbedPage>();
        container.RegisterForNavigation<TabbedPageEmptyMock>();

        var navigationService = new Mock<INavigationService>();
        navigationService
            .As<IRegistryAware>()
            .Setup(x => x.Registry)
            .Returns(container.Resolve<NavigationRegistry>());
        var uri = navigationService.Object
            .CreateBuilder()
            .AddTabbedSegment("TabbedPageEmptyMock", b =>
            {
                b.CreateTab("ViewA", t => t.AddParameter("id", 5))
                 .CreateTab("ViewB", t => t.AddParameter("foo", "bar"))
                 .CreateTab("ViewC");
            })
            .Uri;

        Assert.Equal("TabbedPageEmptyMock?createTab=ViewA%3Fid%3D5&createTab=ViewB%3Ffoo%3Dbar&createTab=ViewC", uri.ToString());

        var parameters = UriParsingHelper.GetSegmentParameters(uri.ToString());
        Assert.Equal(3, parameters.Count);
        Assert.True(parameters.All(x => x.Key == KnownNavigationParameters.CreateTab));

        Assert.Contains(parameters, x => HttpUtility.UrlDecode(x.Value.ToString()) == "ViewA?id=5");
        Assert.Contains(parameters, x => HttpUtility.UrlDecode(x.Value.ToString()) == "ViewB?foo=bar");
    }

    [Fact]
    public void GeneratesDeepLinkTabCreation()
    {
        var container = new TestContainer();
        container.RegisterForNavigation<TabbedPage>();
        container.RegisterForNavigation<NavigationPage>();

        var navigationService = new Mock<INavigationService>();
        navigationService
            .As<IRegistryAware>()
            .Setup(x => x.Registry)
            .Returns(container.Resolve<NavigationRegistry>());
        var uri = navigationService.Object
            .CreateBuilder()
            .AddTabbedSegment(b =>
                b.CreateTab(t => t.AddNavigationPage()
                    .AddSegment("ViewA")
                    .AddSegment("ViewB", s => s.AddParameter("id", 5))
                    .AddSegment("ViewC"))
                 .CreateTab("ViewD"))
            .Uri;

        Assert.Equal("TabbedPage?createTab=NavigationPage%2FViewA%2FViewB%3Fid%3D5%2FViewC&createTab=ViewD", uri.ToString());

        var parameters = UriParsingHelper.GetSegmentParameters(uri.ToString());
        Assert.Equal(2, parameters.Count);
        Assert.Contains(parameters, x => HttpUtility.UrlDecode(x.Value.ToString()) == "NavigationPage/ViewA/ViewB?id=5/ViewC");
    }
}
