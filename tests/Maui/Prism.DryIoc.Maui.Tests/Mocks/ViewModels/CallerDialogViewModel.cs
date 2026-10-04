using Prism.Dialogs;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public class CallerDialogViewModel : IDialogAware
{
    public CallerDialogViewModel(IDialogService dialogService) => DialogService = dialogService;
    public IDialogService DialogService { get; }
    public DialogCloseListener RequestClose { get; }
    public bool CanCloseDialog() => true;
    public void OnDialogOpened(IDialogParameters parameters) { }
    public void OnDialogClosed() { }
}
