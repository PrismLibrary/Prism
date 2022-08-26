using System;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Represents a class that returns information about a dialog that has been closed.
    /// </summary>
    public interface IDialogResult
    {
        /// <summary>
        /// Gets the exception of the dialog.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets the parameters from the dialog.
        /// </summary>
        IDialogParameters Parameters { get; }
    }
}
