using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Moq;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Regions;
using Prism.Regions.Navigation;
using Xamarin.Forms;
using Xunit;
using Region = Prism.Regions.Region;

namespace Prism.Forms.Regions.Tests
{
    public class RegionFixture
    {
        [Fact]
        public void WhenRegionConstructed_SortComparisonIsDefault()
        {
            IRegion region = new Region();

            Assert.NotNull(region.SortComparison);
            Assert.Equal(region.SortComparison, Region.DefaultSortComparison);
        }

        [Fact]
        public void CanAddContentToRegion()
        {
            IRegion region = new Region();

            Assert.Empty(region.Views);

            region.Add(new ContentView());

            Assert.Single(region.Views);
        }


        [Fact]
        public void CanRemoveContentFromRegion()
        {
            IRegion region = new Region();
            var view = new ContentView();

            region.Add(view);
            region.Remove(view);

            Assert.Empty(region.Views);
        }

        [Fact]
        public void RemoveInexistentViewThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();
                var view = new ContentView();

                region.Remove(view);

                Assert.Empty(region.Views);
            });

        }

        [Fact]
        public void RegionExposesCollectionOfContainedViews()
        {
            IRegion region = new Region();

            var view = new ContentView();

            region.Add(view);

            var views = region.Views;

            Assert.NotNull(views);
            Assert.Single(views);
            Assert.Same(view, views.ElementAt(0));
        }

        [Fact]
        public void CanAddAndRetrieveNamedViewInstance()
        {
            IRegion region = new Region();
            var myView = new ContentView();
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

                region.Add(new ContentView(), "MyView");
                region.Add(new ContentView(), "MyView");
            });

        }

        [Fact]
        public void AddNamedViewIsAlsoListedInViewsCollection()
        {
            IRegion region = new Region();
            var myView = new ContentView();

            region.Add(myView, "MyView");

            Assert.Single(region.Views);
            Assert.Same(myView, region.Views.ElementAt(0));
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
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                region.Add(new ContentView(), string.Empty);
            });

        }

        [Fact]
        public void GetViewReturnsNullAfterRemovingViewFromRegion()
        {
            IRegion region = new Region();
            var myView = new ContentView();
            region.Add(myView, "MyView");
            region.Remove(myView);

            Assert.Null(region.GetView("MyView"));
        }

        [Fact]
        public void AddViewPassesSameScopeByDefaultToView()
        {
            var regionManager = Mock.Of<IRegionManager>();
            IRegion region = new Region
            {
                RegionManager = regionManager
            };
            var myView = new ContentView();

            region.Add(myView);

            Assert.Same(regionManager, myView.GetValue(Prism.Regions.Xaml.RegionManager.RegionManagerProperty));
        }

        [Fact]
        public void AddViewPassesSameScopeByDefaultToNamedView()
        {
            var regionManager = Mock.Of<IRegionManager>();
            IRegion region = new Region
            {
                RegionManager = regionManager
            };
            var myView = new ContentView();

            region.Add(myView, "MyView");

            Assert.Same(regionManager, myView.GetValue(Prism.Regions.Xaml.RegionManager.RegionManagerProperty));
        }

        [Fact]
        public void AddViewPassesDiferentScopeWhenAdding()
        {
            var regionManager = Mock.Of<IRegionManager>();
            IRegion region = new Region
            {
                RegionManager = regionManager
            };
            var myView = new ContentView();

            region.Add(myView, "MyView", true);

            Assert.NotSame(regionManager, myView.GetValue(Prism.Regions.Xaml.RegionManager.RegionManagerProperty));
        }

        [Fact]
        public void CreatingNewScopesAsksTheRegionManagerForNewInstance()
        {
            var regionManagerMock = new Mock<IRegionManager>();
            var createRegionManagerCalled = false;
            regionManagerMock.Setup(x => x.CreateRegionManager())
                .Callback(() => createRegionManagerCalled = true)
                .Returns(Mock.Of<IRegionManager>());

            IRegion region = new Region
            {
                RegionManager = regionManagerMock.Object
            };
            var myView = new ContentView();

            region.Add(myView, "MyView", true);

            Assert.True(createRegionManagerCalled);
        }

        [Fact]
        public void AddViewReturnsExistingRegionManager()
        {
            var regionManager = Mock.Of<IRegionManager>();
            IRegion region = new Region
            {
                RegionManager = regionManager
            };
            var myView = new ContentView();

            var returnedRegionManager = region.Add(myView, "MyView", false);

            Assert.Same(regionManager, returnedRegionManager);
        }

        [Fact]
        public void AddViewReturnsNewRegionManager()
        {
            var regionManager = Mock.Of<IRegionManager>();
            IRegion region = new Region
            {
                RegionManager = regionManager
            };
            var myView = new ContentView();

            var returnedRegionManager = region.Add(myView, "MyView", true);

            Assert.NotSame(regionManager, returnedRegionManager);
        }

        [Fact]
        public void AddingNonDependencyObjectToRegionDoesNotThrow()
        {
            IRegion region = new Region();
            var model = new ContentView();

            region.Add(model);

            Assert.Single(region.Views);
        }

        [Fact]
        public void ActivateNonAddedViewThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                var nonAddedView = new ContentView();

                region.Activate(nonAddedView);
            });

        }

        [Fact]
        public void DeactivateNonAddedViewThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                IRegion region = new Region();

                var nonAddedView = new ContentView();

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
            bool viewAddedCalled = false;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    viewAddedCalled = true;
            };

            var model = new ContentView();
            Assert.False(viewAddedCalled);
            region.Add(model);

            Assert.True(viewAddedCalled);
        }

        [Fact]
        public void ViewAddedEventPassesTheViewAddedInTheEventArgs()
        {
            VisualElement viewAdded = null;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    viewAdded = (VisualElement)e.NewItems[0];
                }
            };
            var model = new ContentView();
            Assert.Null(viewAdded);
            region.Add(model);

            Assert.NotNull(viewAdded);
            Assert.Same(model, viewAdded);
        }

        [Fact]
        public void RemoveViewFiresViewRemovedEvent()
        {
            bool viewRemoved = false;

            IRegion region = new Region();
            var model = new ContentView();
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
            VisualElement removedView = null;

            IRegion region = new Region();
            region.Views.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                    removedView = (VisualElement)e.OldItems[0];
            };
            var model = new ContentView();
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
            var model = new ContentView();
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
            var view = new ContentView();
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
            IRegion region = new Region();
            var model = new ContentView();
            region.Add(model);
            region.Activate(model);
            Assert.True(region.ActiveViews.Contains(model));

            region.Remove(model);

            Assert.False(region.ActiveViews.Contains(model));
        }

        [Fact]
        public void ShouldGetNotificationWhenContextChanges()
        {
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

        [Fact]
        public void NavigateDelegatesToIRegionNavigationService()
        {
            try
            {
                // Prepare
                IRegion region = new Region();

                var view = new ContentView();
                region.Add(view);

                var uri = new Uri(view.GetType().Name, UriKind.Relative);
                Action<IRegionNavigationResult> navigationCallback = nr => { };
                var navigationParameters = new NavigationParameters();

                var mockRegionNavigationService = new Mock<IRegionNavigationService>();
                mockRegionNavigationService.Setup(x => x.RequestNavigate(uri, navigationCallback, navigationParameters)).Verifiable();

                var containerMock = new Mock<IContainerExtension>();
                containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationService))).Returns(mockRegionNavigationService.Object);
                ContainerLocator.ResetContainer();
                ContainerLocator.SetContainerExtension(() => containerMock.Object);

                // Act
                region.NavigationService.RequestNavigate(uri, navigationCallback, navigationParameters);

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
            IRegion region = new Region();

            var view1 = new ViewOrder1();
            var view2 = new ViewOrder2();
            var view3 = new ViewOrder3();

            region.Add(view1);
            region.Add(view2);
            region.Add(view3);

            Assert.Equal(3, region.Views.Count());
            Assert.Same(view2, region.Views.ElementAt(0));
            Assert.Same(view3, region.Views.ElementAt(1));
            Assert.Same(view1, region.Views.ElementAt(2));
        }

        [Fact]
        public void WhenViewHasBeenRemovedAndRegionManagerPropertyCleared_ThenItCanBeAddedAgainToARegion()
        {
            var regionManagerMock = new Mock<IRegionManager>();
            regionManagerMock.Setup(x => x.CreateRegionManager())
                .Returns(Mock.Of<IRegionManager>());
            IRegion region = new Region { RegionManager = regionManagerMock.Object };

            var view = new ContentView();

            var scopedRegionManager = region.Add(view, null, true);

            Assert.Equal(view, region.Views.First());

            region.Remove(view);

            view.ClearValue(Prism.Regions.Xaml.RegionManager.RegionManagerProperty);

            Assert.Empty(region.Views);

            var newScopedRegion = region.Add(view, null, true);

            Assert.Equal(view, region.Views.First());

            Assert.Same(newScopedRegion, view.GetValue(Prism.Regions.Xaml.RegionManager.RegionManagerProperty));
        }

        [ViewSortHint("C")]
        private class ViewOrder1 : View { };
        [ViewSortHint("A")]
        private class ViewOrder2 : View { };
        [ViewSortHint("B")]
        private class ViewOrder3 : View { };
    }
}
