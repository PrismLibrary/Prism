using System;
using System.Collections.Generic;
using System.Text;
using Prism.Forms.Regions.Mocks;
using Prism.Regions.Behaviors;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class RegionBehaviorFixture
    {
        [Fact]
        public void CannotChangeRegionAfterAttach()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                TestableRegionBehavior regionBehavior = new TestableRegionBehavior
                {
                    Region = new MockPresentationRegion()
                };

                regionBehavior.Attach();
                regionBehavior.Region = new MockPresentationRegion();
            });
        }

        [Fact]
        public void ShouldFailWhenAttachedWithoutRegion()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var regionBehavior = new TestableRegionBehavior();
                regionBehavior.Attach();
            });
        }

        [Fact]
        public void ShouldCallOnAttachWhenAttachMethodIsInvoked()
        {
            var regionBehavior = new TestableRegionBehavior
            {
                Region = new MockPresentationRegion()
            };

            regionBehavior.Attach();

            Assert.True(regionBehavior.onAttachCalled);
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
