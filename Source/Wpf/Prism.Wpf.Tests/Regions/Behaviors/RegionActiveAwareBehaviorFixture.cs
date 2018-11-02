

using System;
using System.Windows;
using Xunit;
using Moq;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class RegionActiveAwareBehaviorFixture
    {
        [StaFact]
        public void SetsIsActivePropertyOnIActiveAwareObjects()
        {
            var region = new MockPresentationRegion();
            region.RegionManager = new MockRegionManager();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();
            var collection = region.MockActiveViews.Items;

            ActiveAwareFrameworkElement activeAwareObject = new ActiveAwareFrameworkElement();

            Assert.False(activeAwareObject.IsActive);
            collection.Add(activeAwareObject);

            Assert.True(activeAwareObject.IsActive);

            collection.Remove(activeAwareObject);
            Assert.False(activeAwareObject.IsActive);
        }

        [StaFact]
        public void SetsIsActivePropertyOnIActiveAwareDataContexts()
        {
            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();
            var collection = region.MockActiveViews.Items;

            ActiveAwareFrameworkElement activeAwareObject = new ActiveAwareFrameworkElement();

            var frameworkElementMock = new Mock<FrameworkElement>();
            var frameworkElement = frameworkElementMock.Object;
            frameworkElement.DataContext = activeAwareObject;

            Assert.False(activeAwareObject.IsActive);
            collection.Add(frameworkElement);

            Assert.True(activeAwareObject.IsActive);

            collection.Remove(frameworkElement);
            Assert.False(activeAwareObject.IsActive);
        }

        [StaFact]
        public void SetsIsActivePropertyOnBothIActiveAwareViewAndDataContext()
        {
            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();
            var collection = region.MockActiveViews.Items;

            var activeAwareMock = new Mock<IActiveAware>();
            activeAwareMock.SetupProperty(o => o.IsActive);
            var activeAwareObject = activeAwareMock.Object;

            var frameworkElementMock = new Mock<FrameworkElement>();
            frameworkElementMock.As<IActiveAware>().SetupProperty(o => o.IsActive);
            var frameworkElement = frameworkElementMock.Object;
            frameworkElement.DataContext = activeAwareObject;

            Assert.False(((IActiveAware)frameworkElement).IsActive);
            Assert.False(activeAwareObject.IsActive);
            collection.Add(frameworkElement);

            Assert.True(((IActiveAware)frameworkElement).IsActive);
            Assert.True(activeAwareObject.IsActive);

            collection.Remove(frameworkElement);
            Assert.False(((IActiveAware)frameworkElement).IsActive);
            Assert.False(activeAwareObject.IsActive);
        }

        [StaFact]
        public void DetachStopsListeningForChanges()
        {
            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            var collection = region.MockActiveViews.Items;
            behavior.Attach();
            behavior.Detach();
            ActiveAwareFrameworkElement activeAwareObject = new ActiveAwareFrameworkElement();

            collection.Add(activeAwareObject);

            Assert.False(activeAwareObject.IsActive);
        }

        [StaFact]
        public void DoesNotThrowWhenAddingNonActiveAwareObjects()
        {
            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();
            var collection = region.MockActiveViews.Items;

            collection.Add(new object());
        }

        [StaFact]
        public void DoesNotThrowWhenAddingNonActiveAwareDataContexts()
        {
            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();
            var collection = region.MockActiveViews.Items;

            var frameworkElementMock = new Mock<FrameworkElement>();
            var frameworkElement = frameworkElementMock.Object;
            frameworkElement.DataContext = new object();


            collection.Add(frameworkElement);
        }

        [StaFact]
        public void WhenParentViewGetsActivatedOrDeactivated_ThenChildViewIsNotUpdated()
        {
            var scopedRegionManager = new RegionManager();
            var scopedRegion = new Region { Name = "MyScopedRegion", RegionManager = scopedRegionManager };
            scopedRegionManager.Regions.Add(scopedRegion);
            var behaviorForScopedRegion = new RegionActiveAwareBehavior { Region = scopedRegion };
            behaviorForScopedRegion.Attach();
            var childActiveAwareView = new ActiveAwareFrameworkElement();

            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();

            var view = new MockFrameworkElement();
            region.Add(view);
            RegionManager.SetRegionManager(view, scopedRegionManager);
            region.Activate(view);

            scopedRegion.Add(childActiveAwareView);
            scopedRegion.Activate(childActiveAwareView);

            Assert.True(childActiveAwareView.IsActive);

            region.Deactivate(view);

            Assert.True(childActiveAwareView.IsActive);
        }

        [StaFact]
        public void WhenParentViewGetsActivatedOrDeactivated_ThenSyncedChildViewIsUpdated()
        {
            var scopedRegionManager = new RegionManager();
            var scopedRegion = new Region { Name = "MyScopedRegion", RegionManager = scopedRegionManager };
            scopedRegionManager.Regions.Add(scopedRegion);
            var behaviorForScopedRegion = new RegionActiveAwareBehavior { Region = scopedRegion };
            behaviorForScopedRegion.Attach();
            var childActiveAwareView = new SyncedActiveAwareObject();

            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();

            var view = new MockFrameworkElement();
            region.Add(view);
            RegionManager.SetRegionManager(view, scopedRegionManager);
            region.Activate(view);

            scopedRegion.Add(childActiveAwareView);
            scopedRegion.Activate(childActiveAwareView);

            Assert.True(childActiveAwareView.IsActive);

            region.Deactivate(view);

            Assert.False(childActiveAwareView.IsActive);
        }

        [StaFact]
        public void WhenParentViewGetsActivatedOrDeactivated_ThenSyncedChildViewWithAttributeInVMIsUpdated()
        {
            var scopedRegionManager = new RegionManager();
            var scopedRegion = new Region { Name = "MyScopedRegion", RegionManager = scopedRegionManager };
            scopedRegionManager.Regions.Add(scopedRegion);
            var behaviorForScopedRegion = new RegionActiveAwareBehavior { Region = scopedRegion };
            behaviorForScopedRegion.Attach();
            var childActiveAwareView = new ActiveAwareFrameworkElement();
            childActiveAwareView.DataContext = new SyncedActiveAwareObjectViewModel();

            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();

            var view = new MockFrameworkElement();
            region.Add(view);
            RegionManager.SetRegionManager(view, scopedRegionManager);
            region.Activate(view);

            scopedRegion.Add(childActiveAwareView);
            scopedRegion.Activate(childActiveAwareView);

            Assert.True(childActiveAwareView.IsActive);

            region.Deactivate(view);

            Assert.False(childActiveAwareView.IsActive);
        }

        [StaFact]
        public void WhenParentViewGetsActivatedOrDeactivated_ThenSyncedChildViewModelThatIsNotAFrameworkElementIsNotUpdated()
        {
            var scopedRegionManager = new RegionManager();
            var scopedRegion = new Region { Name = "MyScopedRegion", RegionManager = scopedRegionManager };
            scopedRegionManager.Regions.Add(scopedRegion);
            var behaviorForScopedRegion = new RegionActiveAwareBehavior { Region = scopedRegion };
            behaviorForScopedRegion.Attach();
            var childActiveAwareView = new ActiveAwareObject();            

            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();

            var view = new MockFrameworkElement();
            region.Add(view);
            RegionManager.SetRegionManager(view, scopedRegionManager);
            region.Activate(view);

            scopedRegion.Add(childActiveAwareView);
            scopedRegion.Activate(childActiveAwareView);

            Assert.True(childActiveAwareView.IsActive);

            region.Deactivate(view);

            Assert.True(childActiveAwareView.IsActive);
        }

        [StaFact]
        public void WhenParentViewGetsActivatedOrDeactivated_ThenSyncedChildViewNotInActiveViewsIsNotUpdated()
        {
            var scopedRegionManager = new RegionManager();
            var scopedRegion = new Region { Name="MyScopedRegion", RegionManager = scopedRegionManager };
            scopedRegionManager.Regions.Add(scopedRegion);
            var behaviorForScopedRegion = new RegionActiveAwareBehavior { Region = scopedRegion };
            behaviorForScopedRegion.Attach();
            var childActiveAwareView = new SyncedActiveAwareObject();

            var region = new MockPresentationRegion();
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();

            var view = new MockFrameworkElement();
            region.Add(view);
            RegionManager.SetRegionManager(view, scopedRegionManager);
            region.Activate(view);

            scopedRegion.Add(childActiveAwareView);
            scopedRegion.Deactivate(childActiveAwareView);

            Assert.False(childActiveAwareView.IsActive);

            region.Deactivate(view);

            Assert.False(childActiveAwareView.IsActive);

            region.Activate(view);

            Assert.False(childActiveAwareView.IsActive);
        }

        [StaFact]
        public void WhenParentViewWithoutScopedRegionGetsActivatedOrDeactivated_ThenSyncedChildViewIsNotUpdated()
        {
            var commonRegionManager = new RegionManager();
            var nonScopedRegion = new Region { Name="MyRegion", RegionManager = commonRegionManager };
            commonRegionManager.Regions.Add(nonScopedRegion);
            var behaviorForScopedRegion = new RegionActiveAwareBehavior { Region = nonScopedRegion };
            behaviorForScopedRegion.Attach();
            var childActiveAwareView = new SyncedActiveAwareObject();

            var region = new MockPresentationRegion { RegionManager = commonRegionManager };
            var behavior = new RegionActiveAwareBehavior { Region = region };
            behavior.Attach();

            var view = new MockFrameworkElement();
            region.Add(view);
            RegionManager.SetRegionManager(view, commonRegionManager);
            region.Activate(view);

            nonScopedRegion.Add(childActiveAwareView);
            nonScopedRegion.Activate(childActiveAwareView);

            Assert.True(childActiveAwareView.IsActive);

            region.Deactivate(view);

            Assert.True(childActiveAwareView.IsActive);
        }

        class ActiveAwareObject : IActiveAware
        {
            public bool IsActive { get; set; }
            public event EventHandler IsActiveChanged;
        }

        class ActiveAwareFrameworkElement : FrameworkElement, IActiveAware
        {
            public bool IsActive { get; set; }
            public event EventHandler IsActiveChanged;
        }

        [SyncActiveState]
        class SyncedActiveAwareObject : IActiveAware
        {
            public bool IsActive { get; set; }
            public event EventHandler IsActiveChanged;
        }

        [SyncActiveState]
        class SyncedActiveAwareObjectViewModel : IActiveAware
        {
            public bool IsActive { get; set; }
            public event EventHandler IsActiveChanged;
        }
        
    }
}