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
}
