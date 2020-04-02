

using System;
using Xunit;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    
    public class RegionAdapterBaseFixture
    {
        [Fact]
        public void IncorrectTypeThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                IRegionAdapter adapter = new TestableRegionAdapterBase();
                adapter.Initialize(new MockDependencyObject(), "Region1");
            });
        }

        [Fact]
        public void InitializeSetsRegionName()
        {
            IRegionAdapter adapter = new TestableRegionAdapterBase();
            var region = adapter.Initialize(new MockRegionTarget(), "Region1");
            Assert.Equal("Region1", region.Name);
        }

        [Fact]
        public void NullRegionNameThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                IRegionAdapter adapter = new TestableRegionAdapterBase();
                var region = adapter.Initialize(new MockRegionTarget(), null);
            });

        }

        [Fact]
        public void NullObjectThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                IRegionAdapter adapter = new TestableRegionAdapterBase();
                adapter.Initialize(null, "Region1");
            });

        }


        [Fact]
        public void CreateRegionReturnValueIsPassedToAdapt()
        {
            var regionTarget = new MockRegionTarget();
            var adapter = new TestableRegionAdapterBase();

            adapter.Initialize(regionTarget, "Region1");

            Assert.Same(adapter.CreateRegionReturnValue, adapter.AdaptArgumentRegion);
            Assert.Same(regionTarget, adapter.adaptArgumentRegionTarget);
        }

        [Fact]
        public void CreateRegionReturnValueIsPassedToAttachBehaviors()
        {
            var regionTarget = new MockRegionTarget();
            var adapter = new TestableRegionAdapterBase();

            var region = adapter.Initialize(regionTarget, "Region1");

            Assert.Same(adapter.CreateRegionReturnValue, adapter.AttachBehaviorsArgumentRegion);
            Assert.Same(regionTarget, adapter.attachBehaviorsArgumentTargetToAdapt);
        }

        class TestableRegionAdapterBase : RegionAdapterBase<MockRegionTarget>
        {
            public IRegion CreateRegionReturnValue = new MockPresentationRegion();
            public IRegion AdaptArgumentRegion;
            public MockRegionTarget adaptArgumentRegionTarget;
            public IRegion AttachBehaviorsArgumentRegion;
            public MockRegionTarget attachBehaviorsArgumentTargetToAdapt;

            public TestableRegionAdapterBase() :base(null)
            {
                
            }

            protected override void Adapt(IRegion region, MockRegionTarget regionTarget)
            {
                AdaptArgumentRegion = region;
                adaptArgumentRegionTarget = regionTarget;
            }

            protected override IRegion CreateRegion()
            {
                return CreateRegionReturnValue;
            }

            protected override void AttachBehaviors(IRegion region, MockRegionTarget regionTarget)
            {
                AttachBehaviorsArgumentRegion = region;
                attachBehaviorsArgumentTargetToAdapt = regionTarget;
                base.AttachBehaviors(region, regionTarget);
            }
        }

        class MockRegionTarget
        {
        }
    }
}
