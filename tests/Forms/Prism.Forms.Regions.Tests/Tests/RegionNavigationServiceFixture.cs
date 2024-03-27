using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Navigation;
using Xamarin.Forms;
using Xunit;
using Region = Prism.Navigation.Regions.Region;

namespace Prism.Forms.Regions.Tests
{
    public class RegionNavigationServiceFixture
    {
        [Fact]
        public void WhenNavigating_ViewIsActivated()
        {
            // Prepare
            var view = new ContentView();
            var viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            var regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Success == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.True(isViewActive);
        }

        [Fact]
        public void WhenNavigatingWithQueryString_ViewIsActivated()
        {
            // Prepare
            var view = new ContentView();
            var viewUri = new Uri(view.GetType().Name + "?MyQuery=true", UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            var regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Success == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.True(isViewActive);
        }

        [Fact]
        public void WhenNavigatingAndViewCannotBeAcquired_ThenNavigationResultHasError()
        {
            // Prepare
            var view = new ContentView();
            var viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string otherType = "OtherType";

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());
            IContainerExtension container = containerMock.Object;

            var targetHandlerMock = new Mock<IRegionNavigationContentLoader>();
            targetHandlerMock.Setup(th => th.LoadContent(It.IsAny<IRegion>(), It.IsAny<NavigationContext>())).Throws<ArgumentException>();

            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, targetHandlerMock.Object, journal)
            {
                Region = region
            };

            // Act
            Exception error = null;
            target.RequestNavigate(
                new Uri(otherType.GetType().Name, UriKind.Relative),
                nr =>
                {
                    error = nr.Exception;
                });

            // Verify
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.False(isViewActive);
            Assert.IsType<ArgumentException>(error);
        }

        [Fact]
        public void WhenNavigatingWithNullUri_Throws()
        {
            // Prepare
            IRegion region = new Region();

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            NavigationResult navigationResult = null;
            target.RequestNavigate((Uri)null, nr => navigationResult = nr);

            // Verify
            Assert.False(navigationResult.Success);
            Assert.NotNull(navigationResult.Exception);
            Assert.IsType<ArgumentNullException>(navigationResult.Exception);
        }

