using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace HelloWorld.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        string _title = "Main Page";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new DelegateCommand(Navigate);
        }

        void Navigate()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("message", "Message from MainPage");

            _navigationService.Navigate("ViewA", parameters);
        }
    }

}
