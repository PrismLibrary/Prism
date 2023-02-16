namespace Prism.Services;

public static class IDialogServiceExtensions
{
    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters) =>
        dialogService.ShowDialog(name, parameters, default);

    public static void ShowDialog(this IDialogService dialogService, string name) =>
        dialogService.ShowDialog(name, new DialogParameters());

    public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name) =>
        dialogService.ShowDialogAsync(name, new DialogParameters());

    public static void ShowDialog(this IDialogService dialogService, string name, Action callback) =>
        dialogService.ShowDialog(name, null, callback);

    public static void ShowDialog(this IDialogService dialogService, string name, Action<IDialogResult> callback) =>
        dialogService.ShowDialog(name, null, callback);

    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters, Action callback) =>
        dialogService.ShowDialog(name, parameters, DialogCallback.OnClose(callback));

    public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
        dialogService.ShowDialog(name, parameters, DialogCallback.OnClose(callback));

    public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name, IDialogParameters parameters)
    {
        var tcs = new TaskCompletionSource<IDialogResult>();
        dialogService.ShowDialog(name, parameters, result => tcs.TrySetResult(result));
        return tcs.Task;
    }
}
