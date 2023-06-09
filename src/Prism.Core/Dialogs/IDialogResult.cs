using System;

namespace Prism.Dialogs;

#nullable enable
public interface IDialogResult
{
    Exception? Exception { get; }
    IDialogParameters Parameters { get; }
}
