

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionBehaviorCollectionFixture
    {
        [TestMethod]
        public void CanAttachRegionBehaviors()
        {
            RegionBehaviorCollection behaviorCollection = new RegionBehaviorCollection(new MockPresentationRegion());

            var mock1 = new MockRegionBehavior();
            bool mock1Attached = false;
            mock1.OnAttach = () => mock1Attached = true;
            behaviorCollection.Add("Mock1", mock1);

            var mock2 = new MockRegionBehavior();
            bool mock2Attached = false;
            mock2.OnAttach = () => mock2Attached = true;
            behaviorCollection.Add("Mock2", mock2);

            Assert.IsTrue(mock1Attached);
            Assert.IsTrue(mock2Attached);
        }

        [TestMethod]
        public void ShouldAddRegionWhenAddingBehavior()
        {
            var region = new MockPresentationRegion();
            RegionBehaviorCollection behaviorCollection = new RegionBehaviorCollection(region);
            var behavior = new MockRegionBehavior();

            behaviorCollection.Add("Mock", behavior);

            Assert.IsNotNull(behavior.Region);
            Assert.AreSame(region, behavior.Region);
        }

    }
}
