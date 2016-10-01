using Prism.Autofac.Forms.Tests.Mocks.Services;
using Prism.Mvvm;

namespace Prism.Autofac.Forms.Tests.Mocks.ViewModels
{
    public class ConstructorArgumentViewModel : BindableBase
    {
        public IAutofacServiceMock Service { get; }

        public ConstructorArgumentViewModel(IAutofacServiceMock service)
        {
            Service = service;
        }
    }
}
