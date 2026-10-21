using Playground.Module.Dialogs;
using Playground.Module.ViewModels;
using Playground.Module.Views;

namespace Playground.Module;

public class PlaygroundModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
        containerRegistry.RegisterForNavigation<RegionLabView, RegionLabViewModel>();
        containerRegistry.RegisterForNavigation<DialogLabView, DialogLabViewModel>();
        containerRegistry.RegisterForNavigation<ModuleAboutView, ModuleAboutViewModel>();
        containerRegistry.RegisterForNavigation<RegionDetailAlphaView, RegionDetailAlphaViewModel>();
        containerRegistry.RegisterForNavigation<RegionDetailBetaView, RegionDetailBetaViewModel>();

        containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
        containerRegistry.RegisterDialog<ConfirmDialog, ConfirmDialogViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RequestNavigate("ContentRegion", "HomeView");
    }
}
