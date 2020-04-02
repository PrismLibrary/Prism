

using Xunit;
using Moq;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class RegionMemberLifetimeBehaviorFixture 
    {
        protected Region Region { get; set; }
        protected RegionMemberLifetimeBehavior Behavior { get; set; }

        public RegionMemberLifetimeBehaviorFixture()
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

        [Fact]
        public void WhenBehaviorAttachedThenReportsIsAttached()
        {
            Assert.True(Behavior.IsAttached);
        }

        [Fact]
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
            Assert.False(Region.Views.Contains(regionItemMock.Object));
        }

        [Fact]
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
            Assert.True(Region.Views.Contains(regionItemMock.Object));

        }

        [Fact]
        public void WhenIRegionMemberLifetimeItemReturnsKeepAliveFalseCanRemoveFromRegion()
        {
            // Arrange
            var regionItemMock = new Mock<IRegionMemberLifetime>();
            regionItemMock.Setup(i => i.KeepAlive).Returns(false);

            var view = regionItemMock.Object;

            Region.Add(view);
            Region.Activate(view);

            // The presence of the following two lines is essential for the test:
            // we want to access both ActiveView and Views in that order
            Assert.True(Region.ActiveViews.Contains(view));
            Assert.True(Region.Views.Contains(view));

            // Act
            // This may throw
            Region.Remove(view);

            // Assert
            Assert.False(Region.Views.Contains(view));
            Assert.False(Region.ActiveViews.Contains(view));
        }

        [Fact]
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
            Assert.True(Region.Views.Contains(firstMockItem.Object));
            Assert.False(Region.Views.Contains(secondMockItem.Object));
        }

        [Fact]
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
            Assert.True(Region.Views.Contains(firstMockItem.Object));
            Assert.False(Region.Views.Contains(secondMockItem.Object));
        }

        [StaFact]
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
            Assert.False(Region.Views.Contains(regionItem));
        }

        [StaFact]
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
            Assert.False(Region.Views.Contains(regionItem));
            Assert.True(Region.Views.Contains(regionItemToKeepAlive));
        }

        [Fact]
        public virtual void WillRemoveDeactivatedItemIfKeepAliveAttributeFalse()
        {
            // Arrange
            var regionItem = new RegionMemberNotKeptAlive();

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.False(Region.Views.Contains((object)regionItem));
        }

        [Fact]
        public virtual void WillNotRemoveDeactivatedItemIfKeepAliveAttributeTrue()
        {
            // Arrange
            var regionItem = new RegionMemberKeptAlive();

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.True(Region.Views.Contains((object)regionItem));
        }

        [StaFact]
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
            Assert.False(Region.Views.Contains(regionItem));
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
