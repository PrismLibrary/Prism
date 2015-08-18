

using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionManagerExtensionsFixture
    {
        [TestMethod]
        public void CanAddRegionToRegionManager()
        {
            var regionManager = new MockRegionManager();
            var region = new MockRegion();

            regionManager.Regions.Add("region", region);

            Assert.AreEqual(1, regionManager.MockRegionCollection.Count);
            Assert.AreEqual("region", region.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowIfRegionNameArgumentIsDifferentToRegionNameProperty()
        {
            var regionManager = new MockRegionManager();
            var region = new MockRegion();

            region.Name = "region";

            regionManager.Regions.Add("another region", region);
        }
    }

    internal class MockRegionContentRegistry : IRegionViewRegistry
    {
        public Func<string, Type, object> RegisterContentWithViewType;
        public Func<string, Func<object>, object> RegisterContentWithDelegate;
        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;
        public IEnumerable<object> GetContents(string regionName)
        {
            return null;
        }

        void IRegionViewRegistry.RegisterViewWithRegion(string regionName, Type viewType)
        {
            if (RegisterContentWithViewType != null)
            {
                RegisterContentWithViewType(regionName, viewType);
            }
        }

        void IRegionViewRegistry.RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            if (RegisterContentWithDelegate != null)
            {
                RegisterContentWithDelegate(regionName, getContentDelegate);
            }

        }
    }
}
