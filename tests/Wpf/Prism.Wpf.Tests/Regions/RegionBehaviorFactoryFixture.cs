

using System;
using System.Linq;
using Xunit;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

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

            RegionBehaviorFactory factory = new RegionBehaviorFactory(new MockServiceLocator() { GetInstance = (t) => expectedBehavior });

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

    }
}
