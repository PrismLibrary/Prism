using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public class RegionDemoPageViewModel : BindableBase
    {
        private IRegionManager _regionManager { get; }

        public RegionDemoPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        public DelegateCommand<string> NavigateCommand { get; }

        private void Navigate(string target)
        {
            _regionManager.RequestNavigate("ContentRegion", target, NavigationCallback);
        }

        private void NavigationCallback(IRegionNavigationResult obj)
        {

        }
    }
}
