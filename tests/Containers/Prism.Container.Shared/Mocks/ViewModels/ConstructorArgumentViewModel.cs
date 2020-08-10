using Prism.Ioc.Mocks.Services;
using Prism.Mvvm;

namespace Prism.Ioc.Mocks.ViewModels
{
    public class ConstructorArgumentViewModel : BindableBase
    {
        public IServiceA Service { get; }

        public ConstructorArgumentViewModel(IServiceA service)
        {
            Service = service;
        }
    }
}
