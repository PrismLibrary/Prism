using Prism.DryIoc.Forms.Tests.Services;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DryIoc.Forms.Tests.Mocks.Views
{
    public class ConstructorArgumentView : Page
    {
        public ConstructorArgumentView(IDryIocServiceMock service)
        {
            Service = service;
            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        public IDryIocServiceMock Service { get; }
    }
}