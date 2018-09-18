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

        public int OnNavigatingToCalled { get; private set; }

        public int OnNavigatedToCalled { get; private set; }

        public int OnNavigatedFromCalled { get; private set; }

        private async void OnNavigateCommandExecuted()
        {
            await _navigationService.NavigateAsync("/AutowireView");
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            SomeText = parameters.GetValue<string>("text");
            OnNavigatingToCalled++;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            OnNavigatedToCalled++;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            OnNavigatedFromCalled++;
        }
    }
}