using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    public class CustomNamedNavService : Page
    {
        public CustomNamedNavService()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}