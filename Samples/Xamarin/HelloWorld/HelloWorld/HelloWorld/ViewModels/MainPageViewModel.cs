using HelloWorld.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;

namespace HelloWorld.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        string _title = "Main Page";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;

            NavigateCommand = new DelegateCommand(Navigate);
        }

        void Navigate()
        {
            var basic = "http://www.brianlagunas.com/MyNavigationPage/ViewA/ViewC/";

            var nonHttp = "MyMasterDetail?id=1/MyNavigationPage?id=Nav/ViewA?id=A/ViewB?id=B";

            //var uri = new Uri(basic, UriKind.Relative);

            _navigationService.Navigate(basic);
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }

}
