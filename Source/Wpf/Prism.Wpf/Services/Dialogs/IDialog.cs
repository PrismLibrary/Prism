using System;

namespace Prism.Services.Dialogs
{
    public interface IDialog
    {
        bool CanCloseDialog();

        string IconSource { get; set; }

        string Title { get; set; }

        event Action RequestClose;

        void ProcessDialogParameters(IDialogParameters parameters);
    }
}
