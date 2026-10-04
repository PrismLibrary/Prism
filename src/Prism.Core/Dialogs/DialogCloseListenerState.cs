using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Prism.Dialogs;

// Copies of the listener share a releasable delegate rather than retaining a
// closed dialog's window, content, container and caller through a closure.
internal sealed class DialogCloseListenerState
{
    private MulticastDelegate? _callback;

    internal DialogCloseListenerState(MulticastDelegate callback)
    {
        _callback = callback;
    }

    internal bool IsClosed => Volatile.Read(ref _callback) is null;

    internal void Clear()
    {
        Interlocked.Exchange(ref _callback, null);
    }

    internal Task InvokeAsync(IDialogResult result)
    {
        return InvokeCoreAsync(result);
    }

    private Task InvokeCoreAsync(IDialogResult result)
    {
        switch (Volatile.Read(ref _callback))
        {
            case Action<IDialogResult> action:
                action(result);
                return Task.CompletedTask;
            case Func<IDialogResult, Task> func:
                return func(result);
            default:
                return Task.CompletedTask;
        }
    }
}
