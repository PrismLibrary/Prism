using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Forms.Tests.Navigation.Mocks.Views;
using Prism.Logging;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigationServiceFixture : IDisposable
    {
        PageNavigationContainerMock _container;
        IApplicationProvider _applicationProvider;
        ILoggerFacade _loggerFacade;

        public PageNavigationServiceFixture()
        {
            Xamarin.Forms.Mocks.MockForms.Init();

            _container = new PageNavigationContainerMock();

            _container.Register("PageMock", typeof(PageMock));

            _container.Register("ContentPage", typeof(ContentPageMock));
            _container.Register("ContentPage1", typeof(ContentPageMock1));
            _container.Register(typeof(ContentPageMockViewModel).FullName, typeof(ContentPageMock));
            _container.Register(typeof(ContentPageMock1ViewModel).FullName, typeof(ContentPageMock1));

            _container.Register("SecondContentPageMock", typeof(SecondContentPageMock));

            _container.Register("NavigationPage", typeof(NavigationPageMock));
            _container.Register("NavigationPage-Empty", typeof(NavigationPageEmptyMock));
            _container.Register("NavigationPageWithStack", typeof(NavigationPageWithStackMock));
            _container.Register("NavigationPageWithStackNoMatch", typeof(NavigationPageWithStackNoMatchMock));

            _container.Register("MasterDetailPage", typeof(MasterDetailPageMock));
            _container.Register("MasterDetailPage-Empty", typeof(MasterDetailPageEmptyMock));


            _container.Register("TabbedPage", typeof(TabbedPageMock));
            _container.Register("TabbedPage-Empty", typeof(TabbedPageEmptyMock));
            _container.Register("Tab1", typeof(Tab1Mock));
            _container.Register("Tab2", typeof(Tab2Mock));
            _container.Register("Tab3", typeof(Tab3Mock));


            _container.Register("CarouselPage", typeof(CarouselPageMock));

            _applicationProvider = new ApplicationProviderMock();
            _loggerFacade = new EmptyLogger();
        }

        [Fact]
        public void IPageAware_NullByDefault()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var page = ((IPageAware)navigationService).Page;
            Assert.Null(page);
        }

        [Fact]
        public void Navigate_ToUnregisteredPage_ByName()
        {
            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
                var rootPage = new ContentPage();
                ((IPageAware)navigationService).Page = rootPage;

                await navigationService.NavigateAsync("UnregisteredPage");

                Assert.True(rootPage.Navigation.ModalStack.Count == 0);
            });
        }

        [Fact]
        public async void Navigate_ToContentPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async void Navigate_ToContentPage_ByRelativeUri()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync(new Uri("ContentPage", UriKind.Relative));

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async void Navigate_ToContentPage_ByAbsoluteName()
        {
            // Set up top page.
            var recorder = new PageNavigationEventRecorder();
            var rootPage = new ContentPageMock(recorder);
            var rootPageViewModel = (ViewModelBase)rootPage.BindingContext;
            var applicationProvider = new ApplicationProviderMock(rootPage);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade, recorder);

            await navigationService.NavigateAsync("/ContentPage");

            var navigatedPage = applicationProvider.MainPage;
            Assert.IsType<ContentPageMock>(navigatedPage);
            Assert.NotEqual(rootPage, _applicationProvider.MainPage);

            var record = recorder.TakeFirst();
            Assert.Equal(navigatedPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigatedPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigatedPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigatedPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPage, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            Assert.True(recorder.IsEmpty);
        }

        [Fact]
        public async void Navigate_ToContentPage_ByAbsoluteUri()
        {
            // Set up top page.
            var recorder = new PageNavigationEventRecorder(); ;
            var rootPage = new ContentPageMock(recorder);
            var rootPageViewModel = (ViewModelBase)rootPage.BindingContext;
            var applicationProvider = new ApplicationProviderMock(rootPage);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade, recorder);

            await navigationService.NavigateAsync(new Uri("http://localhost/ContentPage", UriKind.Absolute));

            var navigatedPage = applicationProvider.MainPage;
            Assert.IsType<ContentPageMock>(navigatedPage);
            Assert.NotEqual(rootPage, _applicationProvider.MainPage);

            var record = recorder.TakeFirst();
            Assert.Equal(navigatedPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigatedPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedFrom, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigatedPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(navigatedPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPage, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(rootPageViewModel, record.Sender);
            Assert.Equal(PageNavigationEvent.Destroy, record.Event);

            Assert.True(recorder.IsEmpty);
        }



        [Fact]
        public async void Navigate_ToContentPage_ByName_WithNavigationParameters()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            var navParameters = new NavigationParameters();
            navParameters.Add("id", 3);

            await navigationService.NavigateAsync("ContentPage", navParameters);

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as ContentPageMockViewModel;
            Assert.NotNull(viewModel);

            Assert.NotNull(viewModel.NavigatedToParameters);
            Assert.True(viewModel.NavigatedToParameters.Count > 0);
            Assert.Equal(3, viewModel.NavigatedToParameters["id"]);
        }

        [Fact]
        public async void Navigate_ToContentPage_ThenGoBack()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);

            var result = await navigationService.GoBackAsync();

            Assert.True(result.Success);
            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public async void NavigateAsync_ToContentPage_ThenGoBack()
        {
            var pageMock = new ContentPageMock();
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            ((IPageAware)navigationService).Page = pageMock;

            var rootPage = new NavigationPage(pageMock);

            Assert.IsType<ContentPageMock>(rootPage.CurrentPage);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);
            Assert.IsType<TabbedPageMock>(rootPage.CurrentPage);
            var tabbedPageMock = rootPage.CurrentPage as TabbedPageMock;
            Assert.NotNull(tabbedPageMock);
            var viewModel = (ViewModelBase)tabbedPageMock.BindingContext;

            var result = await navigationService.GoBackAsync();

            Assert.True(result.Success);
            Assert.True(rootPage.Navigation.NavigationStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.CurrentPage);
            Assert.True(tabbedPageMock.DestroyCalled);
            Assert.Null(tabbedPageMock.BindingContext);
            Assert.True(viewModel.DestroyCalled);
        }

        [Fact]
        public async void Navigate_ToContentPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as ContentPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);

            var nextPageNavService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            ((IPageAware)nextPageNavService).Page = rootPage.Navigation.ModalStack[0];
            await nextPageNavService.NavigateAsync("NavigationPage");

            Assert.True(viewModel.OnNavigatedFromCalled);
        }

        [Fact]
        public async void Navigate_ToContentPage_PageHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var contentPage = rootPage.Navigation.ModalStack[0] as ContentPageMock;
            Assert.NotNull(contentPage);
            Assert.True(contentPage.OnNavigatedToCalled);

            var nextPageNavService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            ((IPageAware)nextPageNavService).Page = contentPage;

            await nextPageNavService.NavigateAsync("NavigationPage");

            Assert.True(contentPage.OnNavigatedFromCalled);
            Assert.True(contentPage.Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void Navigate_ToContentPage_PageHasIConfirmNavigation_True()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.False(rootPage.OnConfirmNavigationCalled);

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.OnConfirmNavigationCalled);
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void Navigate_ToContentPage_PageHasIConfirmNavigation_False()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.False(rootPage.OnConfirmNavigationCalled);

            var navParams = new NavigationParameters();
            navParams.Add("canNavigate", false);

            await navigationService.NavigateAsync("ContentPage", navParams);

            Assert.True(rootPage.OnConfirmNavigationCalled);
            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public async void Navigate_ToContentPage_ViewModelHasIConfirmNavigation_True()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage() { BindingContext = new ContentPageMockViewModel() };
            ((IPageAware)navigationService).Page = rootPage;

            var viewModel = rootPage.BindingContext as ContentPageMockViewModel;
            Assert.False(viewModel.OnConfirmNavigationCalled);

            await navigationService.NavigateAsync("ContentPage");
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnConfirmNavigationCalled);
        }

        [Fact]
        public async void Navigate_ToContentPage_ViewModelHasIConfirmNavigation_False()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage() { BindingContext = new ContentPageMockViewModel() };
            ((IPageAware)navigationService).Page = rootPage;

            var viewModel = rootPage.BindingContext as ContentPageMockViewModel;
            Assert.False(viewModel.OnConfirmNavigationCalled);

            var navParams = new NavigationParameters();
            navParams.Add("canNavigate", false);

            await navigationService.NavigateAsync("ContentPage", navParams);

            Assert.True(viewModel.OnConfirmNavigationCalled);
            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public async void Navigate_ToNavigatonPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("NavigationPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<NavigationPageMock>(rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as NavigationPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public async void Navigate_ToMasterDetailPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var mdPage = rootPage.Navigation.ModalStack[0] as MasterDetailPage;
            Assert.NotNull(mdPage);

            var viewModel = mdPage.BindingContext as MasterDetailPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public async void Navigate_ToTabbedPage_ByName_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("TabbedPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var mdPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(mdPage);

            var viewModel = mdPage.BindingContext as TabbedPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public async void Navigate_ToCarouselPage_ByName_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("CarouselPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var cPage = rootPage.Navigation.ModalStack[0] as CarouselPageMock;
            Assert.NotNull(cPage);

            var viewModel = cPage.BindingContext as CarouselPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public async void Navigate_FromNavigationPage_ToContentPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

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
        public async Task DeepNavigate_ToNavigationPage_ToTabbedPage_SelectContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"NavigationPage/ContentPage/TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2");

            var navPage = rootPage.Navigation.ModalStack[0] as NavigationPageMock;
            Assert.NotNull(navPage);

            var contentPage = navPage.Navigation.NavigationStack[0] as ContentPageMock;
            Assert.NotNull(contentPage);

            var tabbedPage = navPage.Navigation.NavigationStack[1] as TabbedPageMock;
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_ContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage_ToContentPage_ByAbsoluteName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("/NavigationPage/ContentPage");

            Assert.Equal(0, rootPage.Navigation.ModalStack.Count);

            var navPage = _applicationProvider.MainPage;
            Assert.IsType<NavigationPageMock>(navPage);
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage_ToContentPage_ByAbsoluteUri()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync(new Uri("http://localhost/NavigationPage/ContentPage", UriKind.Absolute));

            Assert.Equal(0, rootPage.Navigation.ModalStack.Count);

            var navPage = _applicationProvider.MainPage;
            Assert.IsType<NavigationPageMock>(navPage);
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_EmptyNavigationPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage-Empty/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }


        [Fact]
        public async void DeepNavigate_From_ContentPage_To_EmptyNavigationPage_ToContentPage_toContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage-Empty/ContentPage/ContentPage1");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];

            Assert.True(navPage.Navigation.NavigationStack.Count == 2);
            var lastPage = navPage.Navigation.NavigationStack.LastOrDefault();
            Assert.True(lastPage.GetType() == typeof(ContentPageMock1));
            await navPage.Navigation.PopAsync();
            lastPage = navPage.Navigation.NavigationStack.LastOrDefault();
            Assert.True(lastPage.GetType() == typeof(ContentPageMock));
        }

        [Fact]
        public async void DeepNavigate_To_EmptyNavigationPage_ToContentPage_toContentPage_toContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("NavigationPage-Empty/ContentPage/ContentPage/ContentPage1");

            var navPage = rootPage.Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 3);
            var lastPage = navPage.Navigation.NavigationStack.LastOrDefault();
            Assert.True(lastPage.GetType() == typeof(ContentPageMock1));
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPageWithNavigationStack_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStack/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPageWithNavigationStack_ToContentPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStack/ContentPage/ContentPage1");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 2);
            var lastPage = navPage.Navigation.NavigationStack.LastOrDefault();
            Assert.True(lastPage.GetType() == typeof(ContentPageMock1));
            await navPage.Navigation.PopAsync();
            lastPage = navPage.Navigation.NavigationStack.LastOrDefault();
            Assert.True(lastPage.GetType() == typeof(ContentPageMock));
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPageWithDifferentNavigationStack_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStackNoMatch/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.Equal(1, navPage.Navigation.NavigationStack.Count);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_TabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/TabbedPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_CarouselPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/CarouselPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_MasterDetailPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/MasterDetailPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        #region MasterDetailPage

        [Fact]
        public async void Navigate_FromMasterDetailPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.IsType<ContentPageMock>(rootPage.Detail);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.IsType<TabbedPageMock>(rootPage.Detail);

            await navigationService.NavigateAsync("CarouselPage");

            Assert.IsType<CarouselPageMock>(rootPage.Detail);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToSamePage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.IsType<ContentPageMock>(rootPage.Detail);

            await navigationService.NavigateAsync("TabbedPage");

            var firstDetailPage = rootPage.Detail;

            Assert.IsType<TabbedPageMock>(firstDetailPage);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.Equal(firstDetailPage, rootPage.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage");

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(0, rootPage.Navigation.NavigationStack.Count);

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<ContentPageMock>(masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToContentPage_UseModalNavigation()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            //await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage", useModalNavigation: true);
            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage");

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(0, rootPage.Navigation.NavigationStack.Count);
            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<ContentPageMock>(masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToContentPage_NotUseModalNavigation()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPageMock();
            var navigationPage = new NavigationPage(rootPage);
            ((IPageAware)navigationService).Page = rootPage;

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(navigationPage.CurrentPage);

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage");

            Assert.Equal(0, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
            var masterDetail = rootPage.Navigation.NavigationStack[1] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<ContentPageMock>(masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToNavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/NavigationPage");

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<NavigationPageMock>(masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToEmptyNavigationPage_ToContentPage()
        {
            var applicationProvider = new ApplicationProviderMock(null);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade);

            await navigationService.NavigateAsync("MasterDetailPage-Empty/NavigationPage-Empty/ContentPage");

            var masterDetail = applicationProvider.MainPage as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<NavigationPageEmptyMock>(masterDetail.Detail);
            Assert.Equal(0, masterDetail.Navigation.ModalStack.Count);
            Assert.Equal(0, masterDetail.Navigation.NavigationStack.Count);
            Assert.Equal(0, masterDetail.Detail.Navigation.ModalStack.Count);
            Assert.Equal(1, masterDetail.Detail.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(masterDetail.Detail.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToNavigationPage_ToContentPage()
        {
            var applicationProvider = new ApplicationProviderMock(null);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade);

            await navigationService.NavigateAsync("MasterDetailPage-Empty/NavigationPage/PageMock");

            var masterDetail = applicationProvider.MainPage as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<NavigationPageMock>(masterDetail.Detail);
            Assert.Equal(0, masterDetail.Navigation.ModalStack.Count);
            Assert.Equal(0, masterDetail.Navigation.NavigationStack.Count);
            Assert.Equal(0, masterDetail.Detail.Navigation.ModalStack.Count);
            Assert.Equal(1, masterDetail.Detail.Navigation.NavigationStack.Count);
            Assert.IsType<PageMock>(masterDetail.Detail.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToDifferentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage/TabbedPage");

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<TabbedPageMock>(masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToSamePage_ToTabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage/ContentPage/TabbedPage");

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType<ContentPageMock>(masterDetail.Detail);

            var tabbedPage = masterDetail.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToTabbedPage_IsPresented()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;
            rootPage.IsPresentedAfterNavigation = true;

            Assert.IsType<ContentPageMock>(rootPage.Detail);
            Assert.False(rootPage.IsPresented);

            await navigationService.NavigateAsync("TabbedPage");
            Assert.IsType<TabbedPageMock>(rootPage.Detail);

            Assert.True(rootPage.IsPresented);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToTabbedPage_IsNotPresented()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;
            rootPage.IsPresentedAfterNavigation = false;

            Assert.IsType<ContentPageMock>(rootPage.Detail);
            Assert.False(rootPage.IsPresented);

            await navigationService.NavigateAsync("TabbedPage");
            Assert.IsType<TabbedPageMock>(rootPage.Detail);

            Assert.False(rootPage.IsPresented);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToTabbedPage_IsPresented_FromViewModel()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageEmptyMock();
            ((IPageAware)navigationService).Page = rootPage;

            ((MasterDetailPageEmptyMockViewModel)rootPage.BindingContext).IsPresentedAfterNavigation = true;

            Assert.Null(rootPage.Detail);
            Assert.False(rootPage.IsPresented);

            await navigationService.NavigateAsync("TabbedPage");
            Assert.IsType<TabbedPageMock>(rootPage.Detail);

            Assert.True(rootPage.IsPresented);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToTabbedPage_IsNotPresented_FromViewModel()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageEmptyMock();
            ((IPageAware)navigationService).Page = rootPage;

            ((MasterDetailPageEmptyMockViewModel)rootPage.BindingContext).IsPresentedAfterNavigation = false;

            Assert.Null(rootPage.Detail);
            Assert.False(rootPage.IsPresented);

            await navigationService.NavigateAsync("TabbedPage");
            Assert.IsType<TabbedPageMock>(rootPage.Detail);

            Assert.False(rootPage.IsPresented);
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToNavigationPage_ToTabbedPage_SelectTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"MasterDetailPage-Empty/NavigationPage/TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2");

            var mdpPage = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            var navPage = mdpPage.Detail as NavigationPageMock;
            var tabbedPage = navPage.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.NotNull(mdpPage);
            Assert.NotNull(navPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToNavigationPage_ToContentPage_ToTabbedPage_SelectTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"MasterDetailPage-Empty/NavigationPage/ContentPage/TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2");

            var mdpPage = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            var navPage = mdpPage.Detail as NavigationPageMock;
            var contentPage = navPage.Navigation.NavigationStack[0] as ContentPageMock;
            var tabbedPage = navPage.Navigation.NavigationStack[1] as TabbedPageMock;
            Assert.NotNull(mdpPage);
            Assert.NotNull(navPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void DeepNavigate_FromMasterDetailPage_ToExistingNavigationPage_ToExistingTabbedPage_SelectTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageEmptyMock();
            rootPage.Detail = new NavigationPageEmptyMock();
            await rootPage.Detail.Navigation.PushAsync(new TabbedPageMock());
            ((IPageAware)navigationService).Page = rootPage;

            var navPage = rootPage.Detail as NavigationPageEmptyMock;
            var tabbedPage = navPage.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.NotNull(navPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab1Mock>(tabbedPage.CurrentPage);

            await navigationService.NavigateAsync($"NavigationPage-Empty/TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab3");

            var existingNavPage = rootPage.Detail as NavigationPageEmptyMock;
            var existingTabbedPage = navPage.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.Equal(navPage, existingNavPage);
            Assert.Equal(tabbedPage, existingTabbedPage);
            Assert.IsType<Tab3Mock>(tabbedPage.CurrentPage);
        }

        #endregion

        #region TabbedPage

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
        }

        [Fact]
        public async void Navigate_FromNavigationPage_ToTabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 1);
            Assert.IsType<TabbedPageMock>(rootPage.Navigation.NavigationStack[0]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage/ContentPage");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);

            Assert.True(tabbedPage.Navigation.ModalStack.Count > 0);

            Assert.IsType<ContentPageMock>(tabbedPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async void Navigate_FromNavigationPage_ToTabbedPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage/ContentPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);
            Assert.IsType<TabbedPageMock>(rootPage.Navigation.NavigationStack[0]);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack[1]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_SelectedTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_SelectedTab_NavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage?{KnownNavigationParameters.SelectedTab}=ContentPage");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);

            var navPage = tabbedPage.CurrentPage as NavigationPageMock;
            Assert.NotNull(navPage);
            Assert.IsType<ContentPageMock>(navPage.CurrentPage);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_SelectedTab_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2/ContentPage");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);

            var contentPage = tabbedPage.Navigation.ModalStack[0] as ContentPageMock;
            Assert.NotNull(contentPage);
        }

        [Fact]
        public async void Navigate_FromNavigationPage_ToTabbedPage_SelectedTab_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2/ContentPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);

            var tabbedPage = rootPage.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);

            var contentPage = tabbedPage.Navigation.NavigationStack[1] as ContentPageMock;
            Assert.NotNull(contentPage);
        }

        [Fact]
        public async void Navigate_FromNavigationPage_ToTabbedPage_SelectedTab_NavigationPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage?{KnownNavigationParameters.SelectedTab}=PageMock/ContentPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);

            var tabbedPage = rootPage.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<NavigationPageMock>(tabbedPage.CurrentPage);

            var navPage = tabbedPage.CurrentPage as NavigationPageMock;
            Assert.NotNull(navPage);
            Assert.IsType<PageMock>(navPage.CurrentPage);

            var contentPage = tabbedPage.Navigation.NavigationStack[1] as ContentPageMock;
            Assert.NotNull(contentPage);            
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToNavigationPage_ToTabbedPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.MasterDetailPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"NavigationPage/TabbedPage/ContentPage");

            Assert.IsType<NavigationPageMock>(rootPage.Detail);

            Assert.True(rootPage.Detail.Navigation.NavigationStack.Count == 2);

            Assert.IsType<TabbedPageMock>(rootPage.Detail.Navigation.NavigationStack[0]);
            Assert.IsType<ContentPageMock>(rootPage.Detail.Navigation.NavigationStack[1]);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToNavigationPage_ToTabbedPage_SelectedTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.MasterDetailPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"NavigationPage/TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2");

            Assert.IsType<NavigationPageMock>(rootPage.Detail);

            Assert.True(rootPage.Detail.Navigation.NavigationStack.Count == 1);

            var tabbedPage = rootPage.Detail.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToNavigationPage_ToTabbedPage_SelectedTab_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.MasterDetailPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"NavigationPage/TabbedPage?{KnownNavigationParameters.SelectedTab}=Tab2/ContentPage");

            Assert.IsType<NavigationPageMock>(rootPage.Detail);

            Assert.True(rootPage.Detail.Navigation.NavigationStack.Count == 2);

            Assert.IsType<TabbedPageMock>(rootPage.Detail.Navigation.NavigationStack[0]);            
            Assert.IsType<Tab2Mock>(((TabbedPageMock)rootPage.Detail.Navigation.NavigationStack[0]).CurrentPage);

            Assert.IsType<ContentPageMock>(rootPage.Detail.Navigation.NavigationStack[1]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_CreateTabs()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage-Empty?{KnownNavigationParameters.CreateTab}=Tab1&{KnownNavigationParameters.CreateTab}=Tab2&{KnownNavigationParameters.CreateTab}=Tab3");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageEmptyMock;
            Assert.NotNull(tabbedPage);
            Assert.Equal(3, tabbedPage.Children.Count());
            Assert.IsType<Tab1Mock>(tabbedPage.Children[0]);
            Assert.IsType<Tab2Mock>(tabbedPage.Children[1]);
            Assert.IsType<Tab3Mock>(tabbedPage.Children[2]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_CreateTabs_SelectTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage-Empty?{KnownNavigationParameters.CreateTab}=Tab1&{KnownNavigationParameters.CreateTab}=Tab2&{KnownNavigationParameters.CreateTab}=Tab3&{KnownNavigationParameters.SelectedTab}=Tab2");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageEmptyMock;
            Assert.NotNull(tabbedPage);
            Assert.Equal(3, tabbedPage.Children.Count());
            Assert.IsType<Tab1Mock>(tabbedPage.Children[0]);
            Assert.IsType<Tab2Mock>(tabbedPage.Children[1]);
            Assert.IsType<Tab3Mock>(tabbedPage.Children[2]);
            Assert.IsType<Tab2Mock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_CreateTabs_WithNavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage-Empty?{KnownNavigationParameters.CreateTab}=NavigationPage|Tab1&{KnownNavigationParameters.CreateTab}=Tab2&{KnownNavigationParameters.CreateTab}=Tab3");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageEmptyMock;
            Assert.NotNull(tabbedPage);
            Assert.Equal(3, tabbedPage.Children.Count());

            var navPage = tabbedPage.Children[0] as NavigationPageMock;
            Assert.IsType<NavigationPageMock>(navPage);
            Assert.IsType<Tab1Mock>(navPage.CurrentPage);
            Assert.IsType<Tab2Mock>(tabbedPage.Children[1]);
            Assert.IsType<Tab3Mock>(tabbedPage.Children[2]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToTabbedPage_CreateTabs_WithNavigationPage_SelectTab()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"TabbedPage-Empty?{KnownNavigationParameters.CreateTab}=Tab1&{KnownNavigationParameters.CreateTab}=NavigationPage|Tab2&{KnownNavigationParameters.CreateTab}=Tab3&{KnownNavigationParameters.SelectedTab}=Tab2");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageEmptyMock;
            Assert.NotNull(tabbedPage);
            Assert.Equal(3, tabbedPage.Children.Count());

            Assert.IsType<Tab1Mock>(tabbedPage.Children[0]);

            var navPage = tabbedPage.Children[1] as NavigationPageMock;
            Assert.IsType<NavigationPageMock>(navPage);
            Assert.IsType<Tab2Mock>(navPage.CurrentPage);
            
            Assert.IsType<Tab3Mock>(tabbedPage.Children[2]);
        }

        #endregion

        #region CarouselPage

        [Fact]
        public async void Navigate_FromContentPage_ToCarouselPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("CarouselPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<CarouselPageMock>(rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToCarouselPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("CarouselPage/ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<CarouselPageMock>(rootPage.Navigation.ModalStack[0]);

            var carouselPage = (CarouselPageMock)rootPage.Navigation.ModalStack[0];

            Assert.True(carouselPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(carouselPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async void Navigate_FromContentPage_ToCarouselPage_SelectedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync($"CarouselPage?{KnownNavigationParameters.SelectedTab}=ContentPage");

            var carouselPage = rootPage.Navigation.ModalStack[0] as CarouselPageMock;
            Assert.NotNull(carouselPage);
            Assert.NotNull(carouselPage.CurrentPage);
            Assert.IsType<ContentPageMock>(carouselPage.CurrentPage);
        }

        #endregion

        #region Remove and Navigate - "../"

        [Fact]
        public async Task RemoveAndNavigate_OneLevel()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../PageMock");

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<PageMock>(rootPage.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async Task RemoveAndNavigate_TwoLevels()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../../PageMock");

            Assert.Equal(3, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<PageMock>(rootPage.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async Task RemoveAndNavigate_ThreeLevels()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../../../PageMock");

            Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<PageMock>(rootPage.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async Task RemoveAndNavigate_FourLevels()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../../../../PageMock");

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<PageMock>(rootPage.Navigation.NavigationStack[0]);
        }

        #endregion

        #region Remove and GoBack - "../"

        [Fact]
        public async Task RemoveAndGoBack_OneLevel()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../");

            Assert.Equal(3, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async Task RemoveAndGoBack_TwoLevels()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../../");

            Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async Task RemoveAndGoBack_ThreeLevels()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            await navigationService.NavigateAsync("../../../");

            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());
        }

        [Fact]
        public async void RemoveAndGoBack_WithNavigationParameters()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.NavigationPage();

            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 1" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 2" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 3" });
            await rootPage.Navigation.PushAsync(new ContentPageMock() { Title = "Page 4" });

            Assert.Equal(4, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack.Last());

            ((IPageAware)navigationService).Page = rootPage.Navigation.NavigationStack.Last();

            var navParameters = new NavigationParameters();
            navParameters.Add("id", 3);

            await navigationService.NavigateAsync("../", navParameters);

            var viewModel = rootPage.Navigation.NavigationStack.Last().BindingContext as ContentPageMockViewModel;
            Assert.NotNull(viewModel);

            Assert.NotNull(viewModel.NavigatedToParameters);
            Assert.True(viewModel.NavigatedToParameters.Count > 0);
            Assert.Equal(3, viewModel.NavigatedToParameters["id"]);
        }

        #endregion


        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
