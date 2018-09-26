using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism
{
    public interface IPrismApplicationBase
    {
        /*
         
         STARTUP EXECUTION ORDER
         * 01. PrismApplicationBase.InternalInitialize
         * 02. PrismApplicationBase.RegisterRequiredTypes
         * 03. PrismApplicationBase.InternalStartAsync(Args:Windows.ApplicationModel.Activation.LaunchActivatedEventArgs Kind:Launch Cause:PrimaryTile)
         * 04. PrismApplicationBase.CallOnInitializedOnce
          
         THEN
         * 05. NavigationService.NavigateAsync(MainPage)
         * 06. FrameFacade.NavigateAsync(MainPage)
         * 07. FrameFacade.NavigateAsync(MainPage)
         * 08. [From]View-Model is null.
         * 09. FrameFacade.NavigateFrameAsync HasThreadAccess: True
         * 10. FrameFacade.OrchestrateAsync.NavigateFrameAsync() returned True.
         * 11. Calling OnNavigatedToAsync
         * 12. INavigatedAwareAsync.OnNavigatedToAsync() called.
         * 13. Calling OnNavigatingTo
         * 14. INavigatingAware not implemented.
         * 15. Calling OnNavigatedTo
         * 16. INavigatedAware not implemented.
         
         */


        IContainerProvider Container { get; }
        void ConfigureViewModelLocator();
        IContainerExtension CreateContainerExtension();
        void OnInitialized();
        void OnStart(StartArgs args);
        Task OnStartAsync(StartArgs args);
        void RegisterTypes(IContainerRegistry container);
        void ConfigureModuleCatalog(IModuleCatalog moduleCatalog);
    }
}
