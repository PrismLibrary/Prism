using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Prism.Forms.Regions.Mocks;
using Prism.Ioc;
using Prism.Regions.Behaviors;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class RegionBehaviorFactoryFixture
    {
        [Fact]
        public void CanRegisterType()
        {
            var factory = new RegionBehaviorFactory(null);

            factory.AddIfMissing<MockRegionBehavior>("key1");
            factory.AddIfMissing<MockRegionBehavior>("key2");

            Assert.Equal(2, factory.Count());
            Assert.True(factory.ContainsKey("key1"));
        }

        [Fact]
        public void WillNotAddTypesWithDuplicateKeys()
        {
            var factory = new RegionBehaviorFactory(null);

            factory.AddIfMissing<MockRegionBehavior>("key1");
            factory.AddIfMissing<MockRegionBehavior>("key1");

            Assert.Single(factory);
        }

        [Fact]
        public void CanCreateRegisteredType()
        {
            var expectedBehavior = new MockRegionBehavior();
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(c => c.Resolve(typeof(MockRegionBehavior))).Returns(expectedBehavior);
            var factory = new RegionBehaviorFactory(containerMock.Object);

            factory.AddIfMissing<MockRegionBehavior>("key1");
            var behavior = factory.CreateFromKey("key1");

            Assert.Same(expectedBehavior, behavior);
        }

        [Fact]
        public void CreateWithUnknownKeyThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var factory = new RegionBehaviorFactory(null);

                factory.CreateFromKey("Key1");
            });
        }
    }
}
