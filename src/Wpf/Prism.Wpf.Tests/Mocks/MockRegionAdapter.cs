

using System.Collections.Generic;
using System.Windows;
using Prism.Regions;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockRegionAdapter : IRegionAdapter
    {
        public List<string> CreatedRegions = new List<string>();
        public MockRegionManagerAccessor Accessor;


        public IRegion Initialize(object regionTarget, string regionName)
        {
            CreatedRegions.Add(regionName);

            var region = new MockPresentationRegion();
            RegionManager.GetObservableRegion(regionTarget as DependencyObject).Value = region;

            // Fire update regions again. This also happens if a region is created and added to the regionmanager
            if (this.Accessor != null)
                Accessor.UpdateRegions();

            return region;
        }
    }
}