using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Sample.Models;
using SampleData.StarTrek;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Sample.ViewModels
{
    class MainPageViewModel : BindableBase, INavigatedAwareAsync
    {
        private IDatabase _data { get; }

        private INavigationService _navigationService { get; }

        public MainPageViewModel(IDatabase data, INavigationService navigationService)
        {
            _data = data;
            _navigationService = navigationService;
            ItemSelectedCommand = new DelegateCommand<Member>(OnItemSelectedCommandExecuted);
        }

        public async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await _data.OpenAsync();
            FillMembers();
        }

        public ObservableCollection<GroupedMembers> Members { get; } = new ObservableCollection<GroupedMembers>();

        string _searchString = string.Empty;
        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value, onChanged: FillMembers);
        }

        public DelegateCommand<Member> ItemSelectedCommand { get; }

        private void FillMembers()
        {
            Members.Clear();
            foreach (var group in _data.Shows
                .OrderBy(x => x.Ordinal)
                .Select(x => new GroupedMembers { Show = x }))
            {
                var members = GetFilteredMembers(group.Show);
                group.Members = new ObservableCollection<Member>(members);
                if (group.Members.Any())
                {
                    Members.Add(group);
                }
            }

            IEnumerable<Member> GetFilteredMembers(Show show)
            {
                return _data.Members
                    .Where(x => x.Show == show.Abbreviation)
                    .Where(x => x.Character.ToLower().Contains(SearchString.Trim().ToLower()) || x.Actor.ToLower().Contains(SearchString.Trim().ToLower()));
            }
        }

        private async void OnItemSelectedCommandExecuted(Member member)
        {
            await _navigationService.NavigateAsync("ItemPage", new DrillInNavigationTransitionInfo(), ("member", member));
        }
    }
}
