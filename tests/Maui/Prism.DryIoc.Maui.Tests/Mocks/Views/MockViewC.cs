using Prism.Xaml;

namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockViewC : ContentPage
{
    public const string ExpectedTitle = "Mock View C";
    public const string ExpectedDynamicTitle = "Dynamic View C";
    public MockViewC()
    {
        Title = ExpectedTitle;
        DynamicTab.SetTitle(this, ExpectedDynamicTitle);
        DynamicTab.SetIconImageSource(this, "home.png");
    }
}
