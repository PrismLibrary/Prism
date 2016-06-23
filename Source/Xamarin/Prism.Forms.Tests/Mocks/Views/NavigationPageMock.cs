using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class NavigationPageMock : NavigationPage
    {
        public NavigationPageMock() : base (new ContentPageMock())
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class NavigationPageEmptyMock : NavigationPage
    {
        public NavigationPageEmptyMock() : base()
        {
            
        }
    }

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
}
