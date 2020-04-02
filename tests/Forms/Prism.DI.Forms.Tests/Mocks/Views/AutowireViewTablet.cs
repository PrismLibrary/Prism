using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    public class AutowireViewTablet : Page
    {
        public AutowireViewTablet()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}