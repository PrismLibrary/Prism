using Prism.Dialogs;

namespace NativeWinUISmoke;

/// <summary>Captures both callback paths, including accidental duplicate completion.</summary>
public sealed class SmokeDialogRequest
{
    private readonly TaskCompletionSource<IDialogResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public SmokeDialogRequest(IDialogService service, string id, bool autoClose, string dialogName = nameof(SmokeDialog))
    {
        var callback = new DialogCallback()
            .OnClose(Complete)
            .OnError((Exception exception) => Complete(new DialogResult { Exception = exception }));
        try
        {
            service.ShowDialog(dialogName,
                new DialogParameters { { "id", id }, { "autoClose", autoClose },
                    { KnownDialogParameters.WindowName, id == "cancel-veto" ? nameof(ObservedDialogWindow) : string.Empty } }, callback);
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
