using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Logging;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    [Collection("Navigation")]
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

        [Fact]
        public async void Navigate_ToContentPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public async void Navigate_ToContentPage_ByRelativeUri()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType<ContentPageMock>(rootPage.Navigation.ModalStack[0]);

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

            Assert.IsType<ContentPageMock>(rootPage.CurrentPage);

            await navigationService.NavigateAsync("TabbedPage");

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);
            Assert.IsType<TabbedPageMock>(rootPage.CurrentPage);
            var tabbedPageMock = rootPage.CurrentPage as TabbedPageMock;
            Assert.NotNull(tabbedPageMock);
            var viewModel = (ViewModelBase)tabbedPageMock.BindingContext;
            
            await navigationService.GoBackAsync();

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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            Assert.IsType<NavigationPageMock>(rootPage.Navigation.ModalStack[0]);

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
        public async void DeepNavigate_From_ContentPage_To_EmptyNavigationPage_ToContentPage_toContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStack/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public async void DeepNavigate_From_ContentPage_To_NavigationPageWithNavigationStack_ToContentPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("ContentPage/NavigationPageWithStackNoMatch/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.Equal(1, navPage.Navigation.NavigationStack.Count);
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
        public async void DeepNavigate_ToCarouselPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("CarouselPage/ContentPage");

            var tabbedPage = rootPage.Navigation.ModalStack[0] as CarouselPageMock;
            Assert.NotNull(tabbedPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<ContentPageMock>(tabbedPage.CurrentPage);
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
