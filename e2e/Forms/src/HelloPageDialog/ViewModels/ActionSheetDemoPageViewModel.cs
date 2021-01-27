using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Services;

namespace HelloPageDialog.ViewModels
{
    public class ActionSheetDemoPageViewModel : PageDialogDemoBaseViewModel
    {
        public ActionSheetDemoPageViewModel(IPageDialogService pageDialogs)
            : base(pageDialogs)
        {
            Actions = new ObservableCollection<string>();

            ClearCommand = new DelegateCommand(OnClearCommandExecuted, () => !string.IsNullOrEmpty(Title) || Actions.Any())
                .ObservesProperty(() => Title)
                .ObservesProperty(() => Actions);
            ShowActionSheetCommand = new DelegateCommand(OnShowActionSheetCommandExecuted)
                .ObservesProperty(() => Title)
                .ObservesProperty(() => Actions)
                .ObservesCanExecute(() => CanExecute);

            AddCommand = new DelegateCommand(OnAddCommandExecuted, () => !string.IsNullOrEmpty(Message))
                .ObservesProperty(() => Message);
        }

        public ObservableCollection<string> Actions { get; }

        public DelegateCommand ClearCommand { get; }

        public DelegateCommand ShowActionSheetCommand { get; }

        public DelegateCommand AddCommand { get; }

        private bool CanExecute => !string.IsNullOrEmpty(Title) && Actions.Any();

        private void OnAddCommandExecuted()
        {
            if (!string.IsNullOrEmpty(Message))
                Actions.Add(Message);

            Message = null;
        }

        private void OnClearCommandExecuted()
        {
            Title = null;
            Actions.Clear();
        }

        private async void OnShowActionSheetCommandExecuted()
        {
            await _pageDialogs.DisplayActionSheetAsync(Title, FlowDirection, CreateButtons());
        }

        private IActionSheetButton[] CreateButtons()
        {
            var buttons = new List<IActionSheetButton>()
            {
                ActionSheetButton.CreateCancelButton("Cancel", () => Callback("Cancel")),
                ActionSheetButton.CreateDestroyButton("Destroy", () => Callback("Destroy"))
            };

            foreach (var text in Actions)
                buttons.Add(ActionSheetButton.CreateButton(text, () => Callback(text)));

            return buttons.ToArray();
        }

        private Task Callback(string message)
        {
            return _pageDialogs.DisplayAlertAsync("Success", $"You clicked '{message}'.", "Ok");
        }
    }
}
