using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public abstract class RegionDemoBase : BindableBase, IDestructible, IInitialize
    {
        private IRegionManager _regionManager { get; }

        protected RegionDemoBase(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        protected abstract string RegionName { get; }

        public DelegateCommand<string> NavigateCommand { get; }

        private void Navigate(string target)
        {
            _regionManager.RequestNavigate(RegionName, target, NavigationCallback);
        }

        private void NavigationCallback(IRegionNavigationResult result)
        {

        }

        public void Destroy()
        {
            _regionManager.Regions.Remove(RegionName);
        }

        public void Initialize(INavigationParameters parameters)
        {
            _regionManager.RequestNavigate(RegionName, "RegionA", NavigationCallback);
        }
    }
}
