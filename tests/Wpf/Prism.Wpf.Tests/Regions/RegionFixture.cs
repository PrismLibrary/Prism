using System;
using System.Collections.Specialized;
using System.Linq;
using Moq;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Wpf.Tests.Mocks;
using Xunit;

namespace Prism.Wpf.Tests.Regions
{
    public class RegionFixture
    {
        [Fact]
        public void WhenRegionConstructed_SortComparisonIsDefault()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();

            Assert.NotNull(region.SortComparison);
            Assert.Equal(region.SortComparison, Region.DefaultSortComparison);
        }

        [Fact]
        public void CanAddContentToRegion()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();

            Assert.Empty(region.Views.Cast<object>());

            region.Add(new object());

            Assert.Single(region.Views.Cast<object>());
        }


        [Fact]
        public void CanRemoveContentFromRegion()
        {
            IRegion region = new Region();
            object view = new object();

            region.Add(view);
            region.Remove(view);

            Assert.Empty(region.Views.Cast<object>());
        }

        [Fact]
        public void RemoveInexistentViewThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();
                object view = new object();

                region.Remove(view);

                Assert.Empty(region.Views.Cast<object>());
            });

        }

        [Fact]
        public void RegionExposesCollectionOfContainedViews()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();

            object view = new object();

            region.Add(view);

            var views = region.Views;

            Assert.NotNull(views);
            Assert.Single(views.Cast<object>());
            Assert.Same(view, views.Cast<object>().ElementAt(0));
        }

        [Fact]
        public void CanAddAndRetrieveNamedViewInstance()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();
            object myView = new object();
            region.Add(myView, "MyView");
            object returnedView = region.GetView("MyView");

            Assert.NotNull(returnedView);
            Assert.Same(returnedView, myView);
        }

        [Fact]
        public void AddingDuplicateNamedViewThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                IRegion region = new Region();

                region.Add(new object(), "MyView");
                region.Add(new object(), "MyView");
            });

        }

        [Fact]
        public void AddNamedViewIsAlsoListedInViewsCollection()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();
            object myView = new object();

            region.Add(myView, "MyView");

            Assert.Single(region.Views.Cast<object>());
            Assert.Same(myView, region.Views.Cast<object>().ElementAt(0));
        }

        [Fact]
        public void GetViewReturnsNullWhenViewDoesNotExistInRegion()
        {
            IRegion region = new Region();

            Assert.Null(region.GetView("InexistentView"));
        }


        [Fact]
        public void GetViewWithNullOrEmptyStringThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                region.GetView(string.Empty);
            });

        }

        [Fact]
        public void AddNamedViewWithNullOrEmptyStringNameThrows()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                region.Add(new object(), string.Empty);
            });

        }

        [Fact]
        public void GetViewReturnsNullAfterRemovingViewFromRegion()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();
            object myView = new object();
            region.Add(myView, "MyView");
            region.Remove(myView);

            Assert.Null(region.GetView("MyView"));
        }

        [Fact]
        public void AddViewPassesSameScopeByDefaultToView()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new MockDependencyObject();

            region.Add(myView);

            Assert.Same(regionManager, myView.GetValue(RegionManager.RegionManagerProperty));
        }

        [Fact]
        public void AddViewPassesSameScopeByDefaultToNamedView()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new MockDependencyObject();

            region.Add(myView, "MyView");

            Assert.Same(regionManager, myView.GetValue(RegionManager.RegionManagerProperty));
        }

        [Fact]
        public void AddViewPassesDifferentScopeWhenAdding()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new MockDependencyObject();

            region.Add(myView, "MyView", true);

            Assert.NotSame(regionManager, myView.GetValue(RegionManager.RegionManagerProperty));
        }

        [Fact]
        public void CreatingNewScopesAsksTheRegionManagerForNewInstance()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new object();

            region.Add(myView, "MyView", true);

            Assert.True(regionManager.CreateRegionManagerCalled);
        }

        [Fact]
        public void AddViewReturnsExistingRegionManager()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new object();

            var returnedRegionManager = region.Add(myView, "MyView", false);

            Assert.Same(regionManager, returnedRegionManager);
        }

        [Fact]
        public void AddViewReturnsNewRegionManager()
        {
            var regionManager = new MockRegionManager();
            IRegion region = new Region();
            region.RegionManager = regionManager;
            var myView = new object();

            var returnedRegionManager = region.Add(myView, "MyView", true);

            Assert.NotSame(regionManager, returnedRegionManager);
        }

        [Fact]
        public void AddingNonDependencyObjectToRegionDoesNotThrow()
        {
            IRegion region = new Region();
            object model = new object();

            region.Add(model);

            Assert.Single(region.Views.Cast<object>());
        }

        [Fact]
        public void ActivateNonAddedViewThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                object nonAddedView = new object();

                region.Activate(nonAddedView);
            });

        }

        [Fact]
        public void DeactivateNonAddedViewThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                object nonAddedView = new object();

                region.Deactivate(nonAddedView);
            });

        }

        [Fact]
        public void ActivateNullViewThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                IRegion region = new Region();

                region.Activate(null);
            });

        }

        [Fact]
        public void AddViewRaisesCollectionViewEvent()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            bool viewAddedCalled = false;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Add)
                                                          viewAddedCalled = true;
                                                  };

            object model = new object();
            Assert.False(viewAddedCalled);
            region.Add(model);

            Assert.True(viewAddedCalled);
        }

        [Fact]
        public void ViewAddedEventPassesTheViewAddedInTheEventArgs()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
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
            Assert.Null(viewAdded);
            region.Add(model);

            Assert.NotNull(viewAdded);
            Assert.Same(model, viewAdded);
        }

        [Fact]
        public void RemoveViewFiresViewRemovedEvent()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            bool viewRemoved = false;

            IRegion region = new Region();
            object model = new object();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Remove)
                                                          viewRemoved = true;
                                                  };

            region.Add(model);
            Assert.False(viewRemoved);

            region.Remove(model);

            Assert.True(viewRemoved);
        }

        [Fact]
        public void ViewRemovedEventPassesTheViewRemovedInTheEventArgs()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            object removedView = null;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
                                                  {
                                                      if (e.Action == NotifyCollectionChangedAction.Remove)
                                                          removedView = e.OldItems[0];
                                                  };
            object model = new object();
            region.Add(model);
            Assert.Null(removedView);

            region.Remove(model);

            Assert.Same(model, removedView);
        }

        [Fact]
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
            Assert.False(viewActivated);

            region.Activate(model);

            Assert.True(viewActivated);
        }

        [Fact]
        public void AddingSameViewTwiceThrows()
        {
            object view = new object();
            IRegion region = new Region();
            region.Add(view);

            try
            {
                region.Add(view);
                //Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.Equal("View already exists in region.", ex.Message);
            }
            catch
            {
                //Assert.Fail();
            }
        }

        [Fact]
        public void RemovingViewAlsoRemovesItFromActiveViews()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();
            object model = new object();
            region.Add(model);
            region.Activate(model);
            Assert.True(region.ActiveViews.Contains(model));

            region.Remove(model);

            Assert.False(region.ActiveViews.Contains(model));
        }

        [Fact]
        public void ShouldGetNotificationWhenContextChanges()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();
            bool contextChanged = false;
            region.PropertyChanged += (s, args) => { if (args.PropertyName == "Context") contextChanged = true; };

            region.Context = "MyNewContext";

            Assert.True(contextChanged);
        }

        [Fact]
        public void ChangingNameOnceItIsSetThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var region = new Region
                {
                    Name = "MyRegion"
                };

                region.Name = "ChangedRegionName";
            });

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

            public IRegionManager AddToRegion(string regionName, string viewName)
            {
                throw new NotImplementedException();
            }

            public IRegionManager RegisterViewWithRegion(string regionName, string viewName)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
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

                var containerMock = new Mock<IContainerExtension>();
                containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationService))).Returns(mockRegionNavigationService.Object);
                ContainerLocator.ResetContainer();
                ContainerLocator.SetContainerExtension(containerMock.Object);

                // Act
                region.RequestNavigate(uri, navigationCallback, navigationParameters);

                // Verify
                mockRegionNavigationService.VerifyAll();
            }
            finally
            {
                ContainerLocator.ResetContainer();
            }
        }

        [Fact]
        public void WhenViewsWithSortHintsAdded_RegionSortsViews()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region();

            object view1 = new ViewOrder1();
            object view2 = new ViewOrder2();
            object view3 = new ViewOrder3();

            region.Add(view1);
            region.Add(view2);
            region.Add(view3);

            Assert.Equal(3, region.Views.Count());
            Assert.Same(view2, region.Views.ElementAt(0));
            Assert.Same(view3, region.Views.ElementAt(1));
            Assert.Same(view1, region.Views.ElementAt(2));
        }

        [StaFact]
        public void WhenViewHasBeenRemovedAndRegionManagerPropertyCleared_ThenItCanBeAddedAgainToARegion()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            IRegion region = new Region { RegionManager = new MockRegionManager() };

            var view = new MockFrameworkElement();

            var scopedRegionManager = region.Add(view, null, true);

            Assert.Equal(view, region.Views.First());

            region.Remove(view);

            view.ClearValue(RegionManager.RegionManagerProperty);

            Assert.Empty(region.Views.Cast<object>());

            var newScopedRegion = region.Add(view, null, true);

            Assert.Equal(view, region.Views.First());

            Assert.Same(newScopedRegion, view.GetValue(RegionManager.RegionManagerProperty));
        }

        [ViewSortHint("C")]
        private class ViewOrder1 { };
        [ViewSortHint("A")]
        private class ViewOrder2 { };
        [ViewSortHint("B")]
        private class ViewOrder3 { };
    }
}
