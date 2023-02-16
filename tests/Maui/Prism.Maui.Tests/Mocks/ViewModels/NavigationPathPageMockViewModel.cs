using Prism.Navigation;

namespace Prism.Maui.Tests.Navigation.Mocks.ViewModels
{
    public class NavigationPathPageMockViewModel
    {
        public INavigationService NavigationService { get; }

        public NavigationPathPageMockViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
    }
}
