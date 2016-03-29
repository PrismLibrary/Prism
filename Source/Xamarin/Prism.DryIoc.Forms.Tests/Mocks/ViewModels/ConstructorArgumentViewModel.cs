using Prism.DryIoc.Forms.Tests.Mocks.Services;

namespace Prism.DryIoc.Forms.Tests.Mocks.ViewModels
{
    public class ConstructorArgumentViewModel
    {
        public IDryIocServiceMock Service { get; }

        public ConstructorArgumentViewModel(IDryIocServiceMock service)
        {
            Service = service;
        }
    }
}
