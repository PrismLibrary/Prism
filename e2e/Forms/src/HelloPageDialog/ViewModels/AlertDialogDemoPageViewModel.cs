using Prism.Commands;
using Prism.Services;

namespace HelloPageDialog.ViewModels
{
    public class AlertDialogDemoPageViewModel : PageDialogDemoBaseViewModel
    {
        public AlertDialogDemoPageViewModel(IPageDialogService pageDialogs) : base(pageDialogs)
        {
            ShowAlertCommand = new DelegateCommand(OnShowAlertCommandExecuted,
                    () => !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Message))
                .ObservesProperty(() => Title)
                .ObservesProperty(() => Message);
        }

        public DelegateCommand ShowAlertCommand { get; }

        private async void OnShowAlertCommandExecuted()
        {
            await _pageDialogs.DisplayAlertAsync(Title, Message, "Ok", FlowDirection);
        }
    }
}
