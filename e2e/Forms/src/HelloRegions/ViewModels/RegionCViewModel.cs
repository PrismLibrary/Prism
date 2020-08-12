using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public class RegionCViewModel : ViewModelBase
    {
        public RegionCViewModel(IRegionNavigationService regionNavigationService)
            : base(regionNavigationService)
        {
            Title = "Hello from Region C";
        }
    }
}
