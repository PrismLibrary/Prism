using System.Threading.Tasks;
using Prism.Ioc;

namespace Prism
{
    public interface IPrismApplicationBase
    {
        IContainerProvider Container { get; }
        void ConfigureViewModelLocator();
        IContainerExtension CreateContainer();
        void OnInitialized();
        void OnStart(StartArgs args);
        Task OnStartAsync(StartArgs args);
        void RegisterTypes(IContainerRegistry container);
    }
}