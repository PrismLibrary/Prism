using Microsoft.Practices.ServiceLocation;
using Prism.Navigation;

namespace Prism.Mvvm
{
    public class NavigationViewModelBase : BindableBase, IPageAware, IConfirmNavigation
    {
        private INavigationService _navigationService;
        private INavigationService NavigationService
        {
            get { return _navigationService ?? (_navigationService = ServiceLocator.Current.GetInstance<INavigationService>()); }
        }

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
            NavigationService.GoBack(Page, new NavigationParameters(), animated, useModalNavigation);
        }

        protected virtual void GoBack(NavigationParameters parameters, bool animated = true, bool useModalNavigation = true)
        {
            NavigationService.GoBack(Page, parameters, animated, useModalNavigation);
        }

        protected virtual void Navigate(string name, bool useModalNavigation = true)
        {
            Navigate(name, new NavigationParameters(), useModalNavigation);
        }

        protected virtual void Navigate(string name, NavigationParameters parameters, bool useModalNavigation = true)
        {
            NavigationService.Navigate(Page, name, parameters, useModalNavigation);
        }
    }
}
