using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class NavigationPageWithStackNoMatchMock : NavigationPage
{
    public NavigationPageWithStackNoMatchMock() : base()
    {
        var p1 = new ContentPage();
        var p2 = new ContentPage();
        var p3 = new ContentPage();

        Navigation.PushAsync(p1);
        p1.Navigation.PushAsync(p2);
        p2.Navigation.PushAsync(p3);
    }
}
