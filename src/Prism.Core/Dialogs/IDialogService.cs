namespace Prism.Dialogs;

/// <summary>
/// Defines a contract for displaying dialogs from ViewModels.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Displays a dialog.
    /// </summary>
    /// <param name="name">The unique name of the dialog to display. Must match an entry in the <see cref="Prism.Ioc.IContainerRegistry"/>.</param>
    /// <param name="parameters">Parameters that the dialog can use for custom functionality.</param>
    /// <param name="callback">The action to be invoked upon successful or failed completion of displaying the dialog.</param>
    /// <example>
    /// In a view model method, use an injected dialog service to display the registered
    /// DemoDialog view and handle its result or a display error. The dialog's view model
    /// implements IDialogAware and receives these parameters in OnDialogOpened.
    /// <code>
    /// var parameters = new DialogParameters
    /// {
    ///     { "title", "Connection Lost!" },
    ///     { "message", "We seem to have lost network connectivity" }
    /// };
    /// var callback = new DialogCallback()
    ///     .OnClose(result =>
    ///     {
    ///         if (result.Result == ButtonResult.OK)
    ///             Console.WriteLine("Dialog accepted.");
    ///     })
    ///     .OnError((Exception error) => Console.WriteLine(error.Message));
    /// dialogService.ShowDialog("DemoDialog", parameters, callback);
    /// </code>
    /// </example>
    void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback);
}
