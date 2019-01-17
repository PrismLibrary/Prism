using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogService
    {
        void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback);

        void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}