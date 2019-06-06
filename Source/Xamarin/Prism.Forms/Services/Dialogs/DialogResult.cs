using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace Prism.Services.Dialogs
{
    internal class DialogResult : IDialogResult
    {
        public Exception Exception { get; set; }
        public IDialogParameters Parameters { get; set; }
    }
}
