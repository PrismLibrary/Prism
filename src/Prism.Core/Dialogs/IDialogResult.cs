using System;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// Contains <see cref="IDialogParameters"/> from the dialog
/// and the <see cref="ButtonResult"/> of the dialog.
/// </summary>
public interface IDialogResult
{
    /// <summary>
    /// An <see cref="System.Exception"/> that was thrown by the DialogService
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// The result of the dialog.
    /// </summary>
    public ButtonResult Result { get; }

    /// <summary>
    /// The parameters from the dialog.
    /// </summary>
    IDialogParameters Parameters { get; }
}
