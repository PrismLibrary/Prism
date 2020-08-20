using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class FrameDemoRegionViewModel : RegionDemoBase
    {
        public FrameDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "FrameRegion";
    }
}
