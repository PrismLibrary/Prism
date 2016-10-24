using System;
using Prism.Mvvm;
using Prism.Navigation;

namespace HelloWorld.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware
    {
        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
