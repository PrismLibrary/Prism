using System.Windows.Input;

namespace ModuleA.ViewModels;

internal class ViewAViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    public ViewAViewModel(IRegionManager regionManager, IDialogService dialogService)
        : base(regionManager)
    {
        _dialogService = dialogService;
        ShowAlertCommand = new DelegateCommand(ShowDialog);
    }

    public ICommand ShowAlertCommand { get; }

    private void ShowDialog()
    {
        _dialogService.ShowDialog("AlertDialog", new DialogParameters
        {
            { "title", "Oh Snap" },
            { "message", "You can actually create much more amazing dialogs with Prism. Hello from ViewA!" }
        });
    }
}
