

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionAdapterMappingsFixture
    {
        [TestMethod]
        public void ShouldGetRegisteredMapping()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            Type registeredType = typeof(ItemsControl);
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(registeredType, regionAdapter);
            var returnedAdapter = regionAdapterMappings.GetMapping(registeredType);

            Assert.IsNotNull(returnedAdapter);
            Assert.AreSame(regionAdapter, returnedAdapter);
        }

        [TestMethod]
        public void ShouldGetMappingForDerivedTypesThanTheRegisteredOnes()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), regionAdapter);
            var returnedAdapter = regionAdapterMappings.GetMapping(typeof(ItemsControlDescendant));

            Assert.IsNotNull(returnedAdapter);
            Assert.AreSame(regionAdapter, returnedAdapter);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetMappingOfUnregisteredTypeThrows()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            regionAdapterMappings.GetMapping(typeof(object));
        }

        [TestMethod]
        public void ShouldGetTheMostSpecializedMapping()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var genericAdapter = new MockRegionAdapter();
            var specializedAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), genericAdapter);
            regionAdapterMappings.RegisterMapping(typeof(ItemsControlDescendant), specializedAdapter);
            var returnedAdapter = regionAdapterMappings.GetMapping(typeof(ItemsControlDescendant));

            Assert.IsNotNull(returnedAdapter);
            Assert.AreSame(specializedAdapter, returnedAdapter);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterAMappingThatAlreadyExistsThrows()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), regionAdapter);
            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), regionAdapter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullControlThrows()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(null, regionAdapter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAdapterThrows()
        {
            var regionAdapterMappings = new RegionAdapterMappings();

            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), null);
        }

        class ItemsControlDescendant : ItemsControl { }

    }
}