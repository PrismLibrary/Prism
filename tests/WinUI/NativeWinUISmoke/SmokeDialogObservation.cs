using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NativeWinUISmoke;

public sealed class SmokeDialogObservation
{
    public int OpenedCount { get; set; }
    public int ClosedCount { get; set; }
    public int LoadedCount { get; set; }
    public int OpenSequence { get; set; }
    public int CloseSequence { get; set; }
    public WindowLifetime? ViewLifetime { get; set; }
    public SmokeDialogViewModel? ViewModel { get; set; }
    public XamlRoot? ViewRoot { get; set; }
    public XamlRoot? HostRoot { get; set; }
    public ContentDialog? Host { get; set; }
    public TaskCompletionSource<bool> Loaded { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}
