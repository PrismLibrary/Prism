namespace Prism.Services;

public interface IDialogAware
{
    bool CanCloseDialog();

    void OnDialogClosed();

    void OnDialogOpened(IDialogParameters parameters);

    DialogCloseEvent RequestClose { get; set; }
}
