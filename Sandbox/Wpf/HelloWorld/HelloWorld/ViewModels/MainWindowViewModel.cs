using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace HelloWorld.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        private readonly IDialogService _dialogService;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand ShowDialogCommand { get; private set; }

        public MainWindowViewModel(IDialogService dialogService)
        {
            ShowDialogCommand = new DelegateCommand(ShowDialog);
            _dialogService = dialogService;
        }

        private void ShowDialog()
        {
            _dialogService.ShowNotification("This is a title", "This is a message!", r => Title = "CallBack");
        }
    }
}
