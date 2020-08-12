using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class ContentViewDemoRegionViewModel : RegionDemoBase
    {
        public ContentViewDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "ContentRegion";
    }
}
