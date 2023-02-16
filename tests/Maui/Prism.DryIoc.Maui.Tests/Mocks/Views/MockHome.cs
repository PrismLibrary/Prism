namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockHome : FlyoutPage
{
    public MockHome()
    {
        Flyout = new ContentPage { Title = "Menu" };
    }
}
