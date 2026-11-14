using Prism.Dialogs;

namespace NativeWinUISmoke;

// Observe the real native Opened event before ShowAsync can raise it.
public sealed class ObservedDialogWindow : DialogWindow
{
    public ObservedDialogWindow(SmokeState state)
    {
        Opened += (_, _) =>
        {
            if (DataContext is SmokeDialogViewModel viewModel)
                state.GetDialog(viewModel.Id).Presented.TrySetResult(true);
        };
    }
}
