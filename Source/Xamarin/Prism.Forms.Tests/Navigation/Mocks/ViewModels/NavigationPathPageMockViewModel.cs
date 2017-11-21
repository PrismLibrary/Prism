using Prism.Navigation;

namespace Prism.Forms.Tests.Navigation.Mocks.ViewModels
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
