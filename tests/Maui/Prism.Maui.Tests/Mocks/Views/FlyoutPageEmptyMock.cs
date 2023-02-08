using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class FlyoutPageEmptyMock : FlyoutPage
{
    public FlyoutPageEmptyMock()
    {
        //ViewModelLocator.SetAutowireViewModel(this, true);
        Flyout = new ContentPageMock { Title = "Master" };
    }
}
