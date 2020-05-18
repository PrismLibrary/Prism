using Prism.Navigation;
using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public class RegionAViewModel : ViewModelBase
    {
        public RegionAViewModel(INavigationService navigationService, IRegionNavigationService regionNavigation)
        {
            Title = "Hello from Region A";
            NavigationService = navigationService;
            RegionNavigation = regionNavigation;
        }

        public INavigationService NavigationService { get; set; }

        public IRegionNavigationService RegionNavigation { get; set; }
    }
}
