using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Logging;
using System;
using System.Linq;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class PageNavigationServiceFixture_MasterDetailPage : IDisposable
    {
        PageNavigationContainerMock _container;
        IApplicationProvider _applicationProvider;
        ILoggerFacade _loggerFacade;

        public PageNavigationServiceFixture_MasterDetailPage()
        {
            _container = new PageNavigationContainerMock();

            _container.Register("MasterDetailPage", typeof(MasterDetailPageMock));
            _container.Register("MasterDetailPage-Empty", typeof(MasterDetailPageEmptyMock));

            _container.Register("NavigationPage", typeof(NavigationPageMock));
            _container.Register("NavigationPage-Empty", typeof(NavigationPageEmptyMock));

            _container.Register("ContentPage", typeof(ContentPageMock));
            _container.Register("TabbedPage", typeof(TabbedPageMock));
            _container.Register("CarouselPage", typeof(CarouselPageMock));

            _container.Register("PageMock", typeof(PageMock));

            _applicationProvider = new ApplicationProviderMock();
            _loggerFacade = new EmptyLogger();
        }

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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage", useModalNavigation: true);

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

            await navigationService.NavigateAsync("MasterDetailPage-Empty/ContentPage", useModalNavigation: false);

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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
            var rootPage = new Xamarin.Forms.ContentPage();
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
        public async void DeepNavigate_ToMasterDetailPage_ToNavigationPage_ToTabbedPage_ToPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/NavigationPage/TabbedPage/PageMock");

            var mdpPage = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            var navPage = mdpPage.Detail as NavigationPageMock;
            var tabbedPage = navPage.Navigation.NavigationStack[0] as TabbedPageMock;
            Assert.NotNull(mdpPage);
            Assert.NotNull(navPage);
            Assert.NotNull(tabbedPage.CurrentPage);
            Assert.IsType<PageMock>(tabbedPage.CurrentPage);
        }

        [Fact]
        public async void DeepNavigate_ToMasterDetailPage_ToNavigationPage_ToContentPage_ToTabbedPage_ToPage()
        {
            var navigationService = new PageNavigationServiceMock(_container, _applicationProvider, _loggerFacade);
            var rootPage = new Xamarin.Forms.ContentPage();
            ((IPageAware)navigationService).Page = rootPage;

            await navigationService.NavigateAsync("MasterDetailPage-Empty/NavigationPage/ContentPage/TabbedPage/PageMock");

            var mdpPage = rootPage.Navigation.ModalStack[0] as MasterDetailPageEmptyMock;
            var navPage = mdpPage.Detail as NavigationPageMock;
            var contentPage = navPage.Navigation.NavigationStack[0] as ContentPageMock;
            var tabbedPage = navPage.Navigation.NavigationStack[1] as TabbedPageMock;
            Assert.NotNull(mdpPage);
            Assert.NotNull(navPage);
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
