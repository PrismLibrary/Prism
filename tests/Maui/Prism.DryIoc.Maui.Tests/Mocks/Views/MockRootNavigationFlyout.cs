namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

/// <summary>A modal flyout with an inactive menu that records navigation and destruction.</summary>
public sealed class MockRootNavigationFlyout : FlyoutPage
{
    public MockRootNavigationFlyout() => Flyout = new MockNavigateFromPage { Title = "Menu" };
}
