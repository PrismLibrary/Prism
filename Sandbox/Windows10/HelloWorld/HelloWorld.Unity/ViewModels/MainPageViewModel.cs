using Prism.Mvvm;
using Prism.Navigation;
using Sample.Models;
using SampleData.StarTrek;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Sample.ViewModels
{
    class MainPageViewModel : BindableBase, INavigatedAwareAsync
    {
        private static readonly Database _data;

        static MainPageViewModel()
        {
            _data = new Database();
        }

        private INavigationService _nav;

        public MainPageViewModel()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals(nameof(SearchString)))
                {
                    FillMembers();
                }
            };
        }

        public async Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            _nav = parameters.GetNavigationService();
            await _data.OpenAsync();
            FillMembers();
        }

        public ObservableCollection<GroupedMembers> Members { get; } = new ObservableCollection<GroupedMembers>();

        string _searchString = string.Empty;
        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value);
        }

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

        public async void ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem is Member m)
            {
                await _nav.NavigateAsync(nameof(Views.ItemPage), new DrillInNavigationTransitionInfo(), (nameof(Member), m.ToJson()));
            }
        }
    }
}
