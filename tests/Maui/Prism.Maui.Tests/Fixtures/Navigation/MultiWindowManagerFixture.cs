using Moq;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation;

namespace Prism.Maui.Tests.Fixtures.Navigation;

public class MultiWindowManagerFixture
{
    public MultiWindowManagerFixture()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
    }

    [Fact]
    public void CurrentFollowsNativeActivationOfSecondaryWindow()
    {
        var primary = new PrismWindow();
        var secondary = new PrismWindow("Secondary");
        var manager = CreateManager(primary, secondary);
        manager.OpenWindow(primary);
        manager.OpenWindow(secondary);

        ((IWindow)primary).Activated();
        ((IWindow)primary).Deactivated();
        ((IWindow)secondary).Activated();

        Assert.Same(secondary, manager.Current);
        Assert.False(primary.IsActive);
        Assert.True(secondary.IsActive);
    }

    [Fact]
    public void OpeningSecondaryWindowDoesNotPretendPrimaryWasDeactivated()
    {
        var primary = new PrismWindow();
        var secondary = new PrismWindow("Secondary");
        var manager = CreateManager(primary, secondary);
        manager.OpenWindow(primary);
        ((IWindow)primary).Activated();

        manager.OpenWindow(secondary);

        Assert.True(primary.IsActive);
        Assert.False(secondary.IsActive);
        Assert.Same(primary, manager.Current);
    }

    private static PrismWindowManager CreateManager(params Window[] windows)
    {
        var application = new Mock<IApplication>();
        application.SetupGet(value => value.Windows).Returns(windows);
        return new PrismWindowManager(application.Object);
    }

    [Fact]
    public void DeactivationOfAnotherWindowDoesNotClearCurrentWindow()
    {
        var primary = new PrismWindow();
        var secondary = new PrismWindow("Secondary");
        var manager = CreateManager(primary, secondary);
        manager.OpenWindow(primary);
        manager.OpenWindow(secondary);
        ((IWindow)primary).Activated();
        ((IWindow)secondary).Activated();

        ((IWindow)primary).Deactivated();

        Assert.Same(secondary, manager.Current);
    }

    [Fact]
    public void NativeDestructionReleasesInitialWindowReference()
    {
        var primary = new PrismWindow();
        var manager = CreateManager(primary);
        manager.OpenWindow(primary);
        ((IWindow)primary).Activated();

        ((IWindow)primary).Destroying();

        Assert.Null(manager.Current);
    }

    [Fact]
    public void ClosingActiveSecondaryWindowReleasesItsCurrentReference()
    {
        var primary = new PrismWindow();
        var secondary = new PrismWindow("Secondary");
        var manager = CreateManager(primary, secondary);
        manager.OpenWindow(primary);
        manager.OpenWindow(secondary);
        ((IWindow)secondary).Activated();

        manager.CloseWindow(secondary);

        Assert.Same(primary, manager.Current);
    }
}
