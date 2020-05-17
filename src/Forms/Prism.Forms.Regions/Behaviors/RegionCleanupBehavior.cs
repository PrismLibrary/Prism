using System;
using Prism.Regions;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    internal class RegionCleanupBehavior : BehaviorBase<Page>
    {
        private WeakReference<IRegion> _regionReference;

        public RegionCleanupBehavior(IRegion region)
        {
            _regionReference = new WeakReference<IRegion>(region);
        }

        public IRegion Region => _regionReference.TryGetTarget(out var target) ? target : null;

        protected override void OnDetachingFrom(Page bindable)
        {
            if (Region != null && Region.RegionManager.Regions.ContainsRegionWithName(Region.Name))
            {
                Region.RegionManager.Regions.Remove(Region.Name);
            }
        }
    }
}
