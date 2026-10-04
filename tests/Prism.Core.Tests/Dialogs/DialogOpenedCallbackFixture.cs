using System.Runtime.CompilerServices;
using Prism.Core.Tests.Mocks;
using Prism.Dialogs;
using Xunit;

namespace Prism.Core.Tests.Dialogs;

public class DialogOpenedCallbackFixture
{
    [Fact]
    public async Task OpenedCallbacksReceiveSameListenerAndRunInOrder()
    {
        var model = new DialogAwareMock();
        var events = new List<string>();
        DialogUtilities.InitializeListener(model, (IDialogResult result) => events.Add($"close:{result.Result}"));
        var callback = new DialogCallback()
            .OnOpened(_ => events.Add("opened"))
            .OnOpenedAsync(async listener =>
            {
                events.Add("work");
                await listener.InvokeAsync(new DialogResult(ButtonResult.OK));
            });
        await callback.InvokeOpened(model.RequestClose);
        Assert.Equal(new[] { "opened", "work", "close:OK" }, events);
    }

    [Fact]
    public async Task ClearedCopiesIgnoreRepeatedCloseAndStopRemainingOpenedCallbacks()
    {
        var model = new DialogAwareMock();
        var count = 0;
        DialogUtilities.InitializeListener(model, (IDialogResult _) =>
        {
            count++;
            DialogUtilities.ClearListener(model);
        });
        var copy = model.RequestClose;
        await new DialogCallback().OnOpened(listener => listener.Invoke()).OnOpened(_ => count += 10)
            .InvokeOpened(copy);
        await copy.InvokeAsync();
        await model.RequestClose.InvokeAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task OpenedErrorsUseExistingErrorCallbacks()
    {
        var model = new DialogAwareMock();
        DialogUtilities.InitializeListener(model, (IDialogResult _) => { });
        Exception error = null;
        var callback = new DialogCallback().OnOpened(_ => throw new InvalidOperationException("work"))
            .OnError<InvalidOperationException>(ex => error = ex);
        await callback.InvokeOpened(model.RequestClose);
        Assert.Equal("work", error.Message);
        await default(DialogCallback).InvokeOpened(model.RequestClose);
        await default(DialogCallback).Invoke(new InvalidOperationException());
        await DialogCallback.Empty.OnOpened(_ => throw new Exception()).InvokeOpened(model.RequestClose);
    }

    [Fact]
    public void RetainedListenerDoesNotRetainClosedDialogState()
    {
        var (listener, reference) = CreateClosedListener();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(reference.IsAlive);
        GC.KeepAlive(listener);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (DialogCloseListener, WeakReference) CreateClosedListener()
    {
        var model = new DialogAwareMock();
        var captured = new object();
        DialogUtilities.InitializeListener(model, (IDialogResult _) => GC.KeepAlive(captured));
        var listener = model.RequestClose;
        DialogUtilities.ClearListener(model);
        return (listener, new WeakReference(captured));
    }
}
