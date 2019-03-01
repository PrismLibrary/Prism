using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogAware
    {
        /// <summary>
        /// Determines if the dialog can be closed.
        /// </summary>
        /// <returns>True: close the dialog; False: the dialog will not close</returns>
        bool CanCloseDialog();

        /// <summary>
        /// Called when the dialog is closed.
        /// </summary>
        void OnDialogClosed();

        /// <summary>
        /// Called when the dialog is opened.
        /// </summary>
        /// <param name="parameters">The parameters passed to the dialog</param>
        void OnDialogOpened(IDialogParameters parameters);

        /// <summary>
        /// The title of the dialog that wil show in the Window title bar.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Instructs the IDialogWindow to close the dialog.
        /// </summary>
        event Action<IDialogResult> RequestClose;
    }
}
