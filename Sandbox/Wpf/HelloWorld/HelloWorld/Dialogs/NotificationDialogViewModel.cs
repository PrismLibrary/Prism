using Prism.Commands;
using Prism.Services.Dialogs;

namespace HelloWorld.Dialogs
{
    public class NotificationDialogViewModel : DialogViewModelBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand CloseDialogCommand { get; set; }

        public NotificationDialogViewModel()
        {
            Title = "Notification";
            CloseDialogCommand = new DelegateCommand(CloseDialog);
        }

        private void CloseDialog()
        {
            RaiseRequestClose(new DialogResult(true));
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}
