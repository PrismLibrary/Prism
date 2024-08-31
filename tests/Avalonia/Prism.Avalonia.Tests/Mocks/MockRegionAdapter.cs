using System.Collections.Generic;
using Avalonia;
using Prism.Navigation.Regions;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockRegionAdapter : IRegionAdapter
    {
        public List<string> CreatedRegions = new List<string>();
        public MockRegionManagerAccessor Accessor;

        public IRegion Initialize(object regionTarget, string regionName)
        {
            CreatedRegions.Add(regionName);

            var region = new MockPresentationRegion();
            RegionManager.GetObservableRegion(regionTarget as AvaloniaObject).Value = region;

            // Fire update regions again. This also happens if a region is created and added to the regionmanager
            if (Accessor != null)
                Accessor.UpdateRegions();

            return region;
        }
    }
}
