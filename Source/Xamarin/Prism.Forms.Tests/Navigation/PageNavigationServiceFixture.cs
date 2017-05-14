﻿using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Logging;
using Prism.Navigation;
using System;
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
            _container = new PageNavigationContainerMock();

            _container.Register("PageMock", typeof(PageMock));

            _container.Register("ContentPage", typeof(ContentPageMock));
            _container.Register(typeof(ContentPageMockViewModel).FullName, typeof(ContentPageMock));

            _container.Register("NavigationPage", typeof(NavigationPageMock));
            _container.Register("NavigationPage-Empty", typeof(NavigationPageEmptyMock));
            _container.Register("NavigationPageWithStack", typeof(NavigationPageWithStackMock));
            _container.Register("NavigationPageWithStackNoMatch", typeof(NavigationPageWithStackNoMatchMock));

            _container.Register("MasterDetailPage", typeof(MasterDetailPageMock));
            _container.Register("MasterDetailPage-Empty", typeof(MasterDetailPageEmptyMock));


            _container.Register("TabbedPage", typeof(TabbedPageMock));
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
                var rootPage = new Xamarin.Forms.ContentPage();
                ((IPageAware)navigationService).Page = rootPage;

                await navigationService.NavigateAsync("UnregisteredPage");

                Assert.True(rootPage.Navigation.ModalStack.Count == 0);
            });
        }

        /// <summary>
        /// It is a test of navigation to RootPage. The Type of Page does not matter.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_To_RootPage()
        {
            var applicationProvider = new ApplicationProviderMock(null);
            var recorder = new PageNavigationEventRecorder();
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade, recorder);

            await navigationService.NavigateAsync("ContentPage");

            var mainPage = applicationProvider.MainPage;

            Assert.IsType<ContentPageMock>(mainPage);
            Assert.Equal(0, mainPage.Navigation.ModalStack.Count);
            Assert.Equal(0, mainPage.Navigation.NavigationStack.Count);

            var record = recorder.TakeFirst();
            Assert.Equal(mainPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(mainPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatingTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(mainPage, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            record = recorder.TakeFirst();
            Assert.Equal(mainPage.BindingContext, record.Sender);
            Assert.Equal(PageNavigationEvent.OnNavigatedTo, record.Event);

            Assert.True(recorder.IsEmpty);
        }

        /// <summary>
        /// It is a test of navigation to RootPage. The Type of Page does not matter.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_To_RootPage_ByAbsoluteName()
        {
            var applicationProvider = new ApplicationProviderMock(null);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade);

            await navigationService.NavigateAsync("/ContentPage");

            var mainPage = applicationProvider.MainPage;

            Assert.IsType<ContentPageMock>(mainPage);
            Assert.Equal(0, mainPage.Navigation.ModalStack.Count);
            Assert.Equal(0, mainPage.Navigation.NavigationStack.Count);
        }

        /// <summary>
        /// It is a test of navigation to RootPage. The Type of Page does not matter.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_To_RootPage_ByRelativeUri()
        {
            var applicationProvider = new ApplicationProviderMock(null);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade);

            await navigationService.NavigateAsync(new Uri("ContentPage", UriKind.Relative));

            var mainPage = applicationProvider.MainPage;

            Assert.IsType<ContentPageMock>(mainPage);
            Assert.Equal(0, mainPage.Navigation.ModalStack.Count);
            Assert.Equal(0, mainPage.Navigation.NavigationStack.Count);
        }
        /// <summary>
        /// It is a test of navigation to RootPage. The Type of Page does not matter.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_To_RootPage_ByAbsoluteUri()
        {
            var applicationProvider = new ApplicationProviderMock(null);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade);

            await navigationService.NavigateAsync(new Uri("http://localhost/ContentPage", UriKind.Absolute));

            Assert.IsType<ContentPageMock>(applicationProvider.MainPage);
            Assert.Equal(0, applicationProvider.MainPage.Navigation.ModalStack.Count);
            Assert.Equal(0, applicationProvider.MainPage.Navigation.NavigationStack.Count);
        }

        [Fact]
        public async Task NavigateAsync_Replace_RootPage_ByAbsoluteName()
        {
            var recorder = new PageNavigationEventRecorder(); ;
            var rootPage = new ContentPageMock(recorder);
            var rootPageViewModel = (ViewModelBase)rootPage.BindingContext;
            var applicationProvider = new ApplicationProviderMock(rootPage);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade, recorder);

            await navigationService.NavigateAsync("/ContentPage");

            var navigatedPage = applicationProvider.MainPage;
            Assert.IsType(typeof(ContentPageMock), navigatedPage);
            Assert.NotEqual(rootPage, navigatedPage);

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
        public async Task NavigateAsync_Replace_RootPage_ByAbsoluteUri()
        {
            var recorder = new PageNavigationEventRecorder();
            var rootPage = new ContentPageMock(recorder);
            var rootPageViewModel = (ViewModelBase)rootPage.BindingContext;
            var applicationProvider = new ApplicationProviderMock(rootPage);
            var navigationService = new PageNavigationServiceMock(_container, applicationProvider, _loggerFacade, recorder);

            await navigationService.NavigateAsync(new Uri("http://localhost/ContentPage"));

            var navigatedPage = applicationProvider.MainPage;
            Assert.IsType(typeof(ContentPageMock), navigatedPage);
            Assert.NotEqual(rootPage, navigatedPage);

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

        /// <summary>
        /// This test case does not depend on the Page of the transition destination. Therefore, the name is OtherPage.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_From_RootPage_ToOtherPage_ByRelativeName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(0, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        /// <summary>
        /// This test case does not depend on the Page of the transition destination. Therefore, the name is OtherPage.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_From_RootPage_ToOtherPage_ByRelativeUri()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync(new Uri("ContentPage", UriKind.Relative));

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(0, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        /// <summary>
        /// This test case does not depend on the Page of the transition destination. Therefore, the name is OtherPage.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NavigateAsync_From_NavigationPage_ToOtherPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.Equal(0, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.NavigationStack[0]);
        }


        [Fact]
        public async Task NavigateAsync_From_NavigationPage_ToOtherPage_ByModal()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            var navigationPage = new NavigationPage(rootPage);
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage", useModalNavigation: true);

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(1, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPage>(rootPage.Navigation.NavigationStack[0]);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async Task NavigateAsync_From_NavigationPage_ToOtherPage_ByNotModal()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new ContentPage();
            var navigationPage = new NavigationPage(rootPage);
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage", useModalNavigation: false);

            Assert.Equal(0, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
            Assert.IsType<ContentPage>(rootPage.Navigation.NavigationStack[0]);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.NavigationStack[1]);
        }

        [Fact]
        public async Task NavigateAsync_From_NavigationPage_PopToRoot()
        {
            var recorder = new PageNavigationEventRecorder(); ;
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
            var rootPage = new ContentPageMock();
            var navigationPage = new NavigationPage(rootPage);

            ((IPageAware) navigationService).Page = rootPage;
            await navigationService.NavigateAsync("ContentPage");

            ((IPageAware)navigationService).Page = navigationPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
            Assert.Equal(1, navigationPage.Navigation.NavigationStack.Count);
            Assert.Equal(rootPage, navigationPage.Navigation.NavigationStack[0]);
        }

        [Fact]
        public async Task NavigateAsync_From_NavigationPage_PopToRoot_And_ReplaceRootPage()
        {
            var recorder = new PageNavigationEventRecorder(); ;
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade, recorder);
            var rootPage = new ContentPage();
            var navigationPage = new NavigationPage(rootPage);

            ((IPageAware)navigationService).Page = rootPage;
            await navigationService.NavigateAsync("ContentPage");

            ((IPageAware)navigationService).Page = navigationPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.Equal(0, navigationPage.Navigation.ModalStack.Count);
            Assert.Equal(1, navigationPage.Navigation.NavigationStack.Count);
            Assert.NotEqual(rootPage, navigationPage.Navigation.NavigationStack[0]);
        }

        [Fact]
        public async void Navigate_ToContentPage_ByName_WithNavigationParameters()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            var navParameters = new NavigationParameters();
            navParameters.Add("id", 3);

            await navigationService.NavigateAsync("ContentPage", navParameters);

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);

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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);

            await navigationService.GoBackAsync();

            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public async void NavigateAsync_ToContentPage_ThenGoBack()
        {
            var pageMock = new ContentPageMock();
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            ((IPageAware)navigationService).Page = pageMock;

            var rootPage = new NavigationPage(pageMock);

            Assert.IsType(typeof(ContentPageMock), rootPage.CurrentPage);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);
            Assert.IsType(typeof(TabbedPageMock), rootPage.CurrentPage);
            var tabbedPageMock = rootPage.CurrentPage as TabbedPageMock;
            Assert.NotNull(tabbedPageMock);
            var viewModel = (ViewModelBase)tabbedPageMock.BindingContext;
            
            await navigationService.GoBackAsync();

            Assert.True(rootPage.Navigation.NavigationStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.CurrentPage);
            Assert.True(tabbedPageMock.DestroyCalled);
            Assert.Null(tabbedPageMock.BindingContext);
            Assert.True(viewModel.DestroyCalled);
        }

        [Fact]
        public async void Navigate_ToContentPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);

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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage() { BindingContext = new ContentPageMockViewModel() };
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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("NavigationPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(NavigationPageMock), rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as NavigationPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public async void Navigate_ToMasterDetailPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
        public async void Navigate_FromMasterDetailPage_ToSamePage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.IsType(typeof(ContentPageMock), rootPage.Detail);

            await navigationService.NavigateAsync("TabbedPage");

            var firstDetailPage = rootPage.Detail;

            Assert.IsType(typeof(TabbedPageMock), firstDetailPage);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.Equal(firstDetailPage, rootPage.Detail);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.IsType(typeof(ContentPageMock), rootPage.Detail);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.IsType(typeof(TabbedPageMock), rootPage.Detail);

            await navigationService.NavigateAsync("CarouselPage");

            Assert.IsType(typeof(CarouselPageMock), rootPage.Detail);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToTabbedPage_IsPresented()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;
            rootPage.IsPresentedAfterNavigation = true;

            Assert.IsType(typeof(ContentPageMock), rootPage.Detail);
            Assert.False(rootPage.IsPresented);

            await navigationService.NavigateAsync("TabbedPage");
            Assert.IsType(typeof(TabbedPageMock), rootPage.Detail);

            Assert.True(rootPage.IsPresented);
        }

        [Fact]
        public async void Navigate_FromMasterDetailPage_ToTabbedPage_IsNotPresented()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new MasterDetailPageMock();
            ((IPageAware)navigationService).Page = rootPage;
            rootPage.IsPresentedAfterNavigation = false;

            Assert.IsType(typeof(ContentPageMock), rootPage.Detail);
            Assert.False(rootPage.IsPresented);

            await navigationService.NavigateAsync("TabbedPage");
            Assert.IsType(typeof(TabbedPageMock), rootPage.Detail);

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
            Assert.IsType(typeof(TabbedPageMock), rootPage.Detail);

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
            Assert.IsType(typeof(TabbedPageMock), rootPage.Detail);

            Assert.False(rootPage.IsPresented);
        }

        #region UnitTest of DeepLink
        [Fact]
        public async void DeepNavigate_From_ContentPage_To_ContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPage_ToContentPage_ByAbsoluteName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPage-Empty/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPageWithNavigationStack_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStack/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPageWithDifferentNavigationStack_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStackNoMatch/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_TabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/TabbedPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_CarouselPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/CarouselPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_MasterDetailPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/MasterDetailPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage");

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(0, rootPage.Navigation.NavigationStack.Count);

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType(typeof(ContentPageMock), masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToContentPage_UseModalNavigation()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage", useModalNavigation: true);

            Assert.Equal(1, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(0, rootPage.Navigation.NavigationStack.Count);
            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType(typeof(ContentPageMock), masterDetail.Detail);
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

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage", useModalNavigation: false);

            Assert.Equal(0, rootPage.Navigation.ModalStack.Count);
            Assert.Equal(2, rootPage.Navigation.NavigationStack.Count);
            var masterDetail = rootPage.Navigation.NavigationStack[1] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType(typeof(ContentPageMock), masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToEmptyMasterDetailPage_ToNavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/NavigationPage");

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType(typeof(NavigationPageMock), masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToDifferentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage/TabbedPage");

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType(typeof(TabbedPageMock), masterDetail.Detail);
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToSamePage_ToTabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage/ContentPage/TabbedPage");

            var masterDetail = rootPage.Navigation.ModalStack[0] as MasterDetailPageMock;
            Assert.NotNull(masterDetail);
            Assert.NotNull(masterDetail.Detail);
            Assert.IsType(typeof(ContentPageMock), masterDetail.Detail);

            var tabbedPage = masterDetail.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
        }


        [Fact]
        public async void DeepNavigate_ToTabbedPage_ToPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("TabbedPage/PageMock");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType(typeof(PageMock), tabbedPage.CurrentPage);
        }

        [Fact]
        public async void DeepNavigate_ToCarouselPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("CarouselPage/ContentPage");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as CarouselPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType(typeof(ContentPageMock), tabbedPage.CurrentPage);
        }

        #endregion 

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
