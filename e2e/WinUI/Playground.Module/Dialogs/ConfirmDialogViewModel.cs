namespace Playground.Module.Dialogs;

internal class ConfirmDialogViewModel : BindableBase, IDialogAware
{
    public ConfirmDialogViewModel()
    {
        AcceptCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.OK)));
        CancelCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
    }

    public string Message
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public DelegateCommand AcceptCommand { get; }

    public DelegateCommand CancelCommand { get; }

    public DialogCloseListener RequestClose { get; set; }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>("message") ?? "Continue?";
    }
}
