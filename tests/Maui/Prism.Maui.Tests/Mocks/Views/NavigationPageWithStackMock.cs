using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class NavigationPageWithStackMock : NavigationPage
{
    public NavigationPageWithStackMock() : base()
    {
        var p1 = new ContentPageMock();
        var p2 = new ContentPage();
        var p3 = new ContentPage();

        Navigation.PushAsync(p1);
        p1.Navigation.PushAsync(p2);
        p2.Navigation.PushAsync(p3);
    }
}
