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
            _dialogService = dialogService;
            ShowDialogCommand = new DelegateCommand(ShowDialog);            
        }

        private void ShowDialog()
        {
            var message = "This is a message that should be shown in the dialog.";

            //using the dialog service as-is
            //_dialogService.ShowDialog("NotificationDialog", new DialogParameters($"message={message}"), r =>
            //{
            //    if (r.Result == ButtonResult.None)
            //        Title = "Result is None";
            //    else if (r.Result == ButtonResult.OK)
            //        Title = "Result is OK";
            //    else if (r.Result == ButtonResult.Cancel)
            //        Title = "Result is Cancel";
            //    else
            //        Title = "I Don't know what you did!?";
            //});

            //using custom extenions methods to simplify the app's dialogs
            //_dialogService.ShowNotification(message, r =>
            //{
            //    if (r.Result == ButtonResult.None)
            //        Title = "Result is None";
            //    else if (r.Result == ButtonResult.OK)
            //        Title = "Result is OK";
            //    else if (r.Result == ButtonResult.Cancel)
            //        Title = "Result is Cancel";
            //    else
            //        Title = "I Don't know what you did!?";
            //});

            _dialogService.ShowConfirmation(message, r =>
            {
                if (r.Result == ButtonResult.None)
                    Title = "Result is None";
                else if (r.Result == ButtonResult.OK)
                    Title = "Result is OK";
                else if (r.Result == ButtonResult.Cancel)
                    Title = "Result is Cancel";
                else
                    Title = "I Don't know what you did!?";
            });
        }
    }

    public static class DialogServiceExtensions
    {
        public static void ShowNotification(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog("NotificationDialog", new DialogParameters($"message={message}"), callBack);
        }

        public static void ShowConfirmation(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog("ConfirmationDialog", new DialogParameters($"message={message}"), callBack);
        }
    }
}
