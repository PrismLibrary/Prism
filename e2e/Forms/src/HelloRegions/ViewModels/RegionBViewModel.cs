using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public class RegionBViewModel : ViewModelBase
    {
        public RegionBViewModel(IRegionNavigationService regionNavigationService)
            : base(regionNavigationService)
        {
            Title = "Hello from Region B";
        }
    }
}
