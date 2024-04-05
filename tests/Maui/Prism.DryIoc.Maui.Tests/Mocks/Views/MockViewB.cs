using Prism.Xaml;

namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public class MockViewB : ContentPage
{
    public const string ExpectedTitle = "Mock View B";
    public MockViewB()
    {
        DynamicTab.SetTitle(this, ExpectedTitle);
    }
}
