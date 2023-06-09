using System;

#nullable enable
namespace Prism.Dialogs;

/// <summary>
/// An <see cref="IDialogResult"/> that contains <see cref="IDialogParameters"/> from the dialog
/// and the <see cref="ButtonResult"/> of the dialog.
/// </summary>
#if NET6_0_OR_GREATER
public record DialogResult : IDialogResult
{
    /// <summary>
    /// An <see cref="System.Exception"/> that was thrown by the DialogService
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// The parameters from the dialog.
    /// </summary>
    public IDialogParameters Parameters { get; init; }

    /// <summary>
    /// The result of the dialog.
    /// </summary>
    public ButtonResult Result { get; init; } = ButtonResult.None;
}
#else
public class DialogResult : IDialogResult
{
    /// <summary>
    /// An <see cref="System.Exception"/> that was thrown by the DialogService
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// The parameters from the dialog.
    /// </summary>
    public IDialogParameters Parameters { get; set; }

    /// <summary>
    /// The result of the dialog.
    /// </summary>
    public ButtonResult Result { get; private set; } = ButtonResult.None;
}
#endif
