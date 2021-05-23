using System.Collections.Generic;
using Prism.Regions;
using Prism.Regions.Adapters;
using Xamarin.Forms;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockRegionAdapter : IRegionAdapter
    {
        public List<string> CreatedRegions = new List<string>();
        public MockRegionManagerAccessor Accessor;


        public IRegion Initialize(VisualElement regionTarget, string regionName)
        {
            CreatedRegions.Add(regionName);

            var region = new MockPresentationRegion();
            Prism.Regions.Xaml.RegionManager.GetObservableRegion(regionTarget).Value = region;

            // Fire update regions again. This also happens if a region is created and added to the regionmanager
            if (Accessor != null)
                Accessor.UpdateRegions();

            return region;
        }
    }
}
