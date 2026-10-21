using Prism.Dialogs;

namespace NativeWinUISmoke;

public sealed class SmokeDialogViewModel : IDialogAware
{
    private readonly SmokeState _state;

    public SmokeDialogViewModel(SmokeState state) => _state = state;

    public DialogCloseListener RequestClose { get; private set; }
    public bool AutoClose { get; private set; }

    public bool CanCloseDialog() => true;

    public void OnDialogOpened(IDialogParameters parameters)
    {
        AutoClose = parameters.TryGetValue<bool>("autoClose", out var autoClose) && autoClose;
        _state.DialogOpenedCount++;
    }

    public void OnDialogClosed() => _state.DialogClosedCount++;
}
