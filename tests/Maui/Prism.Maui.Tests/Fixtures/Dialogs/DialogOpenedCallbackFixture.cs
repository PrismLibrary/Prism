using Prism.Dialogs;
using Prism.Maui.Tests.Mocks;

#nullable enable
namespace Prism.Maui.Tests.Fixtures.Dialogs;

[Collection("Dialog service lifecycle")]
public class DialogOpenedCallbackFixture : IDisposable
{
    public DialogOpenedCallbackFixture() => DispatcherProvider.SetCurrent(TestDispatcher.Provider);
    public void Dispose() => IDialogContainer.DialogStack.Clear();

    [Fact]
    public async Task WorkStartsAfterHostingAndClosesItsDialogInFinally()
    {
        var push = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var work = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var opened = new TaskCompletionSource<DialogCloseListener>(TaskCreationOptions.RunContinuationsAsynchronously);
        var closed = new TaskCompletionSource<IDialogResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost { PushGate = push.Task };
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show(callback: new DialogCallback().OnOpenedAsync(async listener =>
        {
            Assert.True(model.IsActive);
            Assert.Contains(host, test.Page.Navigation.ModalStack);
            opened.SetResult(listener);
            try { await work.Task; }
            finally { await listener.InvokeAsync(new DialogResult(ButtonResult.OK)); }
        }).OnClose(result => closed.TrySetResult(result)));
        Assert.False(opened.Task.IsCompleted);
        push.SetResult();
        var listener = await opened.Task.WaitAsync(TimeSpan.FromSeconds(5));
        work.SetResult();
        Assert.Equal(ButtonResult.OK, (await closed.Task.WaitAsync(TimeSpan.FromSeconds(5))).Result);
        await listener.InvokeAsync();
        Assert.Equal(1, host.PopCount);
        Assert.Equal(1, model.Closed);
    }

    [Fact]
    public async Task CallerVetoAndCoveredDialogDoNotCloseOtherInstance()
    {
        DialogCloseListener firstListener = default;
        DialogCloseListener secondListener = default;
        var first = new DialogServiceTestHost();
        var second = new DialogServiceTestHost();
        var firstModel = new DialogServiceTestModel { AllowClose = false };
        var secondModel = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(first, firstModel);
        var errors = new List<Exception>();
        test.Show(callback: new DialogCallback().OnOpened(listener => firstListener = listener).OnError(errors.Add));
        await firstListener.InvokeAsync();
        Assert.Equal(0, first.PopCount);
        Assert.Empty(errors);
        firstModel.AllowClose = true;
        test.Add("second", second, secondModel);
        test.Show("second", new DialogCallback().OnOpened(listener => secondListener = listener));
        await firstListener.InvokeAsync();
        Assert.Single(errors);
        Assert.Equal(0, second.PopCount);
        await secondListener.InvokeAsync();
        await firstListener.InvokeAsync();
        Assert.Equal(1, firstModel.Closed);
        Assert.Equal(1, secondModel.Closed);
        Assert.Empty(test.Page.Navigation.ModalStack);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailureOrCancellationOfWorkRoutesErrorAfterFinallyCloses(bool cancelled)
    {
        var error = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost();
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show(callback: new DialogCallback().OnOpenedAsync(async listener =>
        {
            try
            {
                await Task.Yield();
                if (cancelled) throw new OperationCanceledException();
                throw new InvalidOperationException("work failed");
            }
            finally { await listener.InvokeAsync(); }
        }).OnError(ex => error.TrySetResult(ex)));
        var exception = await error.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(cancelled ? typeof(OperationCanceledException) : typeof(InvalidOperationException), exception.GetType());
        Assert.Equal(1, model.Closed);
        Assert.Equal(1, host.PopCount);
    }

    [Fact]
    public async Task UserClosesBeforeWorkCompletesAndFinallyIsANoOp()
    {
        var work = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finished = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var opened = new TaskCompletionSource<DialogCloseListener>(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost();
        var model = new DialogServiceTestModel();
        var test = new DialogServiceTestHarness(host, model);
        test.Show(callback: new DialogCallback().OnOpenedAsync(async listener =>
        {
            opened.SetResult(listener);
            try { await work.Task; }
            finally { await listener.InvokeAsync(); finished.SetResult(); }
        }));
        await opened.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await model.RequestClose.InvokeAsync();
        work.SetResult();
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, host.PopCount);
        Assert.Equal(1, model.Closed);
    }

    [Fact]
    public async Task HostingFailureDoesNotPublishAnOpenedListener()
    {
        var opened = false;
        var error = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new DialogServiceTestHost { PushGate = Task.FromException(new InvalidOperationException("push")) };
        var test = new DialogServiceTestHarness(host, new DialogServiceTestModel());
        test.Show(callback: new DialogCallback().OnOpened(_ => opened = true).OnError(ex => error.TrySetResult(ex)));
        await error.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(opened);
    }
}
