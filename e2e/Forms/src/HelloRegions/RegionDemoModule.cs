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
            containerRegistry.RegisterForNavigation<RegionDemoPage, RegionDemoPageViewModel>();
            containerRegistry.RegisterForNavigation<RegionA, RegionAViewModel>();
            containerRegistry.RegisterForNavigation<RegionB, RegionBViewModel>();
            containerRegistry.RegisterForNavigation<RegionC, RegionCViewModel>();
        }
    }
}
