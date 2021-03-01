using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class XamlMasterDetailViewMockViewModel
    {
        public INavigationService NavigationService { get; }

        public XamlMasterDetailViewMockViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
    }
}
