using System;
using System.Threading.Tasks;
using Prism.Ioc;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Defines a contract for displaying dialogs from ViewModels.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <param name="name">The unique name of the dialog to display. Must match an entry in the <see cref="IContainerRegistry"/>.</param>
        /// <param name="parameters">Parameters that the dialog can use for custom functionality.</param>
        /// <param name="callback">The action to be invoked upon successful or failed completion of displaying the dialog.</param>
        /// <example>
        /// This example shows how to display a dialog with two parameters.
        /// <code>
        /// var parameters = new DialogParameters
        /// {
        ///     { "title", "Connection Lost!" },
        ///     { "message", "We seem to have lost network connectivity" }
        /// };
        /// _dialogService.ShowDialog("DemoDialog", parameters, <paramref name="callback"/>: null);
        /// </code>
        /// </example>
        void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}
