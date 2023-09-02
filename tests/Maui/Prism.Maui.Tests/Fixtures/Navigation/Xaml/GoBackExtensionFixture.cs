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

public class GoBackExtensionFixture
{
    [Fact]
    public void Execute_GoBackTypeDefault_GoesBackAPage()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        var mockLoggingProvider = new Mock<ILoggerFactory>();
        mockLoggingProvider.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        container.RegisterInstance(mockLoggingProvider.Object);

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new GoBackExtension();
        extension.Page = page;

        extension.Execute(default);

        Mock.Get(mockNavigation)
            .Verify(x => x.GoBackAsync(It.IsAny<INavigationParameters>()), Times.Once);
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, null)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void Execute_GoBackTypeDefault_NavigationParameters_HasKnownNavigationParameters(bool animated, bool? useModalNavigation)
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        var mockLoggingProvider = new Mock<ILoggerFactory>();
        mockLoggingProvider.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        container.RegisterInstance(mockLoggingProvider.Object);

        INavigationParameters parameters = default;
        Mock.Get(mockNavigation)
            .Setup(x => x.GoBackAsync(It.IsAny<INavigationParameters>()))
            .Callback<INavigationParameters>(navParameters =>
            {
                parameters = navParameters;
            })
            .ReturnsAsync(new NavigationResult(new Exception()));

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new GoBackExtension();
        extension.Page = page;
        extension.Animated = animated;
        extension.UseModalNavigation = useModalNavigation;

        extension.Execute(default);

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.Animated));
        Assert.Equal(animated, parameters.GetValue<bool>(KnownNavigationParameters.Animated));

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.UseModalNavigation));
        Assert.Equal(useModalNavigation, parameters.GetValue<bool?>(KnownNavigationParameters.UseModalNavigation));
    }

    [Fact]
    public void Execute_GoBackTypeToRoot_GoesBackToRoot()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        var mockLoggingProvider = new Mock<ILoggerFactory>();
        mockLoggingProvider.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        container.RegisterInstance(mockLoggingProvider.Object);

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new GoBackExtension();
        extension.Page = page;
        extension.GoBackType = GoBackType.ToRoot;

        extension.Execute(default);

        Mock.Get(mockNavigation)
            .Verify(x => x.GoBackToRootAsync(It.IsAny<INavigationParameters>()), Times.Once);
    }

    [Fact]
    public void Execute_GoBackTypeToRoot_NavigationParameters_DoNotHaveKnownNavigationParameters()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        var mockLoggingProvider = new Mock<ILoggerFactory>();
        mockLoggingProvider.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        container.RegisterInstance(mockLoggingProvider.Object);

        INavigationParameters parameters = default;
        Mock.Get(mockNavigation)
            .Setup(x => x.GoBackToRootAsync(It.IsAny<INavigationParameters>()))
            .Callback<INavigationParameters>(navParameters =>
            {
                parameters = navParameters;
            })
            .ReturnsAsync(new NavigationResult(new Exception()));

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new GoBackExtension();
        extension.Page = page;
        extension.GoBackType = GoBackType.ToRoot;

        extension.Execute(default);

        Assert.False(parameters.ContainsKey(KnownNavigationParameters.Animated));
        Assert.False(parameters.ContainsKey(KnownNavigationParameters.UseModalNavigation));
    }

    [Fact]
    public void Execute_GoBackTypeDefault_CommandParameter_IncludedInNavigationParameters()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        var mockLoggingProvider = new Mock<ILoggerFactory>();
        mockLoggingProvider.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        container.RegisterInstance(mockLoggingProvider.Object);

        INavigationParameters parameters = default;
        Mock.Get(mockNavigation)
            .Setup(x => x.GoBackAsync(It.IsAny<INavigationParameters>()))
            .Callback<INavigationParameters>(navParameters =>
            {
                parameters = navParameters;
            })
            .ReturnsAsync(new NavigationResult(new Exception()));

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new GoBackExtension();
        extension.Page = page;

        extension.Execute("command parameter");

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.XamlParam));
        Assert.Equal("command parameter", parameters.GetValue<string>(KnownNavigationParameters.XamlParam));
    }

    [Fact]
    public void Execute_GoBackTypeToRoot_CommandParameter_IncludedInNavigationParameters()
    {
        var mockNavigation = Mock.Of<INavigationService>();
        var container = new TestContainer();
        container.RegisterInstance(mockNavigation);
        container.RegisterInstance(new PageAccessor());
        container.RegisterForNavigation<PageMock, PageMockViewModel>();
        var mockLoggingProvider = new Mock<ILoggerFactory>();
        mockLoggingProvider.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        container.RegisterInstance(mockLoggingProvider.Object);

        INavigationParameters parameters = default;
        Mock.Get(mockNavigation)
            .Setup(x => x.GoBackToRootAsync(It.IsAny<INavigationParameters>()))
            .Callback<INavigationParameters>(navParameters =>
            {
                parameters = navParameters;
            })
            .ReturnsAsync(new NavigationResult(new Exception()));

        var registry = container.Resolve<NavigationRegistry>();
        var page = registry.CreateView(container, "PageMock") as Page;
        var extension = new GoBackExtension();
        extension.Page = page;
        extension.GoBackType = GoBackType.ToRoot;

        extension.Execute("command parameter");

        Assert.True(parameters.ContainsKey(KnownNavigationParameters.XamlParam));
        Assert.Equal("command parameter", parameters.GetValue<string>(KnownNavigationParameters.XamlParam));
    }
}
