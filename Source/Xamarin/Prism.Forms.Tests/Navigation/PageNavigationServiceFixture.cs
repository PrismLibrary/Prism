using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Navigation;
using System;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigationServiceFixture : IDisposable
    {
        PageNavigationContainerMock _container;

        public PageNavigationServiceFixture()
        {
            _container = new PageNavigationContainerMock();
            _container.Register("ContentPage", typeof(ContentPageMock));
            _container.Register(typeof(ContentPageMockViewModel).FullName, typeof(ContentPageMock));

            _container.Register("NavigationPage", typeof(NavigationPageMock));
            _container.Register("NavigationPage-Empty", typeof(NavigationPageEmptyMock));
            _container.Register("NavigationPageWithStack", typeof(NavigationPageWithStackMock));
            _container.Register("NavigationPageWithStackNoMatch", typeof(NavigationPageWithStackNoMatchMock));

            _container.Register("MasterDetailPage", typeof(MasterDetailPageMock));
            _container.Register("TabbedPage", typeof(TabbedPageMock));
            _container.Register("CarouselPage", typeof(CarouselPageMock));
        }

        [Fact]
        public void IPageAware_NullByDefault()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var page = ((IPageAware)navigationService).Page;
            Assert.Null(page);
        }

        [Fact]
        public void Navigate_ToUnregisteredPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("UnregisteredPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public void Navigate_ToContentPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void NavigateAsync_ToContentPage_ByName()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.NavigationPage();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage", useModalNavigation: false);

            Assert.True(rootPage.Navigation.NavigationStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.NavigationStack[0]);
        }

        [Fact]
        public void Navigate_ToContentPage_ByRelativeUri()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate(new Uri("ContentPage", UriKind.Relative));

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void Navigate_ToContentPage_ByAbsoluteUri()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate(new Uri("http://brianlagunas.com/ContentPage", UriKind.Absolute));

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void Navigate_ToContentPage_ByObject()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate<ContentPageMockViewModel>();

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void Navigate_ToContentPage_ByName_WithNavigationParameters()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            var navParameters = new NavigationParameters();
            navParameters.Add("id", 3);

            navigationService.Navigate("ContentPage", navParameters);

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as ContentPageMockViewModel;
            Assert.NotNull(viewModel);

            Assert.NotNull(viewModel.NavigatedToParameters);
            Assert.True(viewModel.NavigatedToParameters.Count > 0);
            Assert.Equal(3, viewModel.NavigatedToParameters["id"]);
        }

        [Fact]
        public void Navigate_ToContentPage_ThenGoBack()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);

            navigationService.GoBack();

            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public void NavigateAsync_ToContentPage_ThenGoBack()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new NavigationPage(new PageMock());
            ((IPageAware)navigationService).Page = rootPage;

            Assert.IsType(typeof(PageMock), rootPage.CurrentPage);

            navigationService.Navigate("ContentPage", useModalNavigation: false);

            Assert.True(rootPage.Navigation.NavigationStack.Count == 2);
            Assert.IsType(typeof(ContentPageMock), rootPage.CurrentPage);

            navigationService.GoBack(useModalNavigation: false);

            Assert.True(rootPage.Navigation.NavigationStack.Count == 1);
            Assert.IsType(typeof(PageMock), rootPage.CurrentPage);
        }

        [Fact]
        public void Navigate_ToContentPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as ContentPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);

            var nextPageNavService = new PageNavigationServiceMock(_container);
            ((IPageAware)navigationService).Page = rootPage.Navigation.ModalStack[0];
            navigationService.Navigate("NavigationPage");

            Assert.True(viewModel.OnNavigatedFromCalled);
        }

        [Fact]
        public void Navigate_ToContentPage_PageHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var contentPage = rootPage.Navigation.ModalStack[0] as ContentPageMock;
            Assert.NotNull(contentPage);
            Assert.True(contentPage.OnNavigatedToCalled);

            var nextPageNavService = new PageNavigationServiceMock(_container);
            ((IPageAware)navigationService).Page = contentPage;

            navigationService.Navigate("NavigationPage");

            Assert.True(contentPage.OnNavigatedFromCalled);
            Assert.True(contentPage.Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public void Navigate_ToContentPage_PageHasIConfirmNavigation_True()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new ContentPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.False(rootPage.OnConfirmNavigationCalled);

            navigationService.Navigate("ContentPage");

            Assert.True(rootPage.OnConfirmNavigationCalled);
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public void Navigate_ToContentPage_PageHasIConfirmNavigation_False()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new ContentPageMock();
            ((IPageAware)navigationService).Page = rootPage;

            Assert.False(rootPage.OnConfirmNavigationCalled);

            var navParams = new NavigationParameters();
            navParams.Add("canNavigate", false);

            navigationService.Navigate("ContentPage", navParams);

            Assert.True(rootPage.OnConfirmNavigationCalled);
            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public void Navigate_ToContentPage_ViewModelHasIConfirmNavigation_True()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Page() { BindingContext = new ContentPageMockViewModel() };
            ((IPageAware)navigationService).Page = rootPage;

            var viewModel = rootPage.BindingContext as ContentPageMockViewModel;
            Assert.False(viewModel.OnConfirmNavigationCalled);

            navigationService.Navigate("ContentPage");
            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnConfirmNavigationCalled);
        }

        [Fact]
        public void Navigate_ToContentPage_ViewModelHasIConfirmNavigation_False()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Page() { BindingContext = new ContentPageMockViewModel() };
            ((IPageAware)navigationService).Page = rootPage;

            var viewModel = rootPage.BindingContext as ContentPageMockViewModel;
            Assert.False(viewModel.OnConfirmNavigationCalled);

            var navParams = new NavigationParameters();
            navParams.Add("canNavigate", false);

            navigationService.Navigate("ContentPage", navParams);

            Assert.True(viewModel.OnConfirmNavigationCalled);
            Assert.True(rootPage.Navigation.ModalStack.Count == 0);
        }

        [Fact]
        public void Navigate_ToNavigatonPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("NavigationPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(NavigationPageMock), rootPage.Navigation.ModalStack[0]);

            var viewModel = rootPage.Navigation.ModalStack[0].BindingContext as NavigationPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);

            var navPage = rootPage.Navigation.ModalStack[0] as NavigationPage;
            Assert.NotNull(navPage.CurrentPage);

            var childViewModel = navPage.CurrentPage.BindingContext as ContentPageMockViewModel;
            Assert.NotNull(childViewModel);
            Assert.True(childViewModel.OnNavigatedToCalled);
        }

        [Fact]
        public void Navigate_ToMasterDetailPage_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("MasterDetailPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var mdPage = rootPage.Navigation.ModalStack[0] as MasterDetailPage;
            Assert.NotNull(mdPage);

            var viewModel = mdPage.BindingContext as MasterDetailPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);

            Assert.NotNull(mdPage.Detail);

            Assert.IsType(typeof(ContentPageMock), mdPage.Detail);
            var childViewModel = mdPage.Detail.BindingContext as ContentPageMockViewModel;
            Assert.NotNull(childViewModel);
            Assert.True(childViewModel.OnNavigatedToCalled);
        }

        [Fact]
        public void Navigate_ToTabbedPage_ByName_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("TabbedPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var mdPage = rootPage.Navigation.ModalStack[0] as TabbedPageMock;
            Assert.NotNull(mdPage);

            var viewModel = mdPage.BindingContext as TabbedPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public void Navigate_ToCarouselPage_ByName_ViewModelHasINavigationAware()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("CarouselPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);

            var cPage = rootPage.Navigation.ModalStack[0] as CarouselPageMock;
            Assert.NotNull(cPage);

            var viewModel = cPage.BindingContext as CarouselPageMockViewModel;
            Assert.NotNull(viewModel);
            Assert.True(viewModel.OnNavigatedToCalled);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_ContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/ContentPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_NavigationPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/NavigationPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_NavigationPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/NavigationPage/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_EmptyNavigationPage_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/NavigationPage-Empty/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_NavigationPageWithNavigationStack_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/NavigationPageWithStack/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_NavigationPageWithDifferentNavigationStack_ToContentPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/NavigationPageWithStackNoMatch/ContentPage");

            var navPage = rootPage.Navigation.ModalStack[0].Navigation.ModalStack[0];
            Assert.True(navPage.Navigation.NavigationStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_TabbedPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/TabbedPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_CarouselPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/CarouselPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        [Fact]
        public void DeepNavigate_From_ContentPage_To_MasterDetailPage()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate("ContentPage/MasterDetailPage");

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.True(rootPage.Navigation.ModalStack[0].Navigation.ModalStack.Count == 1);
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
