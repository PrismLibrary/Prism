using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.Mvvm;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class ConstructorArgumentViewModel : BindableBase
    {
        public IServiceMock Service { get; }

        public ConstructorArgumentViewModel( IServiceMock service )
        {
            Service = service;
        }
    }
}
