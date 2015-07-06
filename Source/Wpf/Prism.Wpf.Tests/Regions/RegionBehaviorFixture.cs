

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionBehaviorFixture
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotChangeRegionAfterAttach()
        {
            TestableRegionBehavior regionBehavior = new TestableRegionBehavior();

            regionBehavior.Region = new MockPresentationRegion();

            regionBehavior.Attach();
            regionBehavior.Region = new MockPresentationRegion();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldFailWhenAttachedWithoutRegion()
        {
            TestableRegionBehavior regionBehavior = new TestableRegionBehavior();
            regionBehavior.Attach();
        }

        [TestMethod]
        public void ShouldCallOnAttachWhenAttachMethodIsInvoked()
        {
            TestableRegionBehavior regionBehavior = new TestableRegionBehavior();

            regionBehavior.Region = new MockPresentationRegion();

            regionBehavior.Attach();

            Assert.IsTrue(regionBehavior.onAttachCalled);
        }

        private class TestableRegionBehavior : RegionBehavior
        {
            public bool onAttachCalled;

            protected override void OnAttach()
            {
                onAttachCalled = true;
            }
        }
    }

    
}
