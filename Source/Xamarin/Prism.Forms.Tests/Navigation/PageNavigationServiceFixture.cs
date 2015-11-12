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
        }

        [Fact]
        public void IPageAware_NullByDefault()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var page = ((IPageAware)navigationService).Page;
            Assert.Null(page);
        }

        [Fact]
        public void NavigateModal_ToContentPage_ByName()
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
        public void NavigateModal_ToContentPage_ByRelativeUri()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate(new Uri("ContentPage", UriKind.Relative));

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void NavigateModal_ToContentPage_ByAbsoluteUri()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate(new Uri("http://brianlagunas.com/ContentPage", UriKind.Absolute));

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void NavigateModal_ToContentPage_ByObject()
        {
            var navigationService = new PageNavigationServiceMock(_container);
            var rootPage = new Xamarin.Forms.Page();
            ((IPageAware)navigationService).Page = rootPage;

            navigationService.Navigate<ContentPageMockViewModel>();

            Assert.True(rootPage.Navigation.ModalStack.Count == 1);
            Assert.IsType(typeof(ContentPageMock), rootPage.Navigation.ModalStack[0]);
        }

        [Fact]
        public void NavigateModal_ToContentPage_ByName_WithNavigationParameters()
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
        public void NavigateModal_ToContentPage_INavigationAware()
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
        public void NavigateModal_ToNavigatonPage_ByName_INavigationAware()
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

        //TODO: tests need to do
        //navigated from
        //page implements INavigationAware
        //IConfirmNavigation
        //master detail
        //tabbed
        //deep linking
        //PageNavigationOptions

        [Fact]
        public void NavigateModal_ToContentPage_ThenGoBack()
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

        public void Dispose()
        {
            _container = null;
        }
    }
}
