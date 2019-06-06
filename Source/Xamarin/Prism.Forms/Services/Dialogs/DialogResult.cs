using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace Prism.Services.Dialogs
{
    internal class DialogResult : IDialogResult
    {
        private bool _success;
        public bool Success
        {
            get => Exception is null ? _success : false;
            set => _success = value;
        }

        public Exception Exception { get; set; }
        public IDialogParameters Parameters { get; set; }
    }
}
