using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Navigation;

namespace Sandbox.Core
{
    public class NavigationViewModelBase : BindableBase, IPageAware, IConfirmNavigation
    {
        [Dependency]
        public INavigationService NavigationService { get; set; }

        public object Page { get; set; }

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

        protected virtual void GoBack(bool animated = true, bool useModalNavigation = true)
        {
            if (NavigationService != null)
                NavigationService.GoBack(Page, new NavigationParameters(), animated, useModalNavigation);
        }

        protected virtual void GoBack(NavigationParameters parameters, bool animated = true, bool useModalNavigation = true)
        {
            if (NavigationService != null)
                NavigationService.GoBack(Page, parameters, animated, useModalNavigation);
        }

        protected virtual void Navigate(string name, bool useModalNavigation = true)
        {
            if (NavigationService != null)
                Navigate(name, new NavigationParameters(), useModalNavigation);
        }

        protected virtual void Navigate(string name, NavigationParameters parameters, bool useModalNavigation = true)
        {
            if (NavigationService != null)
                NavigationService.Navigate(Page, name, parameters, useModalNavigation);
        }
    }
}
