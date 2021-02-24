using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class XamlTabbedViewMockViewModel : BindableBase
    {
        public INavigationService NavigationService { get; }

        public XamlTabbedViewMockViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
    }
}
