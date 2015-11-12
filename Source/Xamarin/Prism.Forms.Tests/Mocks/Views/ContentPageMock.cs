using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class ContentPageMock : ContentPage
    {
        public ContentPageMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}
