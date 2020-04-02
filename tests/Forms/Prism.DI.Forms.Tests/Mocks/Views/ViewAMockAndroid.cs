using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    public class ViewAMockAndroid : Page
    {
        public ViewAMockAndroid()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}