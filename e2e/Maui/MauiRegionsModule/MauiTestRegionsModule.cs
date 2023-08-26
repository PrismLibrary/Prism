using MauiRegionsModule.ViewModels;
using MauiRegionsModule.Views;

namespace MauiRegionsModule;

public class MauiTestRegionsModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion<RegionViewA>("ContentRegion");
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry
            .RegisterForNavigation<ContentRegionPage>()
            .RegisterForNavigation<RegionHome, RegionHomeViewModel>()
            .RegisterForNavigation<DefaultViewNamedPage>()
            .RegisterForNavigation<DefaultViewInstancePage>()
            .RegisterForNavigation<DefaultViewTypePage>()
            .RegisterForRegionNavigation<RegionViewA, RegionViewAViewModel>()
            .RegisterForRegionNavigation<RegionViewB, RegionViewBViewModel>()
            .RegisterForRegionNavigation<RegionViewC, RegionViewCViewModel>();
    }
}