        [Fact]
        public void WhenNavigatingAndViewImplementsIRegionAware_ThenNavigatedIsInvokedOnNavigation()
        {
            // Prepare
            var region = new Region();

            var viewMock = new Mock<View>();
            viewMock
                .As<IRegionAware>()
                .Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(true);
            var view = viewMock.Object;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewMock
                .As<IRegionAware>()
                .Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri && nc.NavigationService == target)));
        }

        [Fact]
        public void WhenNavigatingAndBindingContextImplementsIRegionAware_ThenNavigatedIsInvokesOnNavigation()
        {
            // Prepare
            var region = new Region();

            var mockView = new Mock<View>();
            var mockIRegionAwareBindingContext = new Mock<IRegionAware>();
            mockIRegionAwareBindingContext
                .Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(true);
            mockView.Object.BindingContext = mockIRegionAwareBindingContext.Object;

            var view = mockView.Object;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            mockIRegionAwareBindingContext.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri)));
        }

        [Fact]
        public void WhenNavigatingAndBothViewAndBindingContextImplementIRegionAware_ThenNavigatedIsInvokesOnNavigation()
        {
            // Prepare
            var region = new Region();

            var mockView = new Mock<View>();
            var mockIRegionAwareView = mockView.As<IRegionAware>();
            mockIRegionAwareView
                .Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(true);

            var mockIRegionAwareBindingContext = new Mock<IRegionAware>();
            mockIRegionAwareBindingContext.Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>())).Returns(true);
            mockView.Object.BindingContext = mockIRegionAwareBindingContext.Object;

            var view = mockView.Object;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            mockIRegionAwareView.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri)));
            mockIRegionAwareBindingContext.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri)));
        }

        [Fact]
        public void WhenNavigating_NavigationIsRecordedInJournal()
        {
            // Prepare
            var view = new ContentView();
            var viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            var regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            IRegionNavigationJournalEntry journalEntry = new RegionNavigationJournalEntry();

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(journalEntry);

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);

            var journalMock = new Mock<IRegionNavigationJournal>();
            journalMock.Setup(x => x.RecordNavigation(journalEntry, true)).Verifiable();

            IRegionNavigationJournal journal = journalMock.Object;


            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(viewUri, nr => { });

            // Verify
            Assert.NotNull(journalEntry);
            Assert.Equal(viewUri, journalEntry.Uri);
            journalMock.VerifyAll();
        }

        [Fact]
        public void WhenNavigatingAndCurrentlyActiveViewImplementsINavigateWithVeto_ThenNavigationRequestQueriesForVeto()
        {
            // Prepare
            var region = new Region();

            var viewMock = new Mock<View>();
            viewMock
                .As<IConfirmNavigationRequest>()
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Verifiable();

            var view = viewMock.Object;
            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewMock.VerifyAll();
        }

        [Fact]
        public void WhenNavigating_ThenNavigationRequestQueriesForVetoOnAllActiveViewsIfAllSucceed()
        {
            // Prepare
            var region = new Region();

            var view1Mock = new Mock<View>();
            view1Mock
                .As<IConfirmNavigationRequest>()
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view1 = view1Mock.Object;
            region.Add(view1);
            region.Activate(view1);

            var view2Mock = new Mock<StackLayout>();
            view2Mock.As<IConfirmNavigationRequest>();

            var view2 = view2Mock.Object;
            region.Add(view2);

            var view3Mock = new Mock<Grid>();
            view3Mock.As<IRegionAware>();

            var view3 = view3Mock.Object;
            region.Add(view3);
            region.Activate(view3);

            var view4Mock = new Mock<Frame>();
            view4Mock
                .As<IConfirmNavigationRequest>()
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view4 = view4Mock.Object;
            region.Add(view4);
            region.Activate(view4);

            var navigationUri = new Uri(view1.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            view1Mock.VerifyAll();
            view2Mock
                .As<IConfirmNavigationRequest>()
                .Verify(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()), Times.Never());
            view3Mock.VerifyAll();
            view4Mock.VerifyAll();
        }

        [Fact]
        public void WhenRequestNavigateAwayAcceptsThroughCallback_ThenNavigationProceeds()
        {
            // Prepare
            var region = new Region();

            var view1Mock = new Mock<View>();
            view1Mock
                .As<IConfirmNavigationRequest>()
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view1 = view1Mock.Object;

            var view2 = new StackLayout();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationSucceeded = false;
            target.RequestNavigate(navigationUri, nr => { navigationSucceeded = nr.Success == true; });

            // Verify
            view1Mock.VerifyAll();
            Assert.True(navigationSucceeded);
            Assert.Equal(new object[] { view1, view2 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenRequestNavigateAwayRejectsThroughCallback_ThenNavigationDoesNotProceed()
        {
            // Prepare
            var region = new Region();

            var view1Mock = new Mock<View>();
            view1Mock
                .As<IConfirmNavigationRequest>()
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1 = view1Mock.Object;

            var view2 = new StackLayout();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationFailed = false;
            target.RequestNavigate(navigationUri, nr => { navigationFailed = nr.Success == false; });

            // Verify
            view1Mock.VerifyAll();
            Assert.True(navigationFailed);
            Assert.Equal(new object[] { view1 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenNavigatingAndBindingContextOnCurrentlyActiveViewImplementsINavigateWithVeto_ThenNavigationRequestQueriesForVeto()
        {
            // Prepare
            var region = new Region();

            var viewModelMock = new Mock<IConfirmNavigationRequest>();
            viewModelMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Verifiable();

            var viewMock = new Mock<View>();

            var view = viewMock.Object;
            view.BindingContext = viewModelMock.Object;

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewModelMock.VerifyAll();
        }

        [Fact]
        public void WhenRequestNavigateAwayOnBindingContextAcceptsThroughCallback_ThenNavigationProceeds()
        {
            // Prepare
            var region = new Region();

            var view1BindingContextMock = new Mock<IConfirmNavigationRequest>();
            view1BindingContextMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view1Mock = new Mock<View>();
            var view1 = view1Mock.Object;
            view1.BindingContext = view1BindingContextMock.Object;

            var view2 = new StackLayout();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationSucceeded = false;
            target.RequestNavigate(navigationUri, nr => { navigationSucceeded = nr.Success == true; });

            // Verify
            view1BindingContextMock.VerifyAll();
            Assert.True(navigationSucceeded);
            Assert.Equal(new object[] { view1, view2 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenRequestNavigateAwayOnBindingContextRejectsThroughCallback_ThenNavigationDoesNotProceed()
        {
            // Prepare
            var region = new Region();

            var view1BindingContextMock = new Mock<IConfirmNavigationRequest>();
            view1BindingContextMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1Mock = new Mock<View>();
            var view1 = view1Mock.Object;
            view1.BindingContext = view1BindingContextMock.Object;

            var view2 = new StackLayout();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationFailed = false;
            target.RequestNavigate(navigationUri, nr => { navigationFailed = nr.Success == false; });

            // Verify
            view1BindingContextMock.VerifyAll();
            Assert.True(navigationFailed);
            Assert.Equal(new object[] { view1 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenViewAcceptsNavigationOutAfterNewIncomingRequestIsReceived_ThenOriginalRequestIsIgnored()
        {
            var region = new Region();

            var viewMock = new Mock<View>();

            var confirmationRequests = new List<Action<bool>>();

            viewMock
                .As<IConfirmNavigationRequest>()
                .Setup(icnr => icnr.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => { confirmationRequests.Add(c); });

            var view = viewMock.Object;
            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri("", UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock
                .Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry)))
                .Returns(new RegionNavigationJournalEntry());

            var contentLoaderMock = new Mock<IRegionNavigationContentLoader>();
            contentLoaderMock
                .Setup(cl => cl.LoadContent(region, It.IsAny<NavigationContext>()))
                .Returns(view);

            var container = containerMock.Object;
            var contentLoader = contentLoaderMock.Object;
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool firstNavigation = false;
            bool secondNavigation = false;
            target.RequestNavigate(navigationUri, nr => firstNavigation = nr.Success);
            target.RequestNavigate(navigationUri, nr => secondNavigation = nr.Success);

            Assert.Equal(2, confirmationRequests.Count);

            confirmationRequests[0](true);
            confirmationRequests[1](true);

            Assert.False(firstNavigation);
            Assert.True(secondNavigation);
        }

        [Fact]
        public void WhenViewModelAcceptsNavigationOutAfterNewIncomingRequestIsReceived_ThenOriginalRequestIsIgnored()
        {
            var region = new Region();

            var viewModelMock = new Mock<IConfirmNavigationRequest>();

            var viewMock = new Mock<View>();
            var view = viewMock.Object;
            view.BindingContext = viewModelMock.Object;

            var confirmationRequests = new List<Action<bool>>();

            viewModelMock
                .Setup(icnr => icnr.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => { confirmationRequests.Add(c); });

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri("", UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock
                .Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry)))
                .Returns(new RegionNavigationJournalEntry());

            var contentLoaderMock = new Mock<IRegionNavigationContentLoader>();
            contentLoaderMock
                .Setup(cl => cl.LoadContent(region, It.IsAny<NavigationContext>()))
                .Returns(view);

            var container = containerMock.Object;
            var contentLoader = contentLoaderMock.Object;
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            NavigationResult firstNavigation = null;
            NavigationResult secondNavigation = null;
            target.RequestNavigate(navigationUri, nr => firstNavigation = nr);
            target.RequestNavigate(navigationUri, nr => secondNavigation = nr);

            Assert.Equal(2, confirmationRequests.Count);

            confirmationRequests[0](true);
            confirmationRequests[1](true);

            Assert.False(firstNavigation.Success);
            Assert.True(secondNavigation.Success);
        }

        [Fact]
        public void BeforeNavigating_NavigatingEventIsRaised()
        {
            // Prepare
            var view = new ContentView();
            var viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            var regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool isNavigatingRaised = false;
            target.Navigating += delegate (object sender, RegionNavigationEventArgs e)
            {
                if (sender == target)
                {
                    isNavigatingRaised = true;
                }
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Success == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            Assert.True(isNavigatingRaised);
        }

        [Fact]
        public void WhenNavigationSucceeds_NavigatedIsRaised()
        {
            // Prepare
            var view = new ContentView();
            var viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            var regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            var contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool isNavigatedRaised = false;
            target.Navigated += delegate (object sender, RegionNavigationEventArgs e)
            {
                if (sender == target)
                {
                    isNavigatedRaised = true;
                }
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Success == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            Assert.True(isNavigatedRaised);
        }

        [Fact]
        public void WhenTargetViewCreationThrowsWithAsyncConfirmation_ThenExceptionIsProvidedToNavigationCallback()
        {
            var containerMock = new Mock<IContainerExtension>();

            var targetException = new Exception();
            var targetHandlerMock = new Mock<IRegionNavigationContentLoader>();
            targetHandlerMock
                .Setup(th => th.LoadContent(It.IsAny<IRegion>(), It.IsAny<NavigationContext>()))
                .Throws(targetException);

            var journalMock = new Mock<IRegionNavigationJournal>();

            Action<bool> navigationCallback = null;
            var viewMock = new Mock<View>();
            viewMock
                .As<IConfirmNavigationRequest>()
                .Setup(v => v.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => { navigationCallback = c; });

            var region = new Region();
            region.Add(viewMock.Object);
            region.Activate(viewMock.Object);

            var target = new RegionNavigationService(containerMock.Object, targetHandlerMock.Object, journalMock.Object)
            {
                Region = region
            };

            NavigationResult result = null;
            target.RequestNavigate(new Uri("", UriKind.Relative), nr => result = nr);
            navigationCallback(true);

            Assert.NotNull(result);
            Assert.Same(targetException, result.Exception);
        }

        [Fact]
        public void WhenNavigatingFromViewThatIsNavigationAware_ThenNotifiesActiveViewNavigatingFrom()
        {
            // Arrange
            var region = new Region();
            var viewMock = new Mock<View>();
            viewMock
                .As<IRegionAware>();
            var view = viewMock.Object;
            region.Add(view);

            var view2 = new StackLayout();
            region.Add(view2);

            region.Activate(view);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);
            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewMock.As<IRegionAware>()
                .Verify(v => v.OnNavigatedFrom(It.Is<NavigationContext>(ctx => ctx.Uri == navigationUri && ctx.Parameters.Count() == 0)));
        }

        [Fact]
        public void WhenNavigationFromViewThatIsNavigationAware_OnlyNotifiesOnNavigateFromForActiveViews()
        {
            // Arrange

            bool navigationFromInvoked = false;

            var region = new Region();

            var viewMock = new Mock<View>();
            viewMock
                .As<IRegionAware>()
                .Setup(x => x.OnNavigatedFrom(It.IsAny<NavigationContext>())).Callback(() => navigationFromInvoked = true);
            var view = viewMock.Object;
            region.Add(view);

            var targetViewMock = new Mock<StackLayout>();
            targetViewMock.As<IRegionAware>();
            region.Add(targetViewMock.Object);

            var activeViewMock = new Mock<Grid>();
            activeViewMock.As<IRegionAware>();
            region.Add(activeViewMock.Object);

            region.Activate(activeViewMock.Object);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(targetViewMock.Object.GetType().Name, UriKind.Relative);
            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            Assert.False(navigationFromInvoked);
        }

        [Fact]
        public void WhenNavigatingFromActiveViewWithNavigationAwareDataContext_NotifiesContextOfNavigatingFrom()
        {
            // Arrange
            var region = new Region();

            var mockBindingContext = new Mock<IRegionAware>();

            var view1Mock = new Mock<View>();
            var view1 = view1Mock.Object;
            view1.BindingContext = mockBindingContext.Object;

            region.Add(view1);

            var view2 = new StackLayout();
            region.Add(view2);

            region.Activate(view1);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);
            IContainerExtension container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            mockBindingContext.Verify(v => v.OnNavigatedFrom(It.Is<NavigationContext>(ctx => ctx.Uri == navigationUri && ctx.Parameters.Count() == 0)));
        }

        [Fact]
        public void WhenNavigatingWithNullCallback_ThenThrows()
        {
            var region = new Region();

            var navigationUri = new Uri("/", UriKind.Relative);
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());
            var container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            Exception ex = null;
            target.RequestNavigate(navigationUri, x => ex = x.Exception);

            Assert.NotNull(ex);
            Assert.IsType<RegionViewException>(ex);
        }

        [Fact]
        public void WhenNavigatingWithNoRegionSet_ThenMarshallExceptionToCallback()
        {
            var navigationUri = new Uri("/", UriKind.Relative);
            IContainerExtension container = new Mock<IContainerExtension>().Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal);

            Exception error = null;
            target.RequestNavigate(navigationUri, nr => error = nr.Exception);

            Assert.NotNull(error);
            Assert.IsType<InvalidOperationException>(error);
        }

        [Fact]
        public void WhenNavigatingWithNullUri_ThenMarshallExceptionToCallback()
        {

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());
            var container = containerMock.Object;
            var contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = new Region()
            };

            Exception error = null;
            target.RequestNavigate("", nr => error = nr.Exception);

            Assert.NotNull(error);
            Assert.IsType<RegionViewException>(error);
        }


        [Fact]
        public void WhenNavigationFailsBecauseTheContentViewCannotBeRetrieved_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var contentLoaderMock = new Mock<IRegionNavigationContentLoader>();
            contentLoaderMock
                .Setup(cl => cl.LoadContent(region, It.IsAny<NavigationContext>()))
                .Throws<InvalidOperationException>();

            var container = containerMock.Object;
            var contentLoader = contentLoaderMock.Object;
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate (object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(new Uri("invalid", UriKind.Relative), nr => isNavigationSuccessful = nr.Success);

            // Verify
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.NotNull(eventArgs.Error);
        }

        [Fact]
        public void WhenNavigationFailsBecauseActiveViewRejectsIt_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var view1Mock = new Mock<View>();
            view1Mock
                .As<IConfirmNavigationRequest>()
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1 = view1Mock.Object;

            var view2 = new StackLayout();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var contentLoaderMock = new Mock<IRegionNavigationContentLoader>();
            contentLoaderMock
                .Setup(cl => cl.LoadContent(region, It.IsAny<NavigationContext>()))
                .Returns(view2);

            var container = containerMock.Object;
            var contentLoader = contentLoaderMock.Object;
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate (object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(navigationUri, nr => isNavigationSuccessful = nr.Success);

            // Verify
            view1Mock.VerifyAll();
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.Null(eventArgs.Error);
        }

        [Fact]
        public void WhenNavigationFailsBecauseBindingContextForActiveViewRejectsIt_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var viewModel1Mock = new Mock<IConfirmNavigationRequest>();
            viewModel1Mock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1Mock = new Mock<View>();
            var view1 = view1Mock.Object;
            view1.BindingContext = viewModel1Mock.Object;

            var view2 = new StackLayout();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var contentLoaderMock = new Mock<IRegionNavigationContentLoader>();
            contentLoaderMock
                .Setup(cl => cl.LoadContent(region, It.IsAny<NavigationContext>()))
                .Returns(view2);

            var container = containerMock.Object;
            var contentLoader = contentLoaderMock.Object;
            var journal = Mock.Of<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate (object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(navigationUri, nr => isNavigationSuccessful = nr.Success);

            // Verify
            viewModel1Mock.VerifyAll();
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.Null(eventArgs.Error);
        }
    }
}
