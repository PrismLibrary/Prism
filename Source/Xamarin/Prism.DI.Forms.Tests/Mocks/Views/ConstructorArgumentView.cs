using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    public class ConstructorArgumentView : Page
    {
        public ConstructorArgumentView( IServiceMock service )
        {
            Service = service;
            ViewModelLocator.SetAutowireViewModel( this, true );
        }

        public IServiceMock Service { get; }
    }
}