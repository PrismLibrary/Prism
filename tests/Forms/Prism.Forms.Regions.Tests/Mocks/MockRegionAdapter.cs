using System.Collections.Generic;
using Prism.Navigation.Regions;
using Xamarin.Forms;
using RegionManager = Prism.Navigation.Regions.Xaml.RegionManager;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockRegionAdapter : IRegionAdapter
    {
        public List<string> CreatedRegions = new List<string>();
        public MockRegionManagerAccessor Accessor;


        public IRegion Initialize(object regionTarget, string regionName)
        {
            CreatedRegions.Add(regionName);

            var region = new MockPresentationRegion();
            if (regionTarget is VisualElement element)
                RegionManager.GetObservableRegion(element).Value = region;

            // Fire update regions again. This also happens if a region is created and added to the RegionManager
            if (Accessor != null)
                Accessor.UpdateRegions();

            return region;
        }
    }
}
