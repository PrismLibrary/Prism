using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IRegionAware
    {
        private IRegionNavigationService _navigationService { get; }

        protected ViewModelBase(IRegionNavigationService regionNavigationService)
        {
            _navigationService = regionNavigationService;
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
        }

        public string Title { get; set; }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DelegateCommand GoBackCommand { get; }

        public DelegateCommand GoForwardCommand { get; }

        public bool IsNavigationTarget(INavigationContext navigationContext) =>
            false;

        public void OnNavigatedFrom(INavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(INavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("message"))
                Message = navigationContext.Parameters.GetValue<string>("message");
            else
                Message = "No Message provided...";
        }

        private void GoBack()
        {
            _navigationService.Journal.GoBack();
        }

        private void GoForward()
        {
            _navigationService.Journal.GoForward();
        }
    }
}
