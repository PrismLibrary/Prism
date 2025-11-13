using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Moq;
using Prism.Common;
using Prism.Maui.Tests.Mocks.Ioc;
using Prism.Maui.Tests.Mocks.ViewModels;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Navigation.Xaml;
using System;

namespace Prism.Maui.Tests.Fixtures.Navigation.Xaml;

public class NavigateToExtensionFixture
{
    [Fact]
    public void Execute_NameIsNull_DoesNotNavigateToPage()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var logFactory = new Mock<ILoggerFactory>();
        logFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        container.RegisterInstance<ILoggerFactory>(logFactory.Object);

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new NavigateToExtension();
        extension.Page = page;

        var ex = Record.Exception(() => extension.Execute(default));

        Mock.Get(mockNavigation)
            .Verify(x => x.NavigateAsync(It.IsAny<Uri>(), It.IsAny<INavigationParameters>()), Times.Never);
    }

    [Fact]
    public void Execute_NameIsSet_NavigatesToPage()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();

        Mock.Get(mockNavigation)
            .Setup(x => x.NavigateAsync(It.IsAny<Uri>(), It.IsAny<INavigationParameters>()))
            .ReturnsAsync(new NavigationResult());

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new NavigateToExtension();
        extension.Page = page;
        extension.Name = "ViewA";

        extension.Execute(default);

        Mock.Get(mockNavigation)
            .Verify(x => x.NavigateAsync(It.Is<Uri>(uri => uri.OriginalString == "ViewA"), It.IsAny<INavigationParameters>()), Times.Once);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void Execute_NavigationParameters_HasKnownNavigationParameters(bool animated, bool? useModalNavigation)
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var logFactory = new Mock<ILoggerFactory>();
        logFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());

        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        container.RegisterInstance(logFactory.Object);

        INavigationParameters parameters = default;
        Mock.Get(mockNavigation)
            .Setup(x => x.NavigateAsync(It.IsAny<Uri>(), It.IsAny<INavigationParameters>()))
            .Callback<Uri, INavigationParameters>((uri, navParameters) =>
            {
                parameters = navParameters;
            })
            .ReturnsAsync(new NavigationResult(new Exception()));

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new NavigateToExtension();
        extension.Page = page;
        extension.Name = "ViewA";
        extension.Animated = animated;
        extension.UseModalNavigation = useModalNavigation;

        extension.Execute(default);

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.Animated));
        Assert.Equal(animated, parameters.GetValue<bool>(KnownNavigationParameters.Animated));

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.UseModalNavigation));
        Assert.Equal(useModalNavigation, parameters.GetValue<bool?>(KnownNavigationParameters.UseModalNavigation));
    }

    [Fact]
    public void Execute_CommandParameter_IncludedInNavigationParameters()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();

        INavigationParameters parameters = default;
        Mock.Get(mockNavigation)
            .Setup(x => x.NavigateAsync(It.IsAny<Uri>(), It.IsAny<INavigationParameters>()))
            .Callback<Uri, INavigationParameters>((uri, navParameters) =>
            {
                parameters = navParameters;
            })
            .ReturnsAsync(new NavigationResult());

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new NavigateToExtension();
        extension.Page = page;
        extension.Name = "ViewA";

        extension.Execute("command parameter");

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.XamlParam));
        Assert.Equal("command parameter", parameters.GetValue<string>(KnownNavigationParameters.XamlParam));
    }
}
