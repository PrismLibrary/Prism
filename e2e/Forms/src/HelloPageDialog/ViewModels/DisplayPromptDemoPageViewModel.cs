using System.Collections.Generic;
using Prism.AppModel;
using Prism.Commands;
using Prism.Services;

namespace HelloPageDialog.ViewModels
{
    public class DisplayPromptDemoPageViewModel : PageDialogDemoBaseViewModel
    {
        public DisplayPromptDemoPageViewModel(IPageDialogService pageDialogs) : base(pageDialogs)
        {
            ShowPromptCommand = new DelegateCommand(OnShowPromptCommandExecuted,
                    () => !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Message))
                .ObservesProperty(() => Title)
                .ObservesProperty(() => Message);

            KeyboardTypes = new[]
            {
                KeyboardType.Chat,
                KeyboardType.Default,
                KeyboardType.Email,
                KeyboardType.Numeric,
                KeyboardType.Plain,
                KeyboardType.Telephone,
                KeyboardType.Text,
                KeyboardType.Url
            };
        }

        private string _placeholder;
        public string Placeholder
        {
            get => _placeholder;
            set => SetProperty(ref _placeholder, value);
        }

        private string _accept = "OK";
        public string Accept
        {
            get => _accept;
            set => SetProperty(ref _accept, value);
        }

        private string _cancel = "Cancel";
        public string Cancel
        {
            get => _cancel;
            set => SetProperty(ref _cancel, value);
        }

        private KeyboardType _keyboardType = KeyboardType.Default;
        public KeyboardType KeyboardType
        {
            get => _keyboardType;
            set => SetProperty(ref _keyboardType, value);
        }

        public IEnumerable<KeyboardType> KeyboardTypes { get; }

        private int _maxLength = -1;
        public int MaxLength
        {
            get => _maxLength;
            set => SetProperty(ref _maxLength, value);
        }

        private string _initialValue;
        public string InitialValue
        {
            get => _initialValue;
            set => SetProperty(ref _initialValue, value);
        }

        public DelegateCommand ShowPromptCommand { get; }

        private async void OnShowPromptCommandExecuted()
        {
            var value = await _pageDialogs.DisplayPromptAsync(Title, Message, Accept, Cancel, Placeholder, MaxLength, KeyboardType);

            if (!string.IsNullOrEmpty(value))
                await _pageDialogs.DisplayAlertAsync("Success", $"You entered: '{value}'.", "Ok");
        }
    }
}
