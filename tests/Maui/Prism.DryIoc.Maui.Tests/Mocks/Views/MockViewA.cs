namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockViewA : ContentPage
{
    public const string ExpectedTitle = "Mock View A";
    public static readonly ImageSource ExpectedIconImageSource = "home.png";
    public MockViewA()
    {
        Title = ExpectedTitle;
        IconImageSource = ExpectedIconImageSource;
    }
}
