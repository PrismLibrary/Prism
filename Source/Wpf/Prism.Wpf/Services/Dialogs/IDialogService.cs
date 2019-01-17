using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogService
    {
        void ShowNotification(string title, string message, Action<IDialogResult> callback);

        //ShowConfirmation

        //ShowDialog - this is for custom dialogs

        //RegisterDialog

        //RegisterDialogWindow
    }
}