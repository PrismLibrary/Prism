using HelloWorld.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Modularity;
using System;
using Prism.Services;
using Prism.Navigation;

namespace HelloWorld.ViewModels
{
    public class SomeOtherViewModel : ViewModelBase
    {
        IPageDialogService _dialogService;

        string _title = "Some Other ViewModel that doesn't follow the ViewModelLocatorRules";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand LoadModuleACommand { get; set; }

        public SomeOtherViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;
            LoadModuleACommand = new DelegateCommand(ShowDialog);
        }

        void ShowDialog()
        {
            _dialogService.DisplayAlertAsync("Hello from SomeOtherViewModel", "This is a message from an exception to the ViewModelLocator rules.", "Cool");
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
        }
    }
}
