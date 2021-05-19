using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Prism.Forms.Regions.Mocks;
using Prism.Ioc;
using Prism.Regions;
using Xamarin.Forms;
using Xunit;
using Region = Prism.Regions.Region;

namespace Prism.Forms.Regions.Tests
{
    public class RegionManagerFixture
    {
        [Fact]
        public void CanAddRegion()
        {
            IRegion region1 = new MockPresentationRegion();
            region1.Name = "MainRegion";

            var regionManager = new RegionManager();
            regionManager.Regions.Add(region1);

            IRegion region2 = regionManager.Regions["MainRegion"];
            Assert.Same(region1, region2);
        }

        [Fact]
        public void ShouldFailIfRegionDoesntExists()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() =>
            {
                var regionManager = new RegionManager();
                IRegion region = regionManager.Regions["nonExistentRegion"];
            });
        }

        [Fact]
        public void CanCheckTheExistenceOfARegion()
        {
            var regionManager = new RegionManager();
            bool result = regionManager.Regions.ContainsRegionWithName("noRegion");

            Assert.False(result);

            IRegion region = new MockPresentationRegion
            {
                Name = "noRegion"
            };
            regionManager.Regions.Add(region);

            result = regionManager.Regions.ContainsRegionWithName("noRegion");

            Assert.True(result);
        }

        [Fact]
        public void AddingMultipleRegionsWithSameNameThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var regionManager = new RegionManager();
                regionManager.Regions.Add(new MockPresentationRegion { Name = "region name" });
                regionManager.Regions.Add(new MockPresentationRegion { Name = "region name" });
            });

        }

        [Fact]
        public void AddPassesItselfAsTheRegionManagerOfTheRegion()
        {
            var regionManager = new RegionManager();
            var region = new MockPresentationRegion
            {
                Name = "region"
            };
            regionManager.Regions.Add(region);

            Assert.Same(regionManager, region.RegionManager);
        }

        [Fact]
        public void CreateRegionManagerCreatesANewInstance()
        {
            var regionManager = new RegionManager();
            var createdRegionManager = regionManager.CreateRegionManager();
            Assert.NotNull(createdRegionManager);
            Assert.IsType<RegionManager>(createdRegionManager);
            Assert.NotSame(regionManager, createdRegionManager);
        }

        [Fact]
        public void CanRemoveRegion()
        {
            var regionManager = new RegionManager();
            IRegion region = new MockPresentationRegion
            {
                Name = "TestRegion"
            };
            regionManager.Regions.Add(region);

            regionManager.Regions.Remove("TestRegion");

            Assert.False(regionManager.Regions.ContainsRegionWithName("TestRegion"));
        }

        [Fact]
        public void ShouldRemoveRegionManagerWhenRemoving()
        {
            var regionManager = new RegionManager();
            var region = new MockPresentationRegion
            {
                Name = "TestRegion"
            };
            regionManager.Regions.Add(region);

            regionManager.Regions.Remove("TestRegion");

            Assert.Null(region.RegionManager);
        }

        [Fact]
        public void UpdatingRegionsGetsCalledWhenAccessingRegionMembers()
        {
            var listener = new MySubscriberClass();

            try
            {
                Prism.Regions.Xaml.RegionManager.UpdatingRegions += listener.OnUpdatingRegions;
                var regionManager = new RegionManager();
                regionManager.Regions.ContainsRegionWithName("TestRegion");
                Assert.True(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                regionManager.Regions.Add(new MockPresentationRegion() { Name = "TestRegion" });
                Assert.True(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                var region = regionManager.Regions["TestRegion"];
                Assert.True(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                regionManager.Regions.Remove("TestRegion");
                Assert.True(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                regionManager.Regions.GetEnumerator();
                Assert.True(listener.OnUpdatingRegionsCalled);
            }
            finally
            {
                Prism.Regions.Xaml.RegionManager.UpdatingRegions -= listener.OnUpdatingRegions;
            }
        }

        [Fact]
        public void ShouldSetObservableRegionContextWhenRegionContextChanges()
        {
            var region = new MockPresentationRegion();
            var view = new ContentView();

            var observableObject = RegionContext.GetObservableContext(view);

            bool propertyChangedCalled = false;
            observableObject.PropertyChanged += (sender, args) => propertyChangedCalled = true;

            Assert.Null(observableObject.Value);
            Prism.Regions.Xaml.RegionManager.SetRegionContext(view, "MyContext");
            Assert.True(propertyChangedCalled);
            Assert.Equal("MyContext", observableObject.Value);
        }

        [Fact]
        public async Task ShouldNotPreventSubscribersToStaticEventFromBeingGarbageCollected()
        {
            var subscriber = new MySubscriberClass();
            Prism.Regions.Xaml.RegionManager.UpdatingRegions += subscriber.OnUpdatingRegions;
            Prism.Regions.Xaml.RegionManager.UpdateRegions();
            Assert.True(subscriber.OnUpdatingRegionsCalled);
            var subscriberWeakReference = new WeakReference(subscriber);

            subscriber = null;
            await Task.Delay(100);
            GC.Collect();

            Assert.False(subscriberWeakReference.IsAlive);
        }

        [Fact]
        public void ExceptionMessageWhenCallingUpdateRegionsShouldBeClear()
        {
            try
            {
                ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException));
                Prism.Regions.Xaml.RegionManager.UpdatingRegions += new EventHandler(RegionManager_UpdatingRegions);

                try
                {
                    Prism.Regions.Xaml.RegionManager.UpdateRegions();
                    //Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.Contains("Abcde", ex.Message);
                }
            }
            finally
            {
                Prism.Regions.Xaml.RegionManager.UpdatingRegions -= new EventHandler(RegionManager_UpdatingRegions);
            }
        }

        private void RegionManager_UpdatingRegions(object sender, EventArgs e)
        {
            try
            {
                throw new Exception("Abcde");
            }
            catch (Exception ex)
            {
                throw new FrameworkException(ex);
            }
        }


        internal class MySubscriberClass
        {
            public bool OnUpdatingRegionsCalled;

            public void OnUpdatingRegions(object sender, EventArgs e)
            {
                OnUpdatingRegionsCalled = true;
            }
        }

        [Fact]
        public void WhenAddingRegions_ThenRegionsCollectionNotifiesUpdate()
        {
            var regionManager = new RegionManager();

            var region1 = new Region { Name = "region1" };
            var region2 = new Region { Name = "region2" };

            NotifyCollectionChangedEventArgs args = null;
            regionManager.Regions.CollectionChanged += (s, e) => args = e;

            regionManager.Regions.Add(region1);

            Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
            Assert.Equal(new object[] { region1 }, args.NewItems);
            Assert.Equal(0, args.NewStartingIndex);
            Assert.Null(args.OldItems);
            Assert.Equal(-1, args.OldStartingIndex);

            regionManager.Regions.Add(region2);

            Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
            Assert.Equal(new object[] { region2 }, args.NewItems);
            Assert.Equal(0, args.NewStartingIndex);
            Assert.Null(args.OldItems);
            Assert.Equal(-1, args.OldStartingIndex);
        }

        [Fact]
        public void WhenRemovingRegions_ThenRegionsCollectionNotifiesUpdate()
        {
            var regionManager = new RegionManager();

            var region1 = new Region { Name = "region1" };
            var region2 = new Region { Name = "region2" };

            regionManager.Regions.Add(region1);
            regionManager.Regions.Add(region2);

            NotifyCollectionChangedEventArgs args = null;
            regionManager.Regions.CollectionChanged += (s, e) => args = e;

            regionManager.Regions.Remove("region2");

            Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
            Assert.Equal(new object[] { region2 }, args.OldItems);
            Assert.Equal(0, args.OldStartingIndex);
            Assert.Null(args.NewItems);
            Assert.Equal(-1, args.NewStartingIndex);

            regionManager.Regions.Remove("region1");

            Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
            Assert.Equal(new object[] { region1 }, args.OldItems);
            Assert.Equal(0, args.OldStartingIndex);
            Assert.Null(args.NewItems);
            Assert.Equal(-1, args.NewStartingIndex);
        }

        [Fact]
        public void WhenRemovingNonExistingRegion_ThenRegionsCollectionDoesNotNotifyUpdate()
        {
            var regionManager = new RegionManager();

            var region1 = new Region { Name = "region1" };

            regionManager.Regions.Add(region1);

            NotifyCollectionChangedEventArgs args = null;
            regionManager.Regions.CollectionChanged += (s, e) => args = e;

            regionManager.Regions.Remove("region2");

            Assert.Null(args);
        }

        //[Fact]
        //public void CanAddViewToRegion()
        //{
        //    var regionManager = new RegionManager();
        //    var view1 = new ContentView();
        //    var view2 = new StackLayout();


        //    IRegion region = new MockRegion
        //    {
        //        Name = "RegionName"
        //    };
        //    regionManager.Regions.Add(region);

        //    regionManager.AddToRegion("RegionName", view1);
        //    regionManager.AddToRegion("RegionName", view2);

        //    Assert.True(regionManager.Regions["RegionName"].Views.Contains(view1));
        //    Assert.True(regionManager.Regions["RegionName"].Views.Contains(view2));
        //}

        [Fact]
        public void CanRegisterViewType()
        {
            try
            {
                var mockRegionContentRegistry = new MockRegionContentRegistry();

                string regionName = null;
                Type viewType = null;

                mockRegionContentRegistry.RegisterContentWithViewType = (name, type) =>
                {
                    regionName = name;
                    viewType = type;
                    return null;
                };
                var containerMock = new Mock<IContainerExtension>();
                containerMock.Setup(c => c.Resolve(typeof(IRegionViewRegistry))).Returns(mockRegionContentRegistry);
                ContainerLocator.SetContainerExtension(() => containerMock.Object);

                var regionManager = new RegionManager();

                regionManager.RegisterViewWithRegion("Region1", typeof(object));

                Assert.Equal("Region1", regionName);
                Assert.Equal(typeof(object), viewType);


            }
            finally
            {
                ContainerLocator.ResetContainer();
            }
        }

        [Fact]
        public void CanRegisterViewTypeGeneric()
        {
            try
            {
                var mockRegionContentRegistry = new MockRegionContentRegistry();

                string regionName = null;
                Type viewType = null;

                mockRegionContentRegistry.RegisterContentWithViewType = (name, type) =>
                {
                    regionName = name;
                    viewType = type;
                    return null;
                };
                var containerMock = new Mock<IContainerExtension>();
                containerMock.Setup(c => c.Resolve(typeof(IRegionViewRegistry))).Returns(mockRegionContentRegistry);
                ContainerLocator.ResetContainer();
                ContainerLocator.SetContainerExtension(() => containerMock.Object);

                var regionManager = new RegionManager();

                regionManager.RegisterViewWithRegion<object>("Region1");

                Assert.Equal("Region1", regionName);
                Assert.Equal(typeof(object), viewType);


            }
            finally
            {
                ContainerLocator.ResetContainer();
            }
        }

        //[Fact]
        //public void CanRegisterDelegate()
        //{
        //    try
        //    {
        //        ContainerLocator.ResetContainer();
        //        var mockRegionContentRegistry = new MockRegionContentRegistry();

        //        string regionName = null;
        //        Func<object> contentDelegate = null;

        //        Func<object> expectedDelegate = () => true;
        //        mockRegionContentRegistry.RegisterContentWithDelegate = (name, usedDelegate) =>
        //        {
        //            regionName = name;
        //            contentDelegate = usedDelegate;
        //            return null;
        //        };
        //        var containerMock = new Mock<IContainerExtension>();
        //        containerMock.Setup(c => c.Resolve(typeof(IRegionViewRegistry))).Returns(mockRegionContentRegistry);
        //        ContainerLocator.SetContainerExtension(() => containerMock.Object);

        //        var regionManager = new RegionManager();

        //        regionManager.RegisterViewWithRegion("Region1", expectedDelegate);

        //        Assert.Equal("Region1", regionName);
        //        Assert.Equal(expectedDelegate, contentDelegate);
        //    }
        //    finally
        //    {
        //        ContainerLocator.ResetContainer();
        //    }
        //}

        [Fact]
        public void CanAddRegionToRegionManager()
        {
            var regionManager = new RegionManager();
            var region = new MockRegion();

            regionManager.Regions.Add("region", region);

            Assert.Single(regionManager.Regions);
            Assert.Equal("region", region.Name);
        }

        [Fact]
        public void ShouldThrowIfRegionNameArgumentIsDifferentToRegionNameProperty()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                var regionManager = new RegionManager();
                var region = new MockRegion
                {
                    Name = "region"
                };

                regionManager.Regions.Add("another region", region);
            });
        }
    }

    internal class FrameworkException : Exception
    {
        public FrameworkException(Exception inner)
            : base(string.Empty, inner)
        {

        }
    }

    internal class MockRegionContentRegistry : IRegionViewRegistry
    {
        public Func<string, Type, object> RegisterContentWithViewType;
        public Func<string, Func<object>, object> RegisterContentWithDelegate;
        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;
        public IEnumerable<object> GetContents(string regionName)
        {
            return null;
        }

        void IRegionViewRegistry.RegisterViewWithRegion(string regionName, Type viewType)
        {
            RegisterContentWithViewType?.Invoke(regionName, viewType);
        }

        void IRegionViewRegistry.RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            RegisterContentWithDelegate?.Invoke(regionName, getContentDelegate);

        }
    }
}
