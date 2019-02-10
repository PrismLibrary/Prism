using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SampleData.StarTrek;
using Windows.UI.Xaml.Media.Animation;

namespace Sample.ViewModels
{
    internal class ItemPageViewModel : BindableBase, INavigatedAware
    {
        private INavigationService _navigationService { get; }

        public ItemPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoBackCommand = new DelegateCommand(OnGoBackCommandExecuted);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Member = parameters.GetValue<Member>("member");
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            // empty
        }

        private Member _member;
        public Member Member
        {
            get => _member;
            set => SetProperty(ref _member, value);
        }

        public DelegateCommand GoBackCommand { get; }

        private async void OnGoBackCommandExecuted()
        {
            await _navigationService.GoBackAsync();
        }
    }
}