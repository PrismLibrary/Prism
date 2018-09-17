using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class PartialViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navigationService { get; }

        public PartialViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand(OnNavigateCommandExecuted);
        }

        private string _someText;
        public string SomeText
        {
            get => _someText;
            set => SetProperty(ref _someText, value);
        }

        public DelegateCommand NavigateCommand { get; }

        public bool OnNavigatingToCalled { get; private set; }

        public bool OnNavigatedToCalled { get; private set; }

        public bool OnNavigatedFromCalled { get; private set; }

        private async void OnNavigateCommandExecuted()
        {
            await _navigationService.NavigateAsync("/AutowireView");
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            SomeText = parameters.GetValue<string>("text");
            OnNavigatingToCalled = true;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            OnNavigatedToCalled = true;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            OnNavigatedFromCalled = true;
        }
    }
}