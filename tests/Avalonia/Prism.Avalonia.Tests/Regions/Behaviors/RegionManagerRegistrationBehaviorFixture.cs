using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Prism.Avalonia.Tests.Mocks;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Xunit;

namespace Prism.Avalonia.Tests.Regions.Behaviors
{
    /// <summary>
    /// Region Manager Registration Behavior Fixture tests.
    /// </summary>
    /// <remarks>
    ///   The MockFrameworkElement depends on the following:
    ///   Avalonia.Control's LoadedEvent and UnloadedEvent wont arrive until Avalonia v0.11.0.
    ///   Discussion: https://github.com/AvaloniaUI/Avalonia/issues/7908
    ///   PR: https://github.com/AvaloniaUI/Avalonia/pull/8277
    /// </remarks>
    public class RegionManagerRegistrationBehaviorFixture
    {
        [StaFact]
        public void ShouldRegisterRegionIfRegionManagerIsSet()
        {
            var control = new ItemsControl();
            var regionManager = new MockRegionManager();
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionManager = d => regionManager
            };
            var region = new MockPresentationRegion() { Name = "myRegionName" };
            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = region,
                HostControl = control
            };

            behavior.Attach();

            Assert.True(regionManager.MockRegionCollection.AddCalled);
            Assert.Same(region, regionManager.MockRegionCollection.AddArgument);
        }

        [StaFact]
        public void DoesNotFailIfRegionManagerIsNotSet()
        {
            var control = new ItemsControl();
            var accessor = new MockRegionManagerAccessor();

            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = new MockPresentationRegion() { Name = "myRegionWithoutManager" },
                HostControl = control
            };
            behavior.Attach();
        }

        [StaFact(Skip = "Review: Potentially not supported")]
        public void RegionGetsAddedInRegionManagerWhenAddedIntoAScopeAndAccessingRegions()
        {
            var regionManager = new MockRegionManager();
            var control = new MockFrameworkElement();

            var regionScopeControl = new ContentControl();
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionManager = d => d == regionScopeControl ? regionManager : null
            };

            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = new MockPresentationRegion() { Name = "myRegionName" },
                HostControl = control
            };
            behavior.Attach();

            Assert.False(regionManager.MockRegionCollection.AddCalled);

            regionScopeControl.Content = control;
            accessor.UpdateRegions();

            Assert.True(regionManager.MockRegionCollection.AddCalled);
        }

        [StaFact(Skip = "Review: Potentially not supported")]
        public void RegionDoesNotGetAddedTwiceWhenUpdatingRegions()
        {
            var regionManager = new MockRegionManager();
            var control = new MockFrameworkElement();

            var regionScopeControl = new ContentControl();
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionManager = d => d == regionScopeControl ? regionManager : null
            };

            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = new MockPresentationRegion() { Name = "myRegionName" },
                HostControl = control
            };
            behavior.Attach();

            Assert.False(regionManager.MockRegionCollection.AddCalled);

            regionScopeControl.Content = control;
            accessor.UpdateRegions();

            Assert.True(regionManager.MockRegionCollection.AddCalled);
            regionManager.MockRegionCollection.AddCalled = false;

            accessor.UpdateRegions();
            Assert.False(regionManager.MockRegionCollection.AddCalled);
        }

        [StaFact(Skip = "Review: Potentially not supported")]
        public void RegionGetsRemovedFromRegionManagerWhenRemovedFromScope()
        {
            var regionManager = new MockRegionManager();
            var control = new MockFrameworkElement();
            var regionScopeControl = new ContentControl();
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionManager = d => d == regionScopeControl ? regionManager : null
            };

            var region = new MockPresentationRegion() { Name = "myRegionName" };
            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = region,
                HostControl = control
            };
            behavior.Attach();

            regionScopeControl.Content = control;
            accessor.UpdateRegions();
            Assert.True(regionManager.MockRegionCollection.AddCalled);
            Assert.Same(region, regionManager.MockRegionCollection.AddArgument);

            regionScopeControl.Content = null;
            accessor.UpdateRegions();

            Assert.True(regionManager.MockRegionCollection.RemoveCalled);
        }

        [StaFact]
        public void CanAttachBeforeSettingName()
        {
            var control = new ItemsControl();
            var regionManager = new MockRegionManager();
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionManager = d => regionManager
            };
            var region = new MockPresentationRegion() { Name = null };
            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = region,
                HostControl = control
            };

            behavior.Attach();
            Assert.False(regionManager.MockRegionCollection.AddCalled);

            region.Name = "myRegionName";

            Assert.True(regionManager.MockRegionCollection.AddCalled);
            Assert.Same(region, regionManager.MockRegionCollection.AddArgument);
        }

        [StaFact]
        public void HostControlSetAfterAttachThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var behavior = new RegionManagerRegistrationBehavior();
                var hostControl1 = new MockDependencyObject();
                var hostControl2 = new MockDependencyObject();
                behavior.HostControl = hostControl1;
                behavior.Attach();
                behavior.HostControl = hostControl2;
            });

        }

        [StaFact]
        public async Task BehaviorDoesNotPreventRegionManagerFromBeingGarbageCollected()
        {
            var control = new MockFrameworkElement();
            var regionManager = new MockRegionManager();
            var regionManagerWeakReference = new WeakReference(regionManager);

            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName",
                GetRegionManager = d => regionManager
            };

            var behavior = new RegionManagerRegistrationBehavior()
            {
                RegionManagerAccessor = accessor,
                Region = new MockPresentationRegion(),
                HostControl = control
            };

            behavior.Attach();

            Assert.True(regionManagerWeakReference.IsAlive);
            GC.KeepAlive(regionManager);

            regionManager = null;
            await Task.Delay(50);

            GC.Collect();

            Assert.False(regionManagerWeakReference.IsAlive);
        }

        internal class MockRegionManager : IRegionManager
        {
            public MockRegionCollection MockRegionCollection = new MockRegionCollection();

            #region IRegionManager Members

            public IRegionCollection Regions
            {
                get { return MockRegionCollection; }
            }

            IRegionManager IRegionManager.CreateRegionManager()
            {
                throw new System.NotImplementedException();
            }

            public IRegionManager AddToRegion(string regionName, object view)
            {
                throw new NotImplementedException();
            }

            public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
            {
                throw new NotImplementedException();
            }

            public IRegionManager RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate)
            { 
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, Uri source)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, string source)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, Uri target, INavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, string target, INavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            #endregion

            public bool Navigate(Uri source)
            {
                throw new NotImplementedException();
            }

            public IRegionManager AddToRegion(string regionName, string viewName)
            {
                throw new NotImplementedException();
            }

            public IRegionManager RegisterViewWithRegion(string regionName, string viewName)
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class MockRegionCollection : IRegionCollection
    {
        public bool RemoveCalled;
        public bool AddCalled;
        public IRegion AddArgument;

        IEnumerator<IRegion> IEnumerable<IRegion>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public IRegion this[string regionName]
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Add(IRegion region)
        {
            AddCalled = true;
            AddArgument = region;
        }

        public bool Remove(string regionName)
        {
            RemoveCalled = true;
            return true;
        }

        public bool ContainsRegionWithName(string regionName)
        {
            throw new System.NotImplementedException();
        }

        public void Add(string regionName, IRegion region)
        {
            throw new NotImplementedException();
        }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
