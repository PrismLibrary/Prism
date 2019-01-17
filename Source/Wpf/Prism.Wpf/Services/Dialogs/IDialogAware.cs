using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogAware
    {
        bool CanCloseDialog();

        string IconSource { get; set; }

        void OnDialogClosed();

        void OnDialogOpened(IDialogParameters parameters);

        string Title { get; set; }

        event Action<IDialogResult> RequestClose;
    }
}
