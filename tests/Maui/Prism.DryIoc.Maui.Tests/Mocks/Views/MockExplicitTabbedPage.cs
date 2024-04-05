namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

internal class MockExplicitTabbedPage : TabbedPage
{
    public MockExplicitTabbedPage()
    {
        Children.Add(new NavigationPage(new MockViewA()));
        Children.Add(new NavigationPage(new MockViewB()));
        Children.Add(new MockViewC());
    }
}
