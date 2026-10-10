using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Moq;
using Prism.Events;
using Prism.Ioc;
using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;
using Prism.Navigation;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class MultiWindowNavigationFixture : IDisposable
{
    private readonly PageNavigationContainerMock _container = new();

    public MultiWindowNavigationFixture()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        _ = MauiApp.CreateBuilder().UseMauiApp<Application>().Build();
        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(_container);
        _container.RegisterForNavigation<ContentPage>("Replacement");
    }

    [Fact]
    public async Task AbsoluteNavigationFromSecondaryWindowReplacesOnlyItsRoot()
    {
        var primaryRoot = new ContentPageMock();
        var secondaryRoot = new ContentPageMock();
        var primary = new PrismWindow { Page = primaryRoot };
        var secondary = new PrismWindow("Secondary") { Page = secondaryRoot };
        var primaryModal = new ContentPage();
        await primary.Navigation.PushModalAsync(primaryModal);
        var service = CreateService(secondaryRoot, primary, secondary);

        var result = await service.NavigateAsync("/Replacement");

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Same(primaryRoot, primary.Page);
        Assert.Same(primaryModal, Assert.Single(primary.Navigation.ModalStack));
        Assert.NotSame(secondaryRoot, secondary.Page);
        Assert.IsType<ContentPage>(secondary.Page);
        Assert.False(primaryRoot.DestroyCalled);
        Assert.True(secondaryRoot.DestroyCalled);
    }

    [Fact]
    public async Task RepeatedBackNavigationKeepsSecondaryWindowOwnership()
    {
        var primaryRoot = new ContentPageMock();
        var secondaryRoot = new ContentPageMock();
        var primary = new PrismWindow { Page = primaryRoot };
        var secondary = new PrismWindow("Secondary") { Page = secondaryRoot };
        var modal = new ContentPage();
        await secondary.Navigation.PushModalAsync(modal);
        var service = CreateService(modal, primary, secondary);

        var result = await service.GoBackAsync();

        Assert.True(result.Success, result.Exception?.ToString());
        Assert.Empty(secondary.Navigation.ModalStack);
        Assert.Same(primaryRoot, primary.Page);
        Assert.Same(secondaryRoot, secondary.Page);
        Assert.False(primaryRoot.OnNavigatedToCalled);
        Assert.True(secondaryRoot.OnNavigatedToCalled);
    }

    private PageNavigationService CreateService(Page page, params Window[] windows)
    {
        var manager = new Mock<IWindowManager>();
        manager.SetupGet(value => value.Windows).Returns(windows);
        manager.SetupGet(value => value.Current).Returns(windows[0]);
        return new PageNavigationService(_container, manager.Object, new EventAggregator(),
            new MutablePageAccessor { Page = page });
    }

    [Fact]
    public async Task DetachedScopeCannotReplaceAnotherWindowRoot()
    {
        var primaryRoot = new ContentPageMock();
        var primary = new PrismWindow { Page = primaryRoot };
        var service = CreateService(new ContentPage(), primary);

        var result = await service.NavigateAsync("/Replacement");

        Assert.False(result.Success);
        Assert.NotNull(result.Exception);
        Assert.Same(primaryRoot, primary.Page);
        Assert.False(primaryRoot.DestroyCalled);
        Assert.Null(_container.CurrentScope);
    }

    public void Dispose()
    {
        ContainerLocator.ResetContainer();
    }
}
