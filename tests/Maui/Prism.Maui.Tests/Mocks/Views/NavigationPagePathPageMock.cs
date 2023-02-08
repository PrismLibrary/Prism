using Microsoft.Maui.Controls;
using Prism.Maui.Tests.Navigation.Mocks.ViewModels;

namespace Prism.Maui.Tests.Navigation.Mocks.Views;

public class NavigationPathPageMock : ContentPage
{
    public NavigationPathPageMockViewModel ViewModel { get; }
    public NavigationPathPageMock()
    {
        //var navService = new PageNavigationServiceMock(null, new ApplicationMock(), null);
        //((IPageAware)navService).Page = this;

        //BindingContext = ViewModel = new NavigationPathPageMockViewModel(navService);
    }
}
