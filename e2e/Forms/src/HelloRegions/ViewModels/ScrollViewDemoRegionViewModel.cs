using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class ScrollViewDemoRegionViewModel : RegionDemoBase
    {
        public ScrollViewDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "ScrollRegion";
    }
}
