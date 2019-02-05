using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogService
    {
        /// <summary>
        /// Shows a non-modal dialog
        /// </summary>
        /// <param name="name">The name of the dialog to show</param>
        /// <param name="parameters">The parameters to pass to the dialog</param>
        /// <param name="callback">The actin to perform when the dialog is closed.</param>
        void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback);

        /// <summary>
        /// Shows a modal dialog
        /// </summary>
        /// <param name="name">The name of the dialog to show</param>
        /// <param name="parameters">The parameters to pass to the dialog</param>
        /// <param name="callback">The actin to perform when the dialog is closed.</param>
        void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}