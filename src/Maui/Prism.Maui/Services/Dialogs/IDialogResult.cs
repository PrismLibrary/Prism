namespace Prism.Services;

public interface IDialogResult
{
    Exception Exception { get; }
    IDialogParameters Parameters { get; }
}
