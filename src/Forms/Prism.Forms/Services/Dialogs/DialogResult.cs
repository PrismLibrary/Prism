using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Provides information about a dialog that has been closed.
    /// </summary>
    internal class DialogResult : IDialogResult
    {
        /// <inheritdoc />
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public IDialogParameters Parameters { get; set; }
    }
}
