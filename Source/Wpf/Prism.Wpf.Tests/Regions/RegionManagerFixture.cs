

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionManagerFixture
    {
        [TestMethod]
        public void CanAddRegion()
        {
            IRegion region1 = new MockPresentationRegion();
            region1.Name = "MainRegion";

            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(region1);

            IRegion region2 = regionManager.Regions["MainRegion"];
            Assert.AreSame(region1, region2);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ShouldFailIfRegionDoesntExists()
        {
            RegionManager regionManager = new RegionManager();
            IRegion region = regionManager.Regions["nonExistentRegion"];
        }

        [TestMethod]
        public void CanCheckTheExistenceOfARegion()
        {
            RegionManager regionManager = new RegionManager();
            bool result = regionManager.Regions.ContainsRegionWithName("noRegion");

            Assert.IsFalse(result);

            IRegion region = new MockPresentationRegion();
            region.Name = "noRegion";
            regionManager.Regions.Add(region);

            result = regionManager.Regions.ContainsRegionWithName("noRegion");

            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddingMultipleRegionsWithSameNameThrowsArgumentException()
        {
            var regionManager = new RegionManager();
            regionManager.Regions.Add(new MockPresentationRegion { Name = "region name" });
            regionManager.Regions.Add(new MockPresentationRegion { Name = "region name" });
        }

        [TestMethod]
        public void AddPassesItselfAsTheRegionManagerOfTheRegion()
        {
            var regionManager = new RegionManager();
            var region = new MockPresentationRegion();
            region.Name = "region";
            regionManager.Regions.Add(region);

            Assert.AreSame(regionManager, region.RegionManager);
        }

        [TestMethod]
        public void CreateRegionManagerCreatesANewInstance()
        {
            var regionManager = new RegionManager();
            var createdRegionManager = regionManager.CreateRegionManager();
            Assert.IsNotNull(createdRegionManager);
            Assert.IsInstanceOfType(createdRegionManager, typeof(RegionManager));
            Assert.AreNotSame(regionManager, createdRegionManager);
        }

        [TestMethod]
        public void CanRemoveRegion()
        {
            var regionManager = new RegionManager();
            IRegion region = new MockPresentationRegion();
            region.Name = "TestRegion";
            regionManager.Regions.Add(region);

            regionManager.Regions.Remove("TestRegion");

            Assert.IsFalse(regionManager.Regions.ContainsRegionWithName("TestRegion"));
        }

        [TestMethod]
        public void ShouldRemoveRegionManagerWhenRemoving()
        {
            var regionManager = new RegionManager();
            var region = new MockPresentationRegion();
            region.Name = "TestRegion";
            regionManager.Regions.Add(region);

            regionManager.Regions.Remove("TestRegion");

            Assert.IsNull(region.RegionManager);
        }

        [TestMethod]
        public void UpdatingRegionsGetsCalledWhenAccessingRegionMembers()
        {
            var listener = new MySubscriberClass();

            try
            {
                RegionManager.UpdatingRegions += listener.OnUpdatingRegions;
                RegionManager regionManager = new RegionManager();
                regionManager.Regions.ContainsRegionWithName("TestRegion");
                Assert.IsTrue(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                regionManager.Regions.Add(new MockPresentationRegion() { Name = "TestRegion" });
                Assert.IsTrue(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                var region = regionManager.Regions["TestRegion"];
                Assert.IsTrue(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                regionManager.Regions.Remove("TestRegion");
                Assert.IsTrue(listener.OnUpdatingRegionsCalled);

                listener.OnUpdatingRegionsCalled = false;
                regionManager.Regions.GetEnumerator();
                Assert.IsTrue(listener.OnUpdatingRegionsCalled);
            }
            finally
            {
                RegionManager.UpdatingRegions -= listener.OnUpdatingRegions;
            }
        }


        [TestMethod]
        public void ShouldSetObservableRegionContextWhenRegionContextChanges()
        {
            var region = new MockPresentationRegion();
            var view = new MockDependencyObject();

            var observableObject = RegionContext.GetObservableContext(view);

            bool propertyChangedCalled = false;
            observableObject.PropertyChanged += (sender, args) => propertyChangedCalled = true;

            Assert.IsNull(observableObject.Value);
            RegionManager.SetRegionContext(view, "MyContext");
            Assert.IsTrue(propertyChangedCalled);
            Assert.AreEqual("MyContext", observableObject.Value);
        }

        [TestMethod]
        public void ShouldNotPreventSubscribersToStaticEventFromBeingGarbageCollected()
        {
            var subscriber = new MySubscriberClass();
            RegionManager.UpdatingRegions += subscriber.OnUpdatingRegions;
            RegionManager.UpdateRegions();
            Assert.IsTrue(subscriber.OnUpdatingRegionsCalled);
            WeakReference subscriberWeakReference = new WeakReference(subscriber);

            subscriber = null;
            GC.Collect();

            Assert.IsFalse(subscriberWeakReference.IsAlive);
        }

        [TestMethod]
        public void ExceptionMessageWhenCallingUpdateRegionsShouldBeClear()
        {
            try
            {

                ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException));
                RegionManager.UpdatingRegions += new EventHandler(RegionManager_UpdatingRegions);

                try
                {
                    RegionManager.UpdateRegions();
                    Assert.Fail();
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.Contains("Abcde"));
                }
            }
            finally
            {
                RegionManager.UpdatingRegions -= new EventHandler(RegionManager_UpdatingRegions);
            }
        }

        public void RegionManager_UpdatingRegions(object sender, EventArgs e)
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


        public class MySubscriberClass
        {
            public bool OnUpdatingRegionsCalled;

            public void OnUpdatingRegions(object sender, EventArgs e)
            {
                OnUpdatingRegionsCalled = true;
            }
        }

        [TestMethod]
        public void WhenAddingRegions_ThenRegionsCollectionNotifiesUpdate()
        {
            var regionManager = new RegionManager();

            var region1 = new Region { Name = "region1" };
            var region2 = new Region { Name = "region2" };

            NotifyCollectionChangedEventArgs args = null;
            regionManager.Regions.CollectionChanged += (s, e) => args = e;

            regionManager.Regions.Add(region1);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
            CollectionAssert.AreEqual(new object[] { region1 }, args.NewItems);
            Assert.AreEqual(0, args.NewStartingIndex);
            Assert.IsNull(args.OldItems);
            Assert.AreEqual(-1, args.OldStartingIndex);

            regionManager.Regions.Add(region2);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
            CollectionAssert.AreEqual(new object[] { region2 }, args.NewItems);
            Assert.AreEqual(0, args.NewStartingIndex);
            Assert.IsNull(args.OldItems);
            Assert.AreEqual(-1, args.OldStartingIndex);
        }

        [TestMethod]
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

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
            CollectionAssert.AreEqual(new object[] { region2 }, args.OldItems);
            Assert.AreEqual(0, args.OldStartingIndex);
            Assert.IsNull(args.NewItems);
            Assert.AreEqual(-1, args.NewStartingIndex);

            regionManager.Regions.Remove("region1");

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
            CollectionAssert.AreEqual(new object[] { region1 }, args.OldItems);
            Assert.AreEqual(0, args.OldStartingIndex);
            Assert.IsNull(args.NewItems);
            Assert.AreEqual(-1, args.NewStartingIndex);
        }

        [TestMethod]
        public void WhenRemovingNonExistingRegion_ThenRegionsCollectionDoesNotNotifyUpdate()
        {
            var regionManager = new RegionManager();

            var region1 = new Region { Name = "region1" };

            regionManager.Regions.Add(region1);

            NotifyCollectionChangedEventArgs args = null;
            regionManager.Regions.CollectionChanged += (s, e) => args = e;

            regionManager.Regions.Remove("region2");

            Assert.IsNull(args);
        }

        [TestMethod]
        public void CanAddViewToRegion()
        {
            var regionManager = new RegionManager();
            var view1 = new object();
            var view2 = new object();


            IRegion region = new MockRegion();
            region.Name = "RegionName";
            regionManager.Regions.Add(region);

            regionManager.AddToRegion("RegionName", view1);
            regionManager.AddToRegion("RegionName", view2);

            Assert.IsTrue(regionManager.Regions["RegionName"].Views.Contains(view1));
            Assert.IsTrue(regionManager.Regions["RegionName"].Views.Contains(view2));
        }

        [TestMethod]
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
                ServiceLocator.SetLocatorProvider(
                    () => new MockServiceLocator
                    {
                        GetInstance = t => mockRegionContentRegistry
                    });

                var regionManager = new RegionManager();

                regionManager.RegisterViewWithRegion("Region1", typeof(object));

                Assert.AreEqual(regionName, "Region1");
                Assert.AreEqual(viewType, typeof(object));


            }
            finally
            {
                ServiceLocator.SetLocatorProvider(null);
            }
        }

        [TestMethod]
        public void CanRegisterDelegate()
        {
            try
            {
                var mockRegionContentRegistry = new MockRegionContentRegistry();

                string regionName = null;
                Func<object> contentDelegate = null;

                Func<object> expectedDelegate = () => true;
                mockRegionContentRegistry.RegisterContentWithDelegate = (name, usedDelegate) =>
                {
                    regionName = name;
                    contentDelegate = usedDelegate;
                    return null;
                };
                ServiceLocator.SetLocatorProvider(
                    () => new MockServiceLocator
                    {
                        GetInstance = t => mockRegionContentRegistry
                    });

                var regionManager = new RegionManager();

                regionManager.RegisterViewWithRegion("Region1", expectedDelegate);

                Assert.AreEqual("Region1", regionName);
                Assert.AreEqual(expectedDelegate, contentDelegate);


            }
            finally
            {
                ServiceLocator.SetLocatorProvider(null);
            }
        }

        [TestMethod]
        public void CanAddRegionToRegionManager()
        {
            var regionManager = new RegionManager();
            var region = new MockRegion();

            regionManager.Regions.Add("region", region);

            Assert.AreEqual(1, regionManager.Regions.Count());
            Assert.AreEqual("region", region.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowIfRegionNameArgumentIsDifferentToRegionNameProperty()
        {
            var regionManager = new RegionManager();
            var region = new MockRegion();

            region.Name = "region";

            regionManager.Regions.Add("another region", region);
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
            if (RegisterContentWithViewType != null)
            {
                RegisterContentWithViewType(regionName, viewType);
            }
        }

        void IRegionViewRegistry.RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            if (RegisterContentWithDelegate != null)
            {
                RegisterContentWithDelegate(regionName, getContentDelegate);
            }

        }
    }
}