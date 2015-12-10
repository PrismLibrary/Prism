using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace $safeprojectname$.ViewModels
{
    public class ViewAViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navigationService;

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private bool _allowNavigate = false;
        public bool AllowNavigate
        {
            get { return _allowNavigate; }
            set { SetProperty(ref _allowNavigate, value); }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public ViewAViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new DelegateCommand(Navigate).ObservesCanExecute(vm => AllowNavigate);
        }

        private void Navigate()
        {
            var navParams = new NavigationParameters();
            navParams.Add("timestamp", DateTime.Now);

            _navigationService.Navigate("ViewB", navParams);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("message"))
                Message = (string)parameters["message"] + "and Prism";
        }
    }
}
