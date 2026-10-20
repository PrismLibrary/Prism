using Prism.Dialogs;

namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

/// <summary>A dialog that stays open until the test requests its normal close lifecycle.</summary>
public sealed class MockRootNavigationDialog : ContentView, IDialogAware
{
    public DialogCloseListener RequestClose { get; }
    public bool AllowClose { get; set; } = true;
    public int Closed { get; private set; }

    public bool CanCloseDialog() => AllowClose;
    public void OnDialogOpened(IDialogParameters parameters) { }
    public void OnDialogClosed() => Closed++;
}
