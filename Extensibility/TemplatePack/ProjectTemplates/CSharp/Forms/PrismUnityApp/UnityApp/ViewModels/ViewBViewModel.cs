using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace $safeprojectname$.ViewModels
{
    public class ViewBViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navigationService;
        private IPageDialogService _pageDialogService;

        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value); }
        }

        public DelegateCommand DisplayAlertCommand { get; set; }

        public DelegateCommand NavigateCommand { get; set; }

        public ViewBViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

            NavigateCommand = new DelegateCommand(GoBack);

            DisplayAlertCommand = new DelegateCommand(DisplayAlert);
        }

        private void DisplayAlert()
        {
            _pageDialogService.DisplayAlert("Hello", "Hello from Prism", "Close");
        }

        private void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("timestamp"))
                TimeStamp = (DateTime)parameters["timestamp"];
        }
    }
}
