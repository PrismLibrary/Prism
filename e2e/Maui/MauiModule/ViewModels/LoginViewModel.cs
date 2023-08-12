namespace MauiModule.ViewModels;

public class LoginViewModel : BindableBase, IDialogAware
{
    private bool _canClose;

    public LoginViewModel()
    {
        LoginCommand = new DelegateCommand(OnLoginCommandExecuted);
    }

    public string Title => "What's your name?";

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public DelegateCommand LoginCommand { get; }

    public DialogCloseListener RequestClose { get; }

    public bool CanCloseDialog() => _canClose;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
    }

    private void OnLoginCommandExecuted()
    {
        _canClose = true;
        RequestClose.Invoke();
    }
}
