using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class CarouselDemoRegionViewModel : RegionDemoBase
    {
        public CarouselDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "CarouselRegion";
    }
}
