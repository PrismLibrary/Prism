using System.Windows.Input;

namespace Prism.Dialogs;

public interface IDialogContainer
{
    View DialogView { get; }
    ICommand Dismiss { get; }
    Task ConfigureLayout(Page currentPage, View dialogView, bool hideOnBackgroundTapped, ICommand dismissCommand, IDialogParameters parameters);
    Task DoPop(Page currentPage);
}
