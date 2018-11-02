

using System;
using System.Windows;
using Xunit;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;
using Prism.Common;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class SyncRegionContextWithHostBehaviorFixture
    {
        [StaFact]
        public void ShouldForwardRegionContextValueToHostControl()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            DependencyObject mockDependencyObject = new MockDependencyObject();
            behavior.HostControl = mockDependencyObject;

            behavior.Attach();
            Assert.Null(region.Context);
            RegionContext.GetObservableContext(mockDependencyObject).Value = "NewValue";

            Assert.Equal("NewValue", region.Context);

        }

        [StaFact]
        public void ShouldUpdateHostControlRegionContextValueWhenContextOfRegionChanges()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            DependencyObject mockDependencyObject = new MockDependencyObject();
            behavior.HostControl = mockDependencyObject;

            ObservableObject<object> observableRegionContext = RegionContext.GetObservableContext(mockDependencyObject);

            behavior.Attach();
            Assert.Null(observableRegionContext.Value);
            region.Context = "NewValue";

            Assert.Equal("NewValue", observableRegionContext.Value);

        }

        [StaFact]
        public void ShouldGetInitialValueFromHostAndSetOnRegion()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            DependencyObject mockDependencyObject = new MockDependencyObject();
            behavior.HostControl = mockDependencyObject;

            RegionContext.GetObservableContext(mockDependencyObject).Value = "NewValue";

            Assert.Null(region.Context);
            behavior.Attach();
            Assert.Equal("NewValue", region.Context);

        }

        [StaFact]
        public void AttachShouldNotThrowWhenHostControlNull()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            behavior.Attach();
        }

        [StaFact]
        public void AttachShouldNotThrowWhenHostControlNullAndRegionContextSet()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            behavior.Attach();
            region.Context = "Changed";
        }

        [StaFact]
        public void ChangingRegionContextObservableObjectValueShouldAlsoChangeRegionContextDependencyProperty()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            DependencyObject hostControl = new MockDependencyObject();
            behavior.HostControl = hostControl;

            behavior.Attach();

            Assert.Null(RegionManager.GetRegionContext(hostControl));
            RegionContext.GetObservableContext(hostControl).Value = "NewValue";

            Assert.Equal("NewValue", RegionManager.GetRegionContext(hostControl));
        }

        [StaFact]
        public void AttachShouldChangeRegionContextDependencyProperty()
        {
            MockPresentationRegion region = new MockPresentationRegion();

            SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
            behavior.Region = region;
            DependencyObject hostControl = new MockDependencyObject();
            behavior.HostControl = hostControl;

            RegionContext.GetObservableContext(hostControl).Value = "NewValue";

            Assert.Null(RegionManager.GetRegionContext(hostControl));
            behavior.Attach();
            Assert.Equal("NewValue", RegionManager.GetRegionContext(hostControl));  
        }

        [StaFact]
        public void SettingHostControlAfterAttachThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                SyncRegionContextWithHostBehavior behavior = new SyncRegionContextWithHostBehavior();
                DependencyObject hostControl1 = new MockDependencyObject();
                behavior.HostControl = hostControl1;

                behavior.Attach();
                DependencyObject hostControl2 = new MockDependencyObject();
                behavior.HostControl = hostControl2;
            });

        }
    }
}