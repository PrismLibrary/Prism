using System;
using System.Threading.Tasks;

namespace Prism.Dialogs;

/// <summary>
/// Provides compatibility Extensions for the <see cref="IDialogService"/>
/// </summary>
public static class IDialogServiceExtensions
{
    /// <summary>
    /// Shows the dialog with the given name and passes parameters to the dialog
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="parameters">The <see cref="IDialogParameters"/> to pass to the dialog</param>
    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters) =>
        dialogService.ShowDialog(name, parameters, DialogCallback.Empty);

    /// <summary>
    /// Shows the dialog with the given name and passes an empty set of DialogParameters
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    public static void ShowDialog(this IDialogService dialogService, string name) =>
        dialogService.ShowDialog(name, new DialogParameters());

    /// <summary>
    /// Shows a dialog with a given name which needs no parameters but has a <see cref="DialogCallback"/>
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="callback">A specified <see cref="DialogCallback"/>.</param>
    public static void ShowDialog(this IDialogService dialogService, string name, DialogCallback callback) =>
        dialogService.ShowDialog(name, new DialogParameters(), callback);

    /// <summary>
    /// Shows a Dialog with a given name and an <see cref="Action"/> for a callback
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="callback"></param>
    /// <remarks>This is for backwards compatibility. Use DialogCallback instead.</remarks>
    public static void ShowDialog(this IDialogService dialogService, string name, Action callback) =>
        dialogService.ShowDialog(name, null, callback);

    /// <summary>
    /// Shows a Dialog with a given name and an <see cref="Action{IDialogResult}"/> for a callback
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="callback"></param>
    /// <remarks>This is for backwards compatibility. Use DialogCallback instead.</remarks>
    public static void ShowDialog(this IDialogService dialogService, string name, Action<IDialogResult> callback) =>
        dialogService.ShowDialog(name, null, callback);

    /// <summary>
    /// Shows a Dialog with a given name and an <see cref="Action"/> for a callback.
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="parameters">The <see cref="IDialogParameters"/> to pass to the dialog</param>
    /// <param name="callback"></param>
    /// <remarks>This is for backwards compatibility. Use DialogCallback instead.</remarks>
    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters, Action callback) =>
        dialogService.ShowDialog(name, parameters, new DialogCallback().OnClose(callback));

    /// <summary>
    /// Shows a Dialog with a given name and an <see cref="Action{IDialogResult}"/> for a callback
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="parameters">The <see cref="IDialogParameters"/> to pass to the dialog</param>
    /// <param name="callback"></param>
    /// <remarks>This is for backwards compatibility. Use DialogCallback instead.</remarks>
    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
        dialogService.ShowDialog(name, parameters, new DialogCallback().OnClose(callback));

    /// <summary>
    /// Asynchronously shows the Dialog and returns the <see cref="IDialogResult"/>.
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <returns>An <see cref="IDialogResult"/> on the close of the dialog.</returns>
    public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name) =>
        dialogService.ShowDialogAsync(name, new DialogParameters());

    /// <summary>
    /// Asynchronously shows the Dialog and returns the <see cref="IDialogResult"/>, with given <see cref="IDialogParameters"/>.
    /// </summary>
    /// <param name="dialogService">The <see cref="IDialogService"/>.</param>
    /// <param name="name">The name of the dialog</param>
    /// <param name="parameters">The <see cref="IDialogParameters"/> to pass to the dialog</param>
    /// <returns>An <see cref="IDialogResult"/> on the close of the dialog.</returns>
    public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name, IDialogParameters parameters)
    {
        var tcs = new TaskCompletionSource<IDialogResult>();
        dialogService.ShowDialog(name, parameters, new DialogCallback().OnClose(result =>
        {
            if (result.Exception is DialogException de && de.Message == DialogException.CanCloseIsFalse)
                return;
            else if (result.Exception is not null)
                tcs.TrySetException(result.Exception);
            else
                tcs.TrySetResult(result);
        }));
        return tcs.Task;
    }
}
