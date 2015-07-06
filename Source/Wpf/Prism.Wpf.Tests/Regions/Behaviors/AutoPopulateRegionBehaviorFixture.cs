

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    [TestClass]
    public class AutoPopulateRegionBehaviorFixture
    {
        [TestMethod]
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

            Assert.AreEqual("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.AreEqual(1, region.MockViews.Items.Count);
            Assert.AreEqual(view, region.MockViews.Items[0]);
        }

        [TestMethod]
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

            Assert.AreEqual("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.AreEqual(1, region.MockViews.Items.Count);
            Assert.AreEqual(view, region.MockViews.Items[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NullRegionThrows()
        {
            var behavior = new AutoPopulateRegionBehavior(new MockRegionContentRegistry());

            behavior.Attach();
        }

        [TestMethod]
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
            Assert.IsFalse(viewFactory.GetContentsCalled);

            region.Name = "MyRegion";

            Assert.IsTrue(viewFactory.GetContentsCalled);
            Assert.AreEqual("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.AreEqual(1, region.MockViews.Items.Count);
            Assert.AreEqual(view, region.MockViews.Items[0]);
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