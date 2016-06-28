using Prism.DryIoc.Forms.Tests.Mocks.Services;
using Prism.Mvvm;

namespace Prism.DryIoc.Forms.Tests.Mocks.ViewModels
{
    public class ConstructorArgumentViewModel : BindableBase
    {
        public IDryIocServiceMock Service { get; }

        public ConstructorArgumentViewModel(IDryIocServiceMock service)
        {
            Service = service;
        }
    }
}
