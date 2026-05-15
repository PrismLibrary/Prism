namespace Prism.Dialogs;

/// <summary>
/// Provides a way for objects involved in Dialogs to be notified of Dialog activities.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IDialogAware"/> is the primary interface that Dialog ViewModels should implement.
/// It allows ViewModels to participate in the dialog lifecycle and control when dialogs can be closed.
/// </para>
/// <para>
/// Dialog services will automatically call the methods on this interface at appropriate points
/// in the dialog lifecycle (when opened, when closed, before closing to check if closing is allowed).
/// </para>
/// </remarks>
    public interface IDialogAware
{
    /// <summary>
    /// Evaluates whether the Dialog is in a state that would allow the Dialog to Close
    /// </summary>
    /// <returns><see langword="true"/> if the Dialog can close; otherwise, <see langword="false"/></returns>
    /// <remarks>
    /// This method is called before the dialog is closed. If it returns <see langword="false"/>, the close operation is cancelled.
    /// Use this to prevent users from closing a dialog while operations are in progress or when validation fails.
    /// </remarks>
    bool CanCloseDialog();

    /// <summary>
    /// Provides a callback to clean up resources or finalize tasks when the Dialog has been closed
    /// </summary>
    /// <remarks>
    /// This method is called after the dialog window has closed. Use it to clean up any resources
    /// or perform final actions related to the dialog.
    /// </remarks>
    void OnDialogClosed();

    /// <summary>
    /// Initializes the state of the Dialog with provided DialogParameters
    /// </summary>
    /// <param name="parameters">Dialog parameters passed when showing the dialog</param>
    /// <remarks>
    /// This method is called after the dialog is displayed but before it is shown to the user.
    /// Use it to initialize the dialog's state based on the provided parameters.
    /// </remarks>
    void OnDialogOpened(IDialogParameters parameters);

    /// <summary>
    /// The <see cref="DialogCloseListener"/> will be set by the <see cref="IDialogService"/> and can be called to
    /// invoke the close of the Dialog.
    /// </summary>
    /// <remarks>
    /// This listener is set by the dialog service and allows the ViewModel to request that the dialog be closed,
    /// optionally with a result that indicates the outcome of the dialog (OK, Cancel, etc.).
    /// </remarks>
    DialogCloseListener RequestClose { get; }
}
