using Playground.Module.Dialogs;

namespace Playground.Module.ViewModels;

internal class DialogLabViewModel : BindableBase
{
    private readonly IDialogService _dialogService;

    public DialogLabViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        ShowNotificationCommand = new DelegateCommand(ShowNotification);
        ShowConfirmCommand = new DelegateCommand(ShowConfirm);
    }

    public string LastResult
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public DelegateCommand ShowNotificationCommand { get; }

    public DelegateCommand ShowConfirmCommand { get; }

    private void ShowNotification()
    {
        _dialogService.ShowDialog(
            nameof(NotificationDialog),
            new DialogParameters
            {
                { "title", "Notification" },
                { "message", "This dialog is registered in PlaygroundModule and hosted by Prism’s native WinUI DialogService." }
            });
    }

    private void ShowConfirm()
    {
        _dialogService.ShowDialog(
            nameof(ConfirmDialog),
            new DialogParameters { { "message", "Discard unsaved playground changes?" } },
            r => LastResult = $"Confirm dialog closed: {r.Result}");
    }
}
