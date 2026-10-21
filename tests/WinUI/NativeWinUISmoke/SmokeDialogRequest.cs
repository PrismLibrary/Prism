using Prism.Dialogs;

namespace NativeWinUISmoke;

/// <summary>Captures both callback paths, including accidental duplicate completion.</summary>
public sealed class SmokeDialogRequest
{
    private readonly TaskCompletionSource<IDialogResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public SmokeDialogRequest(IDialogService service, string id, bool autoClose)
    {
        var callback = new DialogCallback()
            .OnClose(Complete)
            .OnError((Exception exception) => Complete(new DialogResult { Exception = exception }));
        try
        {
            service.ShowDialog(nameof(SmokeDialog),
                new DialogParameters { { "id", id }, { "autoClose", autoClose } }, callback);
        }
        catch (Exception exception)
        {
            // A thrown error must fail the callback assertion rather than hang the harness.
            _completion.TrySetResult(new DialogResult { Exception = exception });
        }
    }

    public int CallbackCalls { get; private set; }
    public Task<IDialogResult> Completion => _completion.Task;

    private void Complete(IDialogResult result)
    {
        CallbackCalls++;
        _completion.TrySetResult(result);
    }
}
