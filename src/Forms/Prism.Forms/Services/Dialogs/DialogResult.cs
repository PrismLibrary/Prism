using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// An <see cref="IDialogResult"/> that contains <see cref="IDialogParameters"/> from the dialog
    /// and the <see cref="System.Exception"/> of the dialog.
    /// </summary>
    internal class DialogResult : IDialogResult
    {
        /// <summary>
        /// The exception of the dialog.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// The parameters from the dialog.
        /// </summary>
        public IDialogParameters Parameters { get; set; }
    }
}
