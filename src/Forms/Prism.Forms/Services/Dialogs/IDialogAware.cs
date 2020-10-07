using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogAware
    {
        bool CanCloseDialog();

        void OnDialogClosed();

        void OnDialogOpened(IDialogParameters parameters);

        event Action<IDialogParameters> RequestClose;
    }
}
