using Prism.Navigation;

namespace Prism.Services.Dialogs
{
    public interface IDialogResult : INavigationResult
    {
        IDialogParameters Parameters { get; }
    }
}