

using System;
using System.Collections.Generic;
using Xunit;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class AutoPopulateRegionBehaviorFixture
    {
        [Fact]
        public void ShouldGetViewsFromRegistryOnAttach()
        {
            var region = new MockPresentationRegion() { Name = "MyRegion" };
            var viewFactory = new MockRegionContentRegistry();
            var view = new object();
            viewFactory.GetContentsReturnValue.Add(view);
            var behavior = new AutoPopulateRegionBehavior(viewFactory)
                               {
                                   Region = region
                               };

            behavior.Attach();

            Assert.Equal("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.Single(region.MockViews.Items);
            Assert.Equal(view, region.MockViews.Items[0]);
        }

        [Fact]
        public void ShouldGetViewsFromRegistryWhenRegisteringItAfterAttach()
        {
            var region = new MockPresentationRegion() { Name = "MyRegion" };
            var viewFactory = new MockRegionContentRegistry();
            var behavior = new AutoPopulateRegionBehavior(viewFactory)
                               {
                                   Region = region
                               };
            var view = new object();

            behavior.Attach();
            viewFactory.GetContentsReturnValue.Add(view);
            viewFactory.RaiseContentRegistered("MyRegion", view);

            Assert.Equal("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.Single(region.MockViews.Items);
            Assert.Equal(view, region.MockViews.Items[0]);
        }

        [Fact]
        public void NullRegionThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var behavior = new AutoPopulateRegionBehavior(new MockRegionContentRegistry());

                behavior.Attach();
            });

        }

        [Fact]
        public void CanAttachBeforeSettingName()
        {
            var region = new MockPresentationRegion() { Name = null };
            var viewFactory = new MockRegionContentRegistry();
            var view = new object();
            viewFactory.GetContentsReturnValue.Add(view);
            var behavior = new AutoPopulateRegionBehavior(viewFactory)
            {
                Region = region
            };

            behavior.Attach();
            Assert.False(viewFactory.GetContentsCalled);

            region.Name = "MyRegion";

            Assert.True(viewFactory.GetContentsCalled);
            Assert.Equal("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.Single(region.MockViews.Items);
            Assert.Equal(view, region.MockViews.Items[0]);
        }

        private class MockRegionContentRegistry : IRegionViewRegistry
        {
            public readonly List<object> GetContentsReturnValue = new List<object>();
            public string GetContentsArgumentRegionName;
            public bool GetContentsCalled;

            public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

            public IEnumerable<object> GetContents(string regionName)
            {
                GetContentsCalled = true;
                this.GetContentsArgumentRegionName = regionName;
                return this.GetContentsReturnValue;
            }

            public void RaiseContentRegistered(string regionName, object view)
            {
                this.ContentRegistered(this, new ViewRegisteredEventArgs(regionName, () => view));
            }

            public void RegisterViewWithRegion(string regionName, Type viewType)
            {
                throw new NotImplementedException();
            }

            public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
            {
                throw new NotImplementedException();
            }
        }
    }
}