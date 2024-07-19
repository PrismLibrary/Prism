

using System;
using System.Linq;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Prism.Wpf.Tests.Mocks;
using Xunit;

namespace Prism.Wpf.Tests.Regions
{

    public class RegionBehaviorFactoryFixture
    {
        [Fact]
        public void CanRegisterType()
        {
            RegionBehaviorFactory factory = new RegionBehaviorFactory(null);

            factory.AddIfMissing("key1", typeof(MockRegionBehavior));
            factory.AddIfMissing("key2", typeof(MockRegionBehavior));

            Assert.Equal(2, factory.Count());
            Assert.True(factory.ContainsKey("key1"));

        }

        [Fact]
        public void WillNotAddTypesWithDuplicateKeys()
        {
            RegionBehaviorFactory factory = new RegionBehaviorFactory(null);

            factory.AddIfMissing("key1", typeof(MockRegionBehavior));
            factory.AddIfMissing("key1", typeof(MockRegionBehavior));

            Assert.Single(factory);
        }

        [Fact]
        public void AddTypeThatDoesNotInheritFromIRegionBehaviorThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                RegionBehaviorFactory factory = new RegionBehaviorFactory(null);

                factory.AddIfMissing("key1", typeof(object));
            });

        }

        [Fact]
        public void CanCreateRegisteredType()
        {
            var expectedBehavior = new MockRegionBehavior();
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(c => c.Resolve(typeof(MockRegionBehavior))).Returns(expectedBehavior);
            RegionBehaviorFactory factory = new RegionBehaviorFactory(containerMock.Object);

            factory.AddIfMissing("key1", typeof(MockRegionBehavior));
            var behavior = factory.CreateFromKey("key1");

            Assert.Same(expectedBehavior, behavior);
        }

        [Fact]
        public void CreateWithUnknownKeyThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                RegionBehaviorFactory factory = new RegionBehaviorFactory(null);

                factory.CreateFromKey("Key1");
            });

        }

        [Fact]
        public void ExistingBehavior_IsReplaced_WithCustomBehavior()
        {
            var expectedBehavior = new MockRegionBehaviorB();
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(c => c.Resolve(typeof(MockRegionBehaviorB))).Returns(expectedBehavior);

            RegionBehaviorFactory factory = new RegionBehaviorFactory(containerMock.Object);

            factory.AddIfMissing<MockRegionBehavior>("key1");
            factory.AddOrReplace<MockRegionBehaviorB>("key1");

            Assert.Single(factory);
            Assert.True(factory.ContainsKey("key1"));
            Assert.IsType<MockRegionBehaviorB>(factory.CreateFromKey("key1"));
        }

        [Fact]
        public void MissingBehavior_IsAdded()
        {
            var expectedBehavior = new MockRegionBehaviorB();
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(c => c.Resolve(typeof(MockRegionBehaviorB))).Returns(expectedBehavior);

            RegionBehaviorFactory factory = new RegionBehaviorFactory(containerMock.Object);

            factory.AddOrReplace<MockRegionBehaviorB>("key1");

            Assert.Single(factory);
            Assert.True(factory.ContainsKey("key1"));
            Assert.IsType<MockRegionBehaviorB>(factory.CreateFromKey("key1"));
        }

    }
}
