using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloDialog.ViewModels
{
    public class UserAlertViewModel : BindableBase, IDialogAware
    {
        public UserAlertViewModel()
        {
            SubmitCommand = new DelegateCommand(ExecuteSubmitCommand, CanCloseDialog)
                .ObservesProperty(() => Name);
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public DelegateCommand SubmitCommand { get; }

        public event Action<IDialogParameters> RequestClose;

        private void ExecuteSubmitCommand()
        {
            var dialogParams = new DialogParameters
            {
                { "name", Name }
            };
            RequestClose(dialogParams);
        }

        public bool CanCloseDialog()
        {
            return !string.IsNullOrWhiteSpace(Name) && Name.Length > 2;
        }

        public void OnDialogClosed()
        {
            Console.WriteLine($"User Alert Closed with Name: {Name}");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
