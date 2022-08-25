using System;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Contains <see cref="IDialogParameters"/> from the dialog
    /// and the <see cref="System.Exception"/> of the dialog.
    /// </summary>
    public interface IDialogResult
    {
        /// <summary>
        /// The exception of the dialog.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// The parameters from the dialog.
        /// </summary>
        IDialogParameters Parameters { get; }
    }
}
