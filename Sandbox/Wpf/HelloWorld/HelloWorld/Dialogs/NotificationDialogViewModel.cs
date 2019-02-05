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

        public DelegateCommand<string> CloseDialogCommand { get; set; }

        public NotificationDialogViewModel()
        {
            Title = "Notification";
            CloseDialogCommand = new DelegateCommand<string>(CloseDialog);
        }

        private void CloseDialog(string parameter)
        {
            bool? result = null;

            if (parameter.ToLower() == "true")
                result = true;
            else if (parameter.ToLower() == "false")
                result = false;

            RaiseRequestClose(new DialogResult(result));
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}
