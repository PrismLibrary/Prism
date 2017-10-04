using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    [Collection("Navigation")]
    public class PageNavigationServiceFixture_TabbedPage : IDisposable
    {
        PageNavigationContainerMock _container;
        IApplicationProvider _applicationProvider;
        ILoggerFacade _loggerFacade;

        public PageNavigationServiceFixture_TabbedPage()
        {
            _container = new PageNavigationContainerMock();

            _container.Register("TabbedPage", typeof(TabbedPageMock));

            _container.Register("PageMock", typeof(PageMock));

            _applicationProvider = new ApplicationProviderMock();
            _loggerFacade = new EmptyLogger();
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
            Assert.IsType<PageMock>(tabbedPage.CurrentPage);
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
