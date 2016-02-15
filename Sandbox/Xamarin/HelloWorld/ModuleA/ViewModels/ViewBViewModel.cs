using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;

namespace ModuleA.ViewModels
{
    public class ViewBViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;

        string _title = "View B";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public ViewBViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new DelegateCommand(Navigate);
        }

        void Navigate()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            Debug.WriteLine("Navigated to ViewB");
        }
    }
}
