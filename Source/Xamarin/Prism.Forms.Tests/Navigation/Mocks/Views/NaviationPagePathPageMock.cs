using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Navigation.Mocks.ViewModels;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Navigation.Mocks.Views
{
    public class NavigationPathPageMock : ContentPage
    {
        public NavigationPathPageMockViewModel ViewModel { get; }
        public NavigationPathPageMock()
        {
            var navService = new PageNavigationServiceMock(null, new ApplicationProviderMock(), null, null);
            ((IPageAware)navService).Page = this;

            BindingContext = ViewModel = new NavigationPathPageMockViewModel(navService);
        }
    }

    public class NavigationPathPageMock2 : NavigationPathPageMock
    {
        public NavigationPathPageMock2()
        {

        }
    }

    public class NavigationPathPageMock3 : NavigationPathPageMock
    {
        public NavigationPathPageMock3()
        {

        }
    }

    public class NavigationPathPageMock4 : NavigationPathPageMock
    {
        public NavigationPathPageMock4()
        {

        }
    }

    public class NavigationPathTabbedPageMock : TabbedPage
    {
        public NavigationPathPageMockViewModel ViewModel { get; }

        public NavigationPathTabbedPageMock()
        {
            var navService = new PageNavigationServiceMock(null, new ApplicationProviderMock(), null, null);
            ((IPageAware)navService).Page = this;

            BindingContext = ViewModel = new NavigationPathPageMockViewModel(navService);

            Children.Add(new NavigationPathPageMock());
            Children.Add(new NavigationPathPageMock2());
            Children.Add(new NavigationPathPageMock3());
        }
    }
}
