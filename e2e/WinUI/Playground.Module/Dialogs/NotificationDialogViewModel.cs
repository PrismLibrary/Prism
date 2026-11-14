namespace Playground.Module.Dialogs;

internal class NotificationDialogViewModel : BindableBase, IDialogAware
{
    public NotificationDialogViewModel()
    {
        CloseCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));
    }

    public string Title
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public string Message
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public DelegateCommand CloseCommand { get; }

    public DialogCloseListener RequestClose { get; set; }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title") ?? string.Empty;
        Message = parameters.GetValue<string>("message") ?? string.Empty;
    }
}
