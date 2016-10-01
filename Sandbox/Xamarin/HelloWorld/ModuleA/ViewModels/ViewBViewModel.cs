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

        private bool _canNavigate = true;
        public bool CanNavigate
        {
            get { return _canNavigate; }
            set { SetProperty(ref _canNavigate, value); }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public ViewBViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand(Navigate).ObservesCanExecute((vm) => CanNavigate);
        }

        async void Navigate()
        {
            CanNavigate = false;
            await _navigationService.GoBackAsync();
            CanNavigate = true;
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
