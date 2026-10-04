using Prism.Dialogs;

namespace Prism.Core.Tests.Mocks;

internal sealed class DialogAwareMock : IDialogAware
{
    public DialogCloseListener RequestClose { get; }
    public bool CanCloseDialog() => true;
    public void OnDialogOpened(IDialogParameters parameters) { }
    public void OnDialogClosed() { }
}
