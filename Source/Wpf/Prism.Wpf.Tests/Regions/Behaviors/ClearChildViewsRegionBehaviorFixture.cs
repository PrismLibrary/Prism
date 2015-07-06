

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    [TestClass]
    public class ClearChildViewsRegionBehaviorFixture
    {
        [TestMethod]
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

            Assert.AreEqual(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));

            region.RegionManager = null;

            Assert.AreEqual(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));
        }

        [TestMethod]
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

            Assert.AreEqual(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));

            region.RegionManager = null;

            Assert.IsNull(childView.GetValue(RegionManager.RegionManagerProperty));
        }

        [TestMethod]
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

            Assert.AreEqual(regionManager, childView.GetValue(RegionManager.RegionManagerProperty));

            region.RegionManager = new MockRegionManager();

            Assert.IsNotNull(childView.GetValue(RegionManager.RegionManagerProperty));
        }
    }
}
