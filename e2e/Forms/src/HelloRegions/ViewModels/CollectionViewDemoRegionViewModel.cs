using Prism.Regions;

namespace HelloRegions.ViewModels
{
    public class CollectionViewDemoRegionViewModel : RegionDemoBase
    {
        public CollectionViewDemoRegionViewModel(IRegionManager regionManager)
            : base(regionManager)
        {
        }

        protected override string RegionName => "CollectionRegion";
    }
}
