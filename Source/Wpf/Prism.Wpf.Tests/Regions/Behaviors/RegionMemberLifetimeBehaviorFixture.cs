

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    [TestClass]
    public class RegionMemberLifetimeBehaviorFixture 
    {
        protected Region Region { get; set; }
        protected RegionMemberLifetimeBehavior Behavior { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Arrange();
        }

        protected virtual void Arrange()
        {
            this.Region = new Region();
            this.Behavior = new RegionMemberLifetimeBehavior();
            this.Behavior.Region = this.Region;
            this.Behavior.Attach();
        }

        [TestMethod]
        public void WhenBehaviorAttachedThenReportsIsAttached()
        {
            Assert.IsTrue(Behavior.IsAttached);
        }

        [TestMethod]
        public void WhenIRegionMemberLifetimeItemReturnsKeepAliveFalseRemovesWhenInactive()
        {
            // Arrange
            var regionItemMock = new Mock<IRegionMemberLifetime>();
            regionItemMock.Setup(i => i.KeepAlive).Returns(false);

            Region.Add(regionItemMock.Object);
            Region.Activate(regionItemMock.Object);

            // Act
            Region.Deactivate(regionItemMock.Object);

            // Assert
            Assert.IsFalse(Region.Views.Contains(regionItemMock.Object));
        }

        [TestMethod]
        public void WhenIRegionMemberLifetimeItemReturnsKeepAliveTrueDoesNotRemoveOnDeactivation()
        {
            // Arrange
            var regionItemMock = new Mock<IRegionMemberLifetime>();
            regionItemMock.Setup(i => i.KeepAlive).Returns(true);

            Region.Add(regionItemMock.Object);
            Region.Activate(regionItemMock.Object);

            // Act
            Region.Deactivate(regionItemMock.Object);

            // Assert
            Assert.IsTrue(Region.Views.Contains(regionItemMock.Object));

        }

        [TestMethod]
        public void WhenRegionContainsMultipleMembers_OnlyRemovesThoseDeactivated()
        {
            // Arrange
            var firstMockItem = new Mock<IRegionMemberLifetime>();
            firstMockItem.Setup(i => i.KeepAlive).Returns(true);

            var secondMockItem = new Mock<IRegionMemberLifetime>();
            secondMockItem.Setup(i => i.KeepAlive).Returns(false);

            Region.Add(firstMockItem.Object);
            Region.Activate(firstMockItem.Object);

            Region.Add(secondMockItem.Object);
            Region.Activate(secondMockItem.Object);

            // Act
            Region.Deactivate(secondMockItem.Object);

            // Assert
            Assert.IsTrue(Region.Views.Contains(firstMockItem.Object));
            Assert.IsFalse(Region.Views.Contains(secondMockItem.Object));
        }

        [TestMethod]
        public void WhenMemberNeverActivatedThenIsNotRemovedOnAnothersDeactivation()
        {
            // Arrange
            var firstMockItem = new Mock<IRegionMemberLifetime>();
            firstMockItem.Setup(i => i.KeepAlive).Returns(false);

            var secondMockItem = new Mock<IRegionMemberLifetime>();
            secondMockItem.Setup(i => i.KeepAlive).Returns(false);

            Region.Add(firstMockItem.Object);  // Never activated

            Region.Add(secondMockItem.Object);
            Region.Activate(secondMockItem.Object);

            // Act
            Region.Deactivate(secondMockItem.Object);

            // Assert
            Assert.IsTrue(Region.Views.Contains(firstMockItem.Object));
            Assert.IsFalse(Region.Views.Contains(secondMockItem.Object));
        }

        [TestMethod]
        public virtual void RemovesRegionItemIfDataContextReturnsKeepAliveFalse()
        {
            // Arrange
            var regionItemMock = new Mock<IRegionMemberLifetime>();
            regionItemMock.Setup(i => i.KeepAlive).Returns(false);

            var regionItem = new MockFrameworkElement();
            regionItem.DataContext = regionItemMock.Object;

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.IsFalse(Region.Views.Contains(regionItem));
        }

        [TestMethod]
        public virtual void RemovesOnlyDeactivatedItemsInRegionBasedOnDataContextKeepAlive()
        {
            // Arrange
            var retionItemDataContextToKeepAlive = new Mock<IRegionMemberLifetime>();
            retionItemDataContextToKeepAlive.Setup(i => i.KeepAlive).Returns(true);

            var regionItemToKeepAlive = new MockFrameworkElement();
            regionItemToKeepAlive.DataContext = retionItemDataContextToKeepAlive.Object;
            Region.Add(regionItemToKeepAlive);
            Region.Activate(regionItemToKeepAlive);

            var regionItemMock = new Mock<IRegionMemberLifetime>();
            regionItemMock.Setup(i => i.KeepAlive).Returns(false);

            var regionItem = new MockFrameworkElement();
            regionItem.DataContext = regionItemMock.Object;

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.IsFalse(Region.Views.Contains(regionItem));
            Assert.IsTrue(Region.Views.Contains(regionItemToKeepAlive));
        }

        [TestMethod]
        public virtual void WillRemoveDeactivatedItemIfKeepAliveAttributeFalse()
        {
            // Arrange
            var regionItem = new RegionMemberNotKeptAlive();

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.IsFalse(Region.Views.Contains((object)regionItem));
        }

        [TestMethod]
        public virtual void WillNotRemoveDeactivatedItemIfKeepAliveAttributeTrue()
        {
            // Arrange
            var regionItem = new RegionMemberKeptAlive();

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.IsTrue(Region.Views.Contains((object)regionItem));
        }

        [TestMethod]
        public virtual void WillRemoveDeactivatedItemIfDataContextKeepAliveAttributeFalse()
        {
            // Arrange
            var regionItemDataContext = new RegionMemberNotKeptAlive();
            var regionItem = new MockFrameworkElement() { DataContext = regionItemDataContext };
            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.IsFalse(Region.Views.Contains(regionItem));
        }

        [RegionMemberLifetime(KeepAlive = false)]
        public class RegionMemberNotKeptAlive
        {
        }

        [RegionMemberLifetime(KeepAlive = true)]
        public class RegionMemberKeptAlive
        {
        }

        
    }

    [TestClass]
    public class RegionMemberLifetimeBehaviorAgainstSingleActiveRegionFixture
                : RegionMemberLifetimeBehaviorFixture
    {
        protected override void Arrange()
        {
            this.Region = new SingleActiveRegion();
            this.Behavior = new RegionMemberLifetimeBehavior();
            this.Behavior.Region = this.Region;
            this.Behavior.Attach();
        }
    }
}
