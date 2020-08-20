using System;
using HelloRegions.ViewModels;
using HelloRegions.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace HelloRegions
{
    public class RegionDemoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterRegionServices();

            containerRegistry.RegisterForNavigation<CarouselDemoRegion, CarouselDemoRegionViewModel>();
            containerRegistry.RegisterForNavigation<CollectionViewDemoRegion, CollectionViewDemoRegionViewModel>();
            containerRegistry.RegisterForNavigation<ContentViewDemoRegion, ContentViewDemoRegionViewModel>();
            containerRegistry.RegisterForNavigation<FlexLayoutDemoRegion, FlexLayoutDemoRegionViewModel>();
            containerRegistry.RegisterForNavigation<FrameDemoRegion, FrameDemoRegionViewModel>();
            containerRegistry.RegisterForNavigation<ScrollViewDemoRegion, ScrollViewDemoRegionViewModel>();
            containerRegistry.RegisterForNavigation<StackLayoutDemoRegion, StackLayoutDemoRegionViewModel>();

            containerRegistry.RegisterForRegionNavigation<RegionA, RegionAViewModel>();
            containerRegistry.RegisterForRegionNavigation<RegionB, RegionBViewModel>();
            containerRegistry.RegisterForRegionNavigation<RegionC, RegionCViewModel>();
        }
    }
}
