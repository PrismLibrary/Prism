using Prism.Mvvm;
using Prism.Navigation;
using SampleData.StarTrek;
using Windows.UI.Xaml.Media.Animation;

namespace Sample.ViewModels
{
    internal class ItemPageViewModel : BindableBase, INavigatedAware
    {
        private INavigationService _nav;

        public ItemPageViewModel()
        {
            // empty
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            _nav = parameters.GetNavigationService();
            if (parameters.TryGetValue<string>(nameof(Member), out var parameter))
            {
                if (Member.TryFromJson(parameter, out var member))
                {
                    Member = member;
                }
                else { /* invalid parameter */ }
            }
            else { /* missing parameter */ }
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

        public async void GoBack()
        {
            await _nav.GoBackAsync();
        }
    }
}