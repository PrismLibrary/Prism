

using Xunit;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class ClearChildViewsRegionBehaviorFixture
    {
        [StaFact]
        public void WhenClearChildViewsPropertyIsNotSet_ThenChildViewsRegionManagerIsNotCleared()
        {
            var regionManager = new MockRegionManager();

            var region = new Region();
            region.RegionManager = regionManager;

            var behavior = new ClearChildViewsRegionBehavior();
            behavior.Region = region;
            behavior.Attach();

            var childView = new MockFrameworkElement();
            region.Add(childView);

            Assert.Equal(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));

            region.RegionManager = null;

            Assert.Equal(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));
        }

        [StaFact]
        public void WhenClearChildViewsPropertyIsTrue_ThenChildViewsRegionManagerIsCleared()
        {
            var regionManager = new MockRegionManager();

            var region = new Region();
            region.RegionManager = regionManager;

            var behavior = new ClearChildViewsRegionBehavior();
            behavior.Region = region;
            behavior.Attach();

            var childView = new MockFrameworkElement();
            region.Add(childView);

            ClearChildViewsRegionBehavior.SetClearChildViews(childView, true);

            Assert.Equal(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));

            region.RegionManager = null;

            Assert.Null(childView.GetValue(RegionManager.RegionManagerProperty));
        }

        [StaFact]
        public void WhenRegionManagerChangesToNotNullValue_ThenChildViewsRegionManagerIsNotCleared()
        {
            var regionManager = new MockRegionManager();

            var region = new Region();
            region.RegionManager = regionManager;

            var behavior = new ClearChildViewsRegionBehavior();
            behavior.Region = region;
            behavior.Attach();

            var childView = new MockFrameworkElement();
            region.Add(childView);

            childView.SetValue(ClearChildViewsRegionBehavior.ClearChildViewsProperty, true);

            Assert.Equal(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));

            region.RegionManager = new MockRegionManager();

            Assert.NotNull(childView.GetValue(RegionManager.RegionManagerProperty));
        }
    }
}
