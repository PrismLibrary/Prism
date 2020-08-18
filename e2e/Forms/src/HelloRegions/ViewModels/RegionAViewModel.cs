using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public class RegionAViewModel : ViewModelBase, IRefreshable
    {
        public RegionAViewModel(INavigationService navigationService)
        {
            Title = "Hello from Region A";
            NavigationService = navigationService;

            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        private string _refreshedMessage;
        public string RefreshedMessage
        {
            get => _refreshedMessage;
            set => SetProperty(ref _refreshedMessage, value);
        }

        public INavigationService NavigationService { get; set; }

        public ICommand RefreshCommand { get; }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private async void OnRefresh()
        {
            IsRefreshing = true;

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                RefreshedMessage = $"Last Refreshed: {DateTime.Now:T}";
            }
            catch (System.Exception)
            {
                // You should catch exceptions and handle them here...
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
