using System;

namespace Prism.Services.Dialogs
{
    public interface IDialogResult
    {
        Exception Exception { get; }
        IDialogParameters Parameters { get; }
    }
}
