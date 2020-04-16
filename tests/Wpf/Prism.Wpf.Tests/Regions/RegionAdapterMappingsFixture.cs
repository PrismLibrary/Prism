using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Moq;
using Prism.Ioc;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;
using Xunit;

namespace Prism.Wpf.Tests.Regions
{

    public class RegionAdapterMappingsFixture
    {
        [Fact]
        public void ShouldGetRegisteredMapping()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            Type registeredType = typeof(ItemsControl);
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(registeredType, regionAdapter);
            var returnedAdapter = regionAdapterMappings.GetMapping(registeredType);

            Assert.NotNull(returnedAdapter);
            Assert.Same(regionAdapter, returnedAdapter);
        }

        [Fact]
        public void ShouldGetRegisteredMapping_UsingGenericControl()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping<ItemsControl>(regionAdapter);

            var returnedAdapter = regionAdapterMappings.GetMapping<ItemsControl>();

            Assert.NotNull(returnedAdapter);
            Assert.Same(regionAdapter, returnedAdapter);
        }

        [Fact]
        public void ShouldGetRegisteredMapping_UsingGenericControlAndAdapter()
        {
            try
            {
                var regionAdapterMappings = new RegionAdapterMappings();
                var regionAdapter = new MockRegionAdapter();

                var containerMock = new Mock<IContainerExtension>();
                containerMock.Setup(c => c.Resolve(typeof(MockRegionAdapter)))
                             .Returns(regionAdapter);
                ContainerLocator.ResetContainer();
                ContainerLocator.SetContainerExtension(() => containerMock.Object);

                regionAdapterMappings.RegisterMapping<ItemsControl, MockRegionAdapter>();

                var returnedAdapter = regionAdapterMappings.GetMapping<ItemsControl>();

                Assert.NotNull(returnedAdapter);
                Assert.Same(regionAdapter, returnedAdapter);
            }
            finally
            {
                ContainerLocator.ResetContainer();
            }
        }

        [Fact]
        public void ShouldGetMappingForDerivedTypesThanTheRegisteredOnes()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var regionAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), regionAdapter);
            var returnedAdapter = regionAdapterMappings.GetMapping(typeof(ItemsControlDescendant));

            Assert.NotNull(returnedAdapter);
            Assert.Same(regionAdapter, returnedAdapter);
        }

        [Fact]
        public void GetMappingOfUnregisteredTypeThrows()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() =>
            {
                var regionAdapterMappings = new RegionAdapterMappings();
                regionAdapterMappings.GetMapping(typeof(object));
            });

        }

        [Fact]
        public void ShouldGetTheMostSpecializedMapping()
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            var genericAdapter = new MockRegionAdapter();
            var specializedAdapter = new MockRegionAdapter();

            regionAdapterMappings.RegisterMapping(typeof(ItemsControl), genericAdapter);
            regionAdapterMappings.RegisterMapping(typeof(ItemsControlDescendant), specializedAdapter);
            var returnedAdapter = regionAdapterMappings.GetMapping(typeof(ItemsControlDescendant));

            Assert.NotNull(returnedAdapter);
            Assert.Same(specializedAdapter, returnedAdapter);
        }

        [Fact]
        public void RegisterAMappingThatAlreadyExistsThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var regionAdapterMappings = new RegionAdapterMappings();
                var regionAdapter = new MockRegionAdapter();

                regionAdapterMappings.RegisterMapping(typeof(ItemsControl), regionAdapter);
                regionAdapterMappings.RegisterMapping(typeof(ItemsControl), regionAdapter);
            });
        }

        [Fact]
        public void NullControlThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var regionAdapterMappings = new RegionAdapterMappings();
                var regionAdapter = new MockRegionAdapter();

                regionAdapterMappings.RegisterMapping(null, regionAdapter);
            });

        }

        [Fact]
        public void NullAdapterThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var regionAdapterMappings = new RegionAdapterMappings();

                regionAdapterMappings.RegisterMapping(typeof(ItemsControl), null);
            });

        }

        class ItemsControlDescendant : ItemsControl { }

    }
}