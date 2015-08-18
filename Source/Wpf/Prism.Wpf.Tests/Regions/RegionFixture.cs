

using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionFixture
    {
        [TestMethod]
        public void WhenRegionConstructed_SortComparisonIsDefault()
        {
            IRegion region = new Region();

            Assert.IsNotNull(region.SortComparison);
            Assert.AreEqual(region.SortComparison, Region.DefaultSortComparison);
        }

        [TestMethod]
        public void CanAddContentToRegion()
        {
            IRegion region = new Region();

            Assert.AreEqual(0, region.Views.Cast<object>().Count());

            region.Add(new object());

            Assert.AreEqual(1, region.Views.Cast<object>().Count());
        }


        [TestMethod]
        public void CanRemoveContentFromRegion()
        {
            IRegion region = new Region();
            object view = new object();

            region.Add(view);
            region.Remove(view);

            Assert.AreEqual(0, region.Views.Cast<object>().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveInexistentViewThrows()
        {
            IRegion region = new Region();
            object view = new object();

            region.Remove(view);

            Assert.AreEqual(0, region.Views.Cast<object>().Count());
        }

        [TestMethod]
        public void RegionExposesCollectionOfContainedViews()
        {
            IRegion region = new Region();

            object view = new object();

            region.Add(view);

            var views = region.Views;

            Assert.IsNotNull(views);
            Assert.AreEqual(1, views.Cast<object>().Count());
            Assert.AreSame(view, views.Cast<object>().ElementAt(0));
        }

        [TestMethod]
        public void CanAddAndRetrieveNamedViewInstance()
        {
            IRegion region = new Region();
            object myView = new object();
            region.Add(myView, "MyView");
            object returnedView = region.GetView("MyView");

            Assert.IsNotNull(returnedView);
            Assert.AreSame(returnedView, myView);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddingDuplicateNamedViewThrows()
        {
            IRegion region = new Region();

            region.Add(new object(), "MyView");
            region.Add(new object(), "MyView");
        }

        [TestMethod]
        public void AddNamedViewIsAlsoListedInViewsCollection()
        {
            IRegion region = new Region();
            object myView = new object();

            region.Add(myView, "MyView");

            Assert.AreEqual(1, region.Views.Cast<object>().Count());
            Assert.AreSame(myView, region.Views.Cast<object>().ElementAt(0));
        }

        [TestMethod]
        public void GetViewReturnsNullWhenViewDoesNotExistInRegion()
        {
            IRegion region = new Region();

            Assert.IsNull(region.GetView("InexistentView"));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetViewWithNullOrEmptyStringThrows()
        {
            IRegion region = new Region();

            region.GetView(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNamedViewWithNullOrEmptyStringNameThrows()
        {
            IRegion region = new Region();

            region.Add(new object(), string.Empty);
        }

        [TestMethod]
        public void GetViewReturnsNullAfterRemovingViewFromRegion()
        {
            IRegion region = new Region();
            object myView = new object();
            region.Add(myView, "MyView");
            region.Remove(myView);

            Assert.IsNull(region.GetView("MyView"));
        }

        [TestMethod]
        public void AddViewPassesSameScopeByDefaultToView()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new MockDependencyObject();

            region.Add(myView);

            Assert.AreSame(regionManager, myView.GetValue(RegionManager.RegionManagerProperty));
        }

        [TestMethod]
        public void AddViewPassesSameScopeByDefaultToNamedView()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new MockDependencyObject();

            region.Add(myView, "MyView");

            Assert.AreSame(regionManager, myView.GetValue(RegionManager.RegionManagerProperty));
        }

        [TestMethod]
        public void AddViewPassesDiferentScopeWhenAdding()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new MockDependencyObject();

            region.Add(myView, "MyView", true);

            Assert.AreNotSame(regionManager, myView.GetValue(RegionManager.RegionManagerProperty));
        }

        [TestMethod]
        public void CreatingNewScopesAsksTheRegionManagerForNewInstance()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new object();

            region.Add(myView, "MyView", true);

            Assert.IsTrue(regionManager.CreateRegionManagerCalled);
        }

        [TestMethod]
        public void AddViewReturnsExistingRegionManager()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new object();

            var returnedRegionManager = region.Add(myView, "MyView", false);

            Assert.AreSame(regionManager, returnedRegionManager);
        }

        [TestMethod]
        public void AddViewReturnsNewRegionManager()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new object();

            var returnedRegionManager = region.Add(myView, "MyView", true);

            Assert.AreNotSame(regionManager, returnedRegionManager);
        }

        [TestMethod]
        public void AddingNonDependencyObjectToRegionDoesNotThrow()
        {
            IRegion region = new Region();
            object model = new object();

            region.Add(model);

            Assert.AreEqual(1, region.Views.Cast<object>().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActivateNonAddedViewThrows()
        {
            IRegion region = new Region();

            object nonAddedView = new object();

            region.Activate(nonAddedView);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeactivateNonAddedViewThrows()
        {
            IRegion region = new Region();

            object nonAddedView = new object();

            region.Deactivate(nonAddedView);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ActivateNullViewThrows()
        {
            IRegion region = new Region();

            region.Activate(null);
        }

        [TestMethod]
        public void AddViewRaisesCollectionViewEvent()
        {
            bool viewAddedCalled = false;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Add)
                                                          viewAddedCalled = true;
                                                  };

            object model = new object();
            Assert.IsFalse(viewAddedCalled);
            region.Add(model);

            Assert.IsTrue(viewAddedCalled);
        }

        [TestMethod]
        public void ViewAddedEventPassesTheViewAddedInTheEventArgs()
        {
            object viewAdded = null;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Add)
                                                      {
                                                          viewAdded = e.NewItems[0];
                                                      }
                                                  };
            object model = new object();
            Assert.IsNull(viewAdded);
            region.Add(model);

            Assert.IsNotNull(viewAdded);
            Assert.AreSame(model, viewAdded);
        }

        [TestMethod]
        public void RemoveViewFiresViewRemovedEvent()
        {
            bool viewRemoved = false;

            IRegion region = new Region();
            object model = new object();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Remove)
                                                          viewRemoved = true;
                                                  };

            region.Add(model);
            Assert.IsFalse(viewRemoved);

            region.Remove(model);

            Assert.IsTrue(viewRemoved);
        }

        [TestMethod]
        public void ViewRemovedEventPassesTheViewRemovedInTheEventArgs()
        {
            object removedView = null;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Remove)
                                                          removedView = e.OldItems[0];
                                                  };
            object model = new object();
            region.Add(model);
            Assert.IsNull(removedView);

            region.Remove(model);

            Assert.AreSame(model, removedView);
        }

        [TestMethod]
        public void ShowViewFiresViewShowedEvent()
        {
            bool viewActivated = false;

            IRegion region = new Region();
            object model = new object();
            region.ActiveViews.CollectionChanged += (o, e) =>
                                                        {
                                                            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Contains(model))
                                                                viewActivated = true;
                                                        };
            region.Add(model);
            Assert.IsFalse(viewActivated);

            region.Activate(model);

            Assert.IsTrue(viewActivated);
        }

        [TestMethod]
        public void AddingSameViewTwiceThrows()
        {
            object view = new object();
            IRegion region = new Region();
            region.Add(view);

            try
            {
                region.Add(view);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("View already exists in region.", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemovingViewAlsoRemovesItFromActiveViews()
        {
            IRegion region = new Region();
            object model = new object();
            region.Add(model);
            region.Activate(model);
            Assert.IsTrue(region.ActiveViews.Contains(model));

            region.Remove(model);

            Assert.IsFalse(region.ActiveViews.Contains(model));
        }

        [TestMethod]
        public void ShouldGetNotificationWhenContextChanges()
        {
            IRegion region = new Region();
            bool contextChanged = false;
            region.PropertyChanged += (s, args) => { if (args.PropertyName == "Context") contextChanged = true; };

            region.Context = "MyNewContext";

            Assert.IsTrue(contextChanged);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangingNameOnceItIsSetThrows()
        {
            var region = new Region();
            region.Name = "MyRegion";

            region.Name = "ChangedRegionName";
        }

        private class MockRegionManager : IRegionManager
        {
            public bool CreateRegionManagerCalled;

            public IRegionManager CreateRegionManager()
            {
                CreateRegionManagerCalled = true;
                return new MockRegionManager();
            }

            public IRegionManager AddToRegion(string regionName, object view)
            {
                throw new NotImplementedException();
            }

            public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
            {
                throw new NotImplementedException();
            }

            public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
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

            public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
            {
                throw new NotImplementedException();
            }

            public IRegionCollection Regions
            {
                get { throw new NotImplementedException(); }
            }

            public IRegion AttachNewRegion(object regionTarget, string regionName)
            {
                throw new NotImplementedException();
            }

            public bool Navigate(Uri source)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void NavigateDelegatesToIRegionNavigationService()
        {
            try
            {
                // Prepare
                IRegion region = new Region();

                object view = new object();
                region.Add(view);

                Uri uri = new Uri(view.GetType().Name, UriKind.Relative);
                Action<NavigationResult> navigationCallback = nr => { };
                NavigationParameters navigationParameters = new NavigationParameters();

                Mock<IRegionNavigationService> mockRegionNavigationService = new Mock<IRegionNavigationService>();
                mockRegionNavigationService.Setup(x => x.RequestNavigate(uri, navigationCallback, navigationParameters)).Verifiable();

                Mock<IServiceLocator> mockServiceLocator = new Mock<IServiceLocator>();
                mockServiceLocator.Setup(x => x.GetInstance<IRegionNavigationService>()).Returns(mockRegionNavigationService.Object);
                ServiceLocator.SetLocatorProvider(() => mockServiceLocator.Object);

                // Act
                region.RequestNavigate(uri, navigationCallback, navigationParameters);

                // Verify
                mockRegionNavigationService.VerifyAll();
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(() => null);
            }
        }

        [TestMethod]
        public void WhenViewsWithSortHintsAdded_RegionSortsViews()
        {
            IRegion region = new Region();

            object view1 = new ViewOrder1();
            object view2 = new ViewOrder2();
            object view3 = new ViewOrder3();

            region.Add(view1);
            region.Add(view2);
            region.Add(view3);

            Assert.AreEqual(3, region.Views.Count());
            Assert.AreSame(view2, region.Views.ElementAt(0));
            Assert.AreSame(view3, region.Views.ElementAt(1));
            Assert.AreSame(view1, region.Views.ElementAt(2));
        }

        [TestMethod]
        public void WhenViewHasBeenRemovedAndRegionManagerPropertyCleared_ThenItCanBeAddedAgainToARegion()
        {
            IRegion region = new Region { RegionManager = new MockRegionManager() };

            var view = new MockFrameworkElement();

            var scopedRegionManager = region.Add(view, null, true);

            Assert.AreEqual(view, region.Views.First());

            region.Remove(view);

            view.ClearValue(RegionManager.RegionManagerProperty);

            Assert.AreEqual(0, region.Views.Cast<object>().Count());

            var newScopedRegion = region.Add(view, null, true);

            Assert.AreEqual(view, region.Views.First());

            Assert.AreSame(newScopedRegion, view.GetValue(RegionManager.RegionManagerProperty));
        }

        [ViewSortHint("C")]
        private class ViewOrder1 { };
        [ViewSortHint("A")]
        private class ViewOrder2 { };
        [ViewSortHint("B")]
        private class ViewOrder3 { };
    }
}
