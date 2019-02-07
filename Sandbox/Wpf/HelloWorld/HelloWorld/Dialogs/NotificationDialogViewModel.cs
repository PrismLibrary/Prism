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

        public NotificationDialogViewModel()
        {
            Title = "Notification";
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}
