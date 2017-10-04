using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigationServiceFixture_NavigationPage : IDisposable
    {
        PageNavigationContainerMock _container;
        IApplicationProvider _applicationProvider;
        ILoggerFacade _loggerFacade;

        public PageNavigationServiceFixture_NavigationPage()
        {
            _container = new PageNavigationContainerMock();
            _container.Register("NavigationPage", typeof(NavigationPageMock));
            _container.Register("NavigationPage-Empty", typeof(NavigationPageEmptyMock));
            _container.Register("NavigationPageWithStack", typeof(NavigationPageWithStackMock));
            _container.Register("NavigationPageWithStackNoMatch", typeof(NavigationPageWithStackNoMatchMock));

            _container.Register("ContentPage", typeof(ContentPageMock));
            _container.Register("TabbedPage", typeof(TabbedPageMock));
            _container.Register("CarouselPage", typeof(CarouselPageMock));

            _container.Register("PageMock", typeof(PageMock));
            _container.Register("SecondContentPageMock", typeof(SecondContentPageMock));

            _applicationProvider = new ApplicationProviderMock();
            _loggerFacade = new EmptyLogger();
        }


        [Fact]
        public async void Navigate_FromNavigationPage_ToContentPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage", useModalNavigation: false);

            Assert.True(rootPage.Navigation.NavigationStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack[0]);
        }

        //TODO: rename tests to follow a new test naming convention 
        [Fact]
        public async Task Navigate_FromNavigationPage_WithoutChildPage_ToContentPage()
        {
            var recorder = new PageNavigationEventRecorder();
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
            var navigationPage = new NavigationPageEmptyMock(recorder);

            ((IPageAware)navigationService).Page = navigationPage;
            await navigationService.NavigateAsync("ContentPage");

            Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
            Assert.Equal(1, navigationPage.Navigation.NavigationStack.Count);
            var contentPage = navigationPage.Navigation.NavigationStack.Last();
            Assert.IsType<ContentPageMock>(contentPage);

            var record = recorder.TakeFirst();
            Assert.Equal(contentPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigationPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigationPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            Assert.True(recorder.IsEmpty);
        }

        //TODO: reimplement test to check order of events when navigating in a navigationpage. because of reverse navigation, this no longer is valid.
        [Fact]
        public async Task NavigateAsync_From_ChildPageOfNavigationPage()
        {
            var recorder = new PageNavigationEventRecorder(); ;
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
            var contentPageMock = new ContentPageMock(recorder);
            var navigationPage = new NavigationPageMock(recorder, contentPageMock);

            // Navigate to Page2
            ((IPageAware)navigationService).Page = contentPageMock;
            await navigationService.NavigateAsync("SecondContentPageMock");

            Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
            Assert.Equal(2, navigationPage.Navigation.NavigationStack.Count);

            var pageMock = navigationPage.Navigation.NavigationStack.Last();

            Assert.IsType<SecondContentPageMock>(pageMock);

            var record = recorder.TakeFirst();
            Assert.Equal(pageMock, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(pageMock.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMock, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMock.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(pageMock, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(pageMock.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            Assert.True(recorder.IsEmpty);
        }

        //TODO: reimplement test to check order of events when navigating in a navigationpage. because of reverse navigation, this no longer is valid.
        [Fact]
        public async Task NavigateAsync_From_NavigationPage_With_ChildPage_And_DoesNotReplaseRootPage()
        {
            var recorder = new PageNavigationEventRecorder(); ;
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
            var contentPageMock = new ContentPageMock(recorder);
            var contentPageMockViewModel = contentPageMock.BindingContext;
            var navigationPage = new NavigationPageMock(recorder, contentPageMock);

            // Navigate to Page2
            ((IPageAware)navigationService).Page = contentPageMock;
            await navigationService.NavigateAsync("SecondContentPageMock");

            var secondContentPage = navigationPage.Navigation.NavigationStack.Last();
            var secondContentPageViewModel = secondContentPage.BindingContext;

            recorder.Clear();
            // PopToRootAsync
            ((IPageAware)navigationService).Page = navigationPage;
            await navigationService.NavigateAsync("ContentPage");

            Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
            Assert.Equal(1, navigationPage.Navigation.NavigationStack.Count);

            var rootPage = navigationPage.Navigation.NavigationStack.Last();
            Assert.Equal(contentPageMock, rootPage);

            var record = recorder.TakeFirst();
            Assert.Equal(contentPageMock, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMockViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMock, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMockViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMock, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(contentPageMock.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPage, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            Assert.True(recorder.IsEmpty);
        }

        //TODO: reimplement test to check order of events when navigating in a navigationpage. because of reverse navigation, this no longer is valid.
        //[Fact]
        //public async Task NavigateAsync_From_NavigationPage_With_ChildPage_And_ReplaseRootPage()
        //{
        //    var recorder = new PageNavigationEventRecorder(); ;
        //    var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
        //    var secondContentPageMock = new SecondContentPageMock(recorder);
        //    var secondContentPageMockViewModel = secondContentPageMock.BindingContext;
        //    var navigationPage = new NavigationPageMock(recorder, secondContentPageMock);

        //    // Navigate to Page2
        //    ((IPageAware)navigationService).Page = secondContentPageMock;
        //    await navigationService.NavigateAsync("MasterDetailPage");

        //    var masterDetailPage = navigationPage.Navigation.NavigationStack.Last();
        //    var masterDetailPageViewModel = masterDetailPage.BindingContext;

        //    recorder.Clear();
        //    // PopToRootAsync
        //    ((IPageAware)navigationService).Page = navigationPage;
        //    await navigationService.NavigateAsync("ContentPage");

        //    Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
        //    Assert.Equal(1, navigationPage.Navigation.NavigationStack.Count);

        //    var contentPage = navigationPage.Navigation.NavigationStack.Last();
        //    Assert.NotEqual(secondContentPageMock, contentPage);

        //    var record = recorder.TakeFirst();
        //    Assert.Equal(contentPage, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(contentPage.BindingContext, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(secondContentPageMock, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(secondContentPageMockViewModel, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(contentPage, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(contentPage.BindingContext, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(masterDetailPage, record.Sender);
        //    Assert.Equal(PageNavigationEvent.Destroy, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(masterDetailPageViewModel, record.Sender);
        //    Assert.Equal(PageNavigationEvent.Destroy, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(secondContentPageMock, record.Sender);
        //    Assert.Equal(PageNavigationEvent.Destroy, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(secondContentPageMockViewModel, record.Sender);
        //    Assert.Equal(PageNavigationEvent.Destroy, record.Event);

        //    Assert.True(recorder.IsEmpty);
        //}

        //TODO: reimplement test to check order of events when navigating in a navigationpage. because of reverse navigation, this no longer is valid.
        //[Fact]
        //public async Task NavigateAsync_From_NavigationPage_When_NotClearNavigationStack()
        //{
        //    var recorder = new PageNavigationEventRecorder(); ;
        //    var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
        //    var contentPageMock = new ContentPageMock(recorder);
        //    var navigationPage = new NavigationPageMock(recorder, contentPageMock);
        //    navigationPage.ClearNavigationStackOnNavigation = false;

        //    // Navigate to Page2
        //    ((IPageAware)navigationService).Page = contentPageMock;
        //    await navigationService.NavigateAsync("SecondContentPageMock");

        //    var secondContentPage = navigationPage.Navigation.NavigationStack.Last();
        //    var secondContentPageViewModel = secondContentPage.BindingContext;

        //    recorder.Clear();
        //    ((IPageAware)navigationService).Page = navigationPage;
        //    await navigationService.NavigateAsync("ContentPage");

        //    Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
        //    Assert.Equal(3, navigationPage.Navigation.NavigationStack.Count);

        //    var currentPage = navigationPage.Navigation.NavigationStack.Last();
        //    Assert.NotEqual(contentPageMock, currentPage);

        //    var record = recorder.TakeFirst();
        //    Assert.Equal(currentPage, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(currentPage.BindingContext, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(secondContentPage, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(secondContentPageViewModel, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(currentPage, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

        //    record = recorder.TakeFirst();
        //    Assert.Equal(currentPage.BindingContext, record.Sender);
        //    Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

        //    Assert.True(recorder.IsEmpty);
        //}
        
        [Fact]
        public async Task NavigateAsync_From_NavigationPage_When_NotClearNavigationStack_And_SamePage()
        {
            var recorder = new PageNavigationEventRecorder(); ;
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
            var contentPageMock = new ContentPageMock(recorder);
            var navigationPage = new NavigationPageMock(recorder, contentPageMock);
            navigationPage.ClearNavigationStackOnNavigation = false;

            // Navigate to Page2
            ((IPageAware)navigationService).Page = contentPageMock;
            await navigationService.NavigateAsync("SecondContentPageMock");

            var secondContentPage = navigationPage.Navigation.NavigationStack.Last();

            recorder.Clear();
            ((IPageAware)navigationService).Page = navigationPage;
            await navigationService.NavigateAsync("SecondContentPageMock");

            Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
            Assert.Equal(2, navigationPage.Navigation.NavigationStack.Count);

            var currentPage = navigationPage.Navigation.NavigationStack.Last();
            Assert.Equal(secondContentPage, currentPage);

            var record = recorder.TakeFirst();
            Assert.Equal(secondContentPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(secondContentPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            Assert.True(recorder.IsEmpty);
        }


        [Fact]
        public async Task DeepNavigate_ToNavigationPage_ToTabbedPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("NavigationPage/ContentPage/TabbedPage/PageMock");

            var navPage = rootPage.Navigation.ModalStack[0] as NavigationPageMock;
            Assert.NotNull(navPage);

            var contentPage = navPage.Navigation.NavigationStack[0] as ContentPageMock;
            Assert.NotNull(contentPage);

            var tabbedPage = navPage.Navigation.NavigationStack[1] as TabbedPageMock;
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<PageMock>(tabbedPage.CurrentPage);
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
