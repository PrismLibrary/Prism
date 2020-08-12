using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class FlexLayoutDemoRegionViewModel : RegionDemoBase
    {
        public FlexLayoutDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "FlexRegion";
    }
}
