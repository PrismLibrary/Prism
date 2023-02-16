using Prism.Maui.Tests.Navigation.Mocks.ViewModels;
using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Navigation.Mocks.Views;

public class NavigationPathTabbedPageMock : TabbedPage
{
    public NavigationPathPageMockViewModel ViewModel { get; }

    public NavigationPathTabbedPageMock()
    {
        //var navService = new PageNavigationServiceMock(null, new ApplicationMock(), null);
        //((IPageAware)navService).Page = this;

        //BindingContext = ViewModel = new NavigationPathPageMockViewModel(navService);

        Children.Add(new NavigationPathPageMock());
        Children.Add(new NavigationPathPageMock2());
        Children.Add(new NavigationPathPageMock3());
    }
}
