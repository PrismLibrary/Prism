using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    public class AutowireView : Page
    {
        public AutowireView()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}