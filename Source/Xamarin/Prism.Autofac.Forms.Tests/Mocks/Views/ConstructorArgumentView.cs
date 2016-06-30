using Prism.Autofac.Forms.Tests.Mocks.Services;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Autofac.Forms.Tests.Mocks.Views
{
    public class ConstructorArgumentView : Page
    {
        public ConstructorArgumentView(IAutofacServiceMock service)
        {
            Service = service;
            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        public IAutofacServiceMock Service { get; }
    }
}