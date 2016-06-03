using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.DryIoc.Forms.Tests.Mocks.ViewModels
{
    public class AutowireViewModel : BindableBase
    {
        public INavigationService NavigationService { get; }

        public AutowireViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
    }
}