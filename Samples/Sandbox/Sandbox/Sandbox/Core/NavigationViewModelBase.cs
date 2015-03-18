using Prism.Mvvm;
using Prism.Navigation;

namespace Sandbox.Core
{
    public class NavigationViewModelBase : BindableBase, IConfirmNavigation
    {
        public INavigationService NavigationService { get; set; }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public virtual bool CanNavigate(NavigationParameters parameters)
        {
            return true;
        }
    }
}
