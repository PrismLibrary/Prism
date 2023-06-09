using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Prism.Dialogs;

public readonly struct DialogCallback
{
    private MulticastDelegate _callback { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DialogCallback(MulticastDelegate callback)
    {
        _callback = callback;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task Invoke(Exception ex) =>
        Invoke(new DialogResult { Exception = ex });

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task Invoke(IDialogResult result)
    {
        if (_callback is null)
            return;
        else if (_callback is Action action)
            action();
        else if (_callback is Action<IDialogResult> actionResult)
            actionResult(result);
        else if (_callback is Action<Exception> actionError && result.Exception is not null)
            actionError(result.Exception);
        else if (_callback is Func<Task> func)
            await func();
        else if (_callback is Func<IDialogResult, Task> funcResult)
            await funcResult(result);
        else if(_callback is Func<Exception, Task> funcError && result.Exception is not null)
            await funcError(result.Exception);
    }

    public static DialogCallback OnClose(Action action) =>
        new (action);

    public static DialogCallback OnClose(Action<IDialogResult> action) =>
        new (action);

    public static DialogCallback OnError(Action<Exception> action) =>
        new (action);

    public static DialogCallback OnCloseAsync(Func<Task> func) =>
        new(func);

    public static DialogCallback OnCloseAsync(Func<IDialogResult, Task> func) =>
        new(func);

    public static DialogCallback OnErrorAsync(Func<Exception, Task> func) =>
        new(func);
}
