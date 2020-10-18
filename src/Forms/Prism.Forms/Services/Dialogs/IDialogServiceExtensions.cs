using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prism.Ioc;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Common extensions for <see cref="IDialogService"/>
    /// </summary>
    public static class IDialogServiceExtensions
    {
        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="name">The unique name of the dialog to display. Must match an entry in the <see cref="IContainerRegistry"/>.</param>
        public static void ShowDialog(this IDialogService dialogService, string name) =>
            dialogService.ShowDialog(name, null, null);

        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="name">The unique name of the dialog to display. Must match an entry in the <see cref="IContainerRegistry"/>.</param>
        /// <param name="callback">The action to be invoked upon successful or failed completion of displaying the dialog.</param>
        public static void ShowDialog(this IDialogService dialogService, string name, Action<IDialogResult> callback) =>
            dialogService.ShowDialog(name, null, callback);

        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="name">The unique name of the dialog to display. Must match an entry in the <see cref="IContainerRegistry"/>.</param>
        /// <param name="parameters">Parameters that the dialog can use for custom functionality.</param>
        public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters) =>
            dialogService.ShowDialog(name, parameters, null);

        /// <summary>
        /// Displays a dialog asynchronously.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="name">The unique name of the dialog to display. Must match an entry in the <see cref="IContainerRegistry"/>.</param>
        /// <param name="parameters">Parameters that the dialog can use for custom functionality.</param>
        /// <returns><see cref="IDialogResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name, IDialogParameters parameters = null)
        {
            var tcs = new TaskCompletionSource<IDialogResult>();
            void DialogCallback(IDialogResult dialogResult)
            {
                tcs.SetResult(dialogResult);
            }
            dialogService.ShowDialog(name, parameters, DialogCallback);
            return tcs.Task;
        }
    }
}
