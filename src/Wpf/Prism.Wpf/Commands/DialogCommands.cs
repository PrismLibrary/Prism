using System.Windows.Input;
using Prism.Commands.Parameters;
using Prism.Dialogs;
using Prism.Ioc;

namespace Prism.Commands;

/// <summary>
/// Commands for handling dialog operations using Xaml commands
/// </summary>
public static class DialogCommands
{
    private static IDialogService dialogService = ContainerLocator.Container.Resolve<IDialogService>();

    /// <summary>
    /// Command for closing current dialog
    /// </summary>
    public static ICommand CloseDialogCommand =>
        new DelegateCommand<CloseDialogCommandParameter>(OnCloseDialogCommandExecuted);

    /// <summary>
    /// Command for showing dialog
    /// </summary>
    public static ICommand ShowDialogCommand =>
        new DelegateCommand<ShowDialogCommandParameter>(OnShowDialogCommandExecuted);

    private static void OnCloseDialogCommandExecuted(CloseDialogCommandParameter obj)
    {
        if (obj is null)
            return;

        obj.DialogContext.RequestClose.Invoke(obj.ButtonResult);
    }

    private static void OnShowDialogCommandExecuted(ShowDialogCommandParameter obj)
    {
        dialogService.ShowDialog(obj.DialogName, obj.Parameters);
    }
}
