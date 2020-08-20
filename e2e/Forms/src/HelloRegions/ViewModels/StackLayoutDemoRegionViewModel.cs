using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class StackLayoutDemoRegionViewModel : RegionDemoBase
    {
        public StackLayoutDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "StackRegion";
    }
}
