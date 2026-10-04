using Prism.Dialogs;

namespace Prism.Wpf.Tests.Mocks;

internal sealed class DialogCallerModel : IDialogAware
{
    public bool AllowClose { get; set; } = true;
    public int ClosedCount { get; private set; }
    public DialogCloseListener RequestClose { get; }
    public bool CanCloseDialog() => AllowClose;
    public void OnDialogOpened(IDialogParameters parameters) { }
    public void OnDialogClosed() => ClosedCount++;
}
