using Prism.Dialogs;
using System;

namespace HelloWorld.Core
{
    public static class DialogServiceExtensions
    {
        public static void ShowNotification(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.Show("NotificationDialog", new DialogParameters($"message={message}"), callBack);
        }

        public static void ShowNotificationInAnotherWindow(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.Show("NotificationDialog", new DialogParameters($"message={message}"), callBack, "AnotherDialogWindow");
        }

        public static void ShowConfirmation(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog("ConfirmationDialog", new DialogParameters($"message={message}"), callBack);
        }

        public static void ShowConfirmationInAnotherWindow(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog("ConfirmationDialog", new DialogParameters($"message={message}"), callBack, "AnotherDialogWindow");
        }
    }
}
