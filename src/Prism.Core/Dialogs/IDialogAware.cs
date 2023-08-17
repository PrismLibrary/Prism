namespace Prism.Dialogs;

/// <summary>
/// Provides a way for objects involved in Dialogs to be notified of Dialog activities.
/// </summary>
public interface IDialogAware
{
    /// <summary>
    /// Evaluates whether the Dialog is in a state that would allow the Dialog to Close
    /// </summary>
    /// <returns><c>true</c> if the Dialog can close</returns>
    bool CanCloseDialog();

    /// <summary>
    /// Provides a callback to clean up resources or finalize tasks when the Dialog has been closed
    /// </summary>
    void OnDialogClosed();

    /// <summary>
    /// Initializes the state of the Dialog with provided DialogParameters
    /// </summary>
    /// <param name="parameters"></param>
    void OnDialogOpened(IDialogParameters parameters);

    /// <summary>
    /// The <see cref="DialogCloseListener"/> will be set by the <see cref="IDialogService"/> and can be called to
    /// invoke the close of the Dialog.
    /// </summary>
    DialogCloseListener RequestClose { get; }
}
