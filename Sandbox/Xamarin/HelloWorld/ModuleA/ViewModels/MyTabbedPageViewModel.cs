using Prism.Mvvm;
using Prism.Navigation;
using Prism.AppModel;
using System.Diagnostics;

namespace ModuleA.ViewModels
{
    public class MyTabbedPageViewModel : BindableBase, INavigationAware, IPageLifecycleAware
    {
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

        public MyTabbedPageViewModel(IApplicationCommands applicationCommands)
        {
            _applicationCommands = applicationCommands;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public void OnAppearing()
        {
            Debug.WriteLine("MyTabbedPage is appearing");
        }

        public void OnDisappearing()
        {
            Debug.WriteLine("MyTabbedPage is disappearing");
        }
    }
}
