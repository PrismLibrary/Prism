using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Moq;
using Prism.Ioc;
using Prism.Regions;
using Xunit;

namespace Prism.Wpf.Tests.Regions
{

    public class RegionNavigationServiceFixture
    {
        [Fact]
        public void WhenNavigating_ViewIsActivated()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.True(isViewActive);
        }

        [Fact]
        public void WhenNavigatingWithQueryString_ViewIsActivated()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name + "?MyQuery=true", UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.True(isViewActive);
        }

        [Fact]
        public void WhenNavigatingAndViewCannotBeAcquired_ThenNavigationResultHasError()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string otherType = "OtherType";

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());
            IContainerExtension container = containerMock.Object;

            Mock<IRegionNavigationContentLoader> targetHandlerMock = new Mock<IRegionNavigationContentLoader>();
            targetHandlerMock.Setup(th => th.LoadContent(It.IsAny<IRegion>(), It.IsAny<NavigationContext>())).Throws<ArgumentException>();

            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, targetHandlerMock.Object, journal)
            {
                Region = region
            };

            // Act
            Exception error = null;
            target.RequestNavigate(
                new Uri(otherType.GetType().Name, UriKind.Relative),
                nr =>
                {
                    error = nr.Error;
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
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            NavigationResult navigationResult = null;
            target.RequestNavigate((Uri)null, nr => navigationResult = nr);

            // Verify
            Assert.False(navigationResult.Result.Value);
            Assert.NotNull(navigationResult.Error);
            Assert.IsType<ArgumentNullException>(navigationResult.Error);
        }

        [Fact]
        public void WhenNavigatingAndViewImplementsINavigationAware_ThenNavigatedIsInvokedOnNavigation()
        {
            // Prepare
            var region = new Region();

            var viewMock = new Mock<INavigationAware>();
            viewMock.Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>())).Returns(true);
            var view = viewMock.Object;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewMock.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri && nc.NavigationService == target)));
        }

        [StaFact]
        public void WhenNavigatingAndDataContextImplementsINavigationAware_ThenNavigatedIsInvokesOnNavigation()
        {
            // Prepare
            var region = new Region();

            Mock<FrameworkElement> mockFrameworkElement = new Mock<FrameworkElement>();
            Mock<INavigationAware> mockINavigationAwareDataContext = new Mock<INavigationAware>();
            mockINavigationAwareDataContext.Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>())).Returns(true);
            mockFrameworkElement.Object.DataContext = mockINavigationAwareDataContext.Object;

            var view = mockFrameworkElement.Object;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            mockINavigationAwareDataContext.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri)));
        }

        [StaFact]
        public void WhenNavigatingAndBothViewAndDataContextImplementINavigationAware_ThenNavigatedIsInvokesOnNavigation()
        {
            // Prepare
            var region = new Region();

            Mock<FrameworkElement> mockFrameworkElement = new Mock<FrameworkElement>();
            Mock<INavigationAware> mockINavigationAwareView = mockFrameworkElement.As<INavigationAware>();
            mockINavigationAwareView.Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>())).Returns(true);

            Mock<INavigationAware> mockINavigationAwareDataContext = new Mock<INavigationAware>();
            mockINavigationAwareDataContext.Setup(ina => ina.IsNavigationTarget(It.IsAny<NavigationContext>())).Returns(true);
            mockFrameworkElement.Object.DataContext = mockINavigationAwareDataContext.Object;

            var view = mockFrameworkElement.Object;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            mockINavigationAwareView.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri)));
            mockINavigationAwareDataContext.Verify(v => v.OnNavigatedTo(It.Is<NavigationContext>(nc => nc.Uri == navigationUri)));
        }

        [Fact]
        public void WhenNavigating_NavigationIsRecordedInJournal()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            IRegionNavigationJournalEntry journalEntry = new RegionNavigationJournalEntry();

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(journalEntry);

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);

            var journalMock = new Mock<IRegionNavigationJournal>();
            journalMock.Setup(x => x.RecordNavigation(journalEntry, true)).Verifiable();

            IRegionNavigationJournal journal = journalMock.Object;


            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
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

            var viewMock = new Mock<IConfirmNavigationRequest>();
            viewMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Verifiable();

            var view = viewMock.Object;
            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
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

            var view1Mock = new Mock<IConfirmNavigationRequest>();
            view1Mock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view1 = view1Mock.Object;
            region.Add(view1);
            region.Activate(view1);

            var view2Mock = new Mock<IConfirmNavigationRequest>();

            var view2 = view2Mock.Object;
            region.Add(view2);

            var view3Mock = new Mock<INavigationAware>();

            var view3 = view3Mock.Object;
            region.Add(view3);
            region.Activate(view3);

            var view4Mock = new Mock<IConfirmNavigationRequest>();
            view4Mock
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
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            view1Mock.VerifyAll();
            view2Mock.Verify(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()), Times.Never());
            view3Mock.VerifyAll();
            view4Mock.VerifyAll();
        }

        [Fact]
        public void WhenRequestNavigateAwayAcceptsThroughCallback_ThenNavigationProceeds()
        {
            // Prepare
            var region = new Region();

            var view1Mock = new Mock<IConfirmNavigationRequest>();
            view1Mock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view1 = view1Mock.Object;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationSucceeded = false;
            target.RequestNavigate(navigationUri, nr => { navigationSucceeded = nr.Result == true; });

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

            var view1Mock = new Mock<IConfirmNavigationRequest>();
            view1Mock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1 = view1Mock.Object;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationFailed = false;
            target.RequestNavigate(navigationUri, nr => { navigationFailed = nr.Result == false; });

            // Verify
            view1Mock.VerifyAll();
            Assert.True(navigationFailed);
            Assert.Equal(new object[] { view1 }, region.ActiveViews.ToArray());
        }

        [StaFact]
        public void WhenNavigatingAndDataContextOnCurrentlyActiveViewImplementsINavigateWithVeto_ThenNavigationRequestQueriesForVeto()
        {
            // Prepare
            var region = new Region();

            var viewModelMock = new Mock<IConfirmNavigationRequest>();
            viewModelMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Verifiable();

            var viewMock = new Mock<FrameworkElement>();

            var view = viewMock.Object;
            view.DataContext = viewModelMock.Object;

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewModelMock.VerifyAll();
        }

        [StaFact]
        public void WhenRequestNavigateAwayOnDataContextAcceptsThroughCallback_ThenNavigationProceeds()
        {
            // Prepare
            var region = new Region();

            var view1DataContextMock = new Mock<IConfirmNavigationRequest>();
            view1DataContextMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(true))
                .Verifiable();

            var view1Mock = new Mock<FrameworkElement>();
            var view1 = view1Mock.Object;
            view1.DataContext = view1DataContextMock.Object;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationSucceeded = false;
            target.RequestNavigate(navigationUri, nr => { navigationSucceeded = nr.Result == true; });

            // Verify
            view1DataContextMock.VerifyAll();
            Assert.True(navigationSucceeded);
            Assert.Equal(new object[] { view1, view2 }, region.ActiveViews.ToArray());
        }

        [StaFact]
        public void WhenRequestNavigateAwayOnDataContextRejectsThroughCallback_ThenNavigationDoesNotProceed()
        {
            // Prepare
            var region = new Region();

            var view1DataContextMock = new Mock<IConfirmNavigationRequest>();
            view1DataContextMock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1Mock = new Mock<FrameworkElement>();
            var view1 = view1Mock.Object;
            view1.DataContext = view1DataContextMock.Object;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationFailed = false;
            target.RequestNavigate(navigationUri, nr => { navigationFailed = nr.Result == false; });

            // Verify
            view1DataContextMock.VerifyAll();
            Assert.True(navigationFailed);
            Assert.Equal(new object[] { view1 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenViewAcceptsNavigationOutAfterNewIncomingRequestIsReceived_ThenOriginalRequestIsIgnored()
        {
            var region = new Region();

            var viewMock = new Mock<IConfirmNavigationRequest>();
            var view = viewMock.Object;

            var confirmationRequests = new List<Action<bool>>();

            viewMock
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
            var journal = new Mock<IRegionNavigationJournal>().Object;

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool firstNavigation = false;
            bool secondNavigation = false;
            target.RequestNavigate(navigationUri, nr => firstNavigation = nr.Result.Value);
            target.RequestNavigate(navigationUri, nr => secondNavigation = nr.Result.Value);

            Assert.Equal(2, confirmationRequests.Count);

            confirmationRequests[0](true);
            confirmationRequests[1](true);

            Assert.False(firstNavigation);
            Assert.True(secondNavigation);
        }

        [StaFact]
        public void WhenViewModelAcceptsNavigationOutAfterNewIncomingRequestIsReceived_ThenOriginalRequestIsIgnored()
        {
            var region = new Region();

            var viewModelMock = new Mock<IConfirmNavigationRequest>();

            var viewMock = new Mock<FrameworkElement>();
            var view = viewMock.Object;
            view.DataContext = viewModelMock.Object;

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
            var journal = new Mock<IRegionNavigationJournal>().Object;

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool firstNavigation = false;
            bool secondNavigation = false;
            target.RequestNavigate(navigationUri, nr => firstNavigation = nr.Result.Value);
            target.RequestNavigate(navigationUri, nr => secondNavigation = nr.Result.Value);

            Assert.Equal(2, confirmationRequests.Count);

            confirmationRequests[0](true);
            confirmationRequests[1](true);

            Assert.False(firstNavigation);
            Assert.True(secondNavigation);
        }

        [Fact]
        public void BeforeNavigating_NavigatingEventIsRaised()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool isNavigatingRaised = false;
            target.Navigating += delegate(object sender, RegionNavigationEventArgs e)
            {
                if (sender == target)
                {
                    isNavigatingRaised = true;
                }
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            Assert.True(isNavigatingRaised);
        }

        [Fact]
        public void WhenNavigationSucceeds_NavigatedIsRaised()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool isNavigatedRaised = false;
            target.Navigated += delegate(object sender, RegionNavigationEventArgs e)
            {
                if (sender == target)
                {
                    isNavigatedRaised = true;
                }
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

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
            var viewMock = new Mock<IConfirmNavigationRequest>();
            viewMock
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
            Assert.Same(targetException, result.Error);
        }

        [Fact]
        public void WhenNavigatingFromViewThatIsNavigationAware_ThenNotifiesActiveViewNavigatingFrom()
        {
            // Arrange
            var region = new Region();
            var viewMock = new Mock<INavigationAware>();
            var view = viewMock.Object;
            region.Add(view);

            var view2 = new object();
            region.Add(view2);

            region.Activate(view);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);
            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewMock.Verify(v => v.OnNavigatedFrom(It.Is<NavigationContext>(ctx => ctx.Uri == navigationUri && ctx.Parameters.Count() == 0)));
        }

        [Fact]
        public void WhenNavigationFromViewThatIsNavigationAware_OnlyNotifiesOnNavigateFromForActiveViews()
        {
            // Arrange

            bool navigationFromInvoked = false;

            var region = new Region();

            var viewMock = new Mock<INavigationAware>();
            viewMock
                .Setup(x => x.OnNavigatedFrom(It.IsAny<NavigationContext>())).Callback(() => navigationFromInvoked = true);
            var view = viewMock.Object;
            region.Add(view);

            var targetViewMock = new Mock<INavigationAware>();
            region.Add(targetViewMock.Object);

            var activeViewMock = new Mock<INavigationAware>();
            region.Add(activeViewMock.Object);

            region.Activate(activeViewMock.Object);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(targetViewMock.Object.GetType().Name, UriKind.Relative);
            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            Assert.False(navigationFromInvoked);
        }

        [StaFact]
        public void WhenNavigatingFromActiveViewWithNavigatinAwareDataConext_NotifiesContextOfNavigatingFrom()
        {
            // Arrange
            var region = new Region();

            var mockDataContext = new Mock<INavigationAware>();

            var view1Mock = new Mock<FrameworkElement>();
            var view1 = view1Mock.Object;
            view1.DataContext = mockDataContext.Object;

            region.Add(view1);

            var view2 = new object();
            region.Add(view2);

            region.Activate(view1);

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IRegionNavigationJournalEntry))).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);
            IContainerExtension container = containerMock.Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            mockDataContext.Verify(v => v.OnNavigatedFrom(It.Is<NavigationContext>(ctx => ctx.Uri == navigationUri && ctx.Parameters.Count() == 0)));
        }

        [Fact]
        public void WhenNavigatingWithNullCallback_ThenThrows()
        {
            var region = new Region();

            var navigationUri = new Uri("/", UriKind.Relative);
            IContainerExtension container = new Mock<IContainerExtension>().Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    target.RequestNavigate(navigationUri, null);
                });
        }

        [Fact]
        public void WhenNavigatingWithNoRegionSet_ThenMarshallExceptionToCallback()
        {
            var navigationUri = new Uri("/", UriKind.Relative);
            IContainerExtension container = new Mock<IContainerExtension>().Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal);

            Exception error = null;
            target.RequestNavigate(navigationUri, nr => error = nr.Error);

            Assert.NotNull(error);
            Assert.IsType<InvalidOperationException>(error);
        }

        [Fact]
        public void WhenNavigatingWithNullUri_ThenMarshallExceptionToCallback()
        {
            IContainerExtension container = new Mock<IContainerExtension>().Object;
            RegionNavigationContentLoader contentLoader = new Mock<RegionNavigationContentLoader>(container).Object;
            IRegionNavigationJournal journal = new Mock<IRegionNavigationJournal>().Object;

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = new Region()
            };

            Exception error = null;
            target.RequestNavigate(null, nr => error = nr.Error);

            Assert.NotNull(error);
            Assert.IsType<ArgumentNullException>(error);
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
            var journal = new Mock<IRegionNavigationJournal>().Object;

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate(object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(new Uri("invalid", UriKind.Relative), nr => isNavigationSuccessful = nr.Result);

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

            var view1Mock = new Mock<IConfirmNavigationRequest>();
            view1Mock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1 = view1Mock.Object;

            var view2 = new object();

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
            var journal = new Mock<IRegionNavigationJournal>().Object;

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate(object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(navigationUri, nr => isNavigationSuccessful = nr.Result);

            // Verify
            view1Mock.VerifyAll();
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.Null(eventArgs.Error);
        }

        [StaFact]
        public void WhenNavigationFailsBecauseDataContextForActiveViewRejectsIt_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var viewModel1Mock = new Mock<IConfirmNavigationRequest>();
            viewModel1Mock
                .Setup(ina => ina.ConfirmNavigationRequest(It.IsAny<NavigationContext>(), It.IsAny<Action<bool>>()))
                .Callback<NavigationContext, Action<bool>>((nc, c) => c(false))
                .Verifiable();

            var view1Mock = new Mock<FrameworkElement>();
            var view1 = view1Mock.Object;
            view1.DataContext = viewModel1Mock.Object;

            var view2 = new object();

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
            var journal = new Mock<IRegionNavigationJournal>().Object;

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate(object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(navigationUri, nr => isNavigationSuccessful = nr.Result);

            // Verify
            viewModel1Mock.VerifyAll();
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.Null(eventArgs.Error);
        }
    }
}
