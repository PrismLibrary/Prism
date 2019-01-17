using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Prism.Services.Dialogs.DefaultDialogs
{
    public class NotificationDialogViewModel : BindableBase, IDialog
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action RequestClose;

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand CloseDialogCommand { get; set; }

        private string _iconSource;
        public string IconSource
        {
            get { return _iconSource; }
            set { SetProperty(ref _iconSource, value); }
        }

        public NotificationDialogViewModel()
        {
            CloseDialogCommand = new DelegateCommand(CloseDialog);
        }

        private void CloseDialog()
        {
            RequestClose?.Invoke();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void ProcessDialogParameters(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("title");
            Message = parameters.GetValue<string>("message");
        }
    }
}
