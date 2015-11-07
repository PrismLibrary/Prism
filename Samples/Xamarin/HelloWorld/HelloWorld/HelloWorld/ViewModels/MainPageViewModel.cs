using HelloWorld.Events;
using HelloWorld.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;

namespace HelloWorld.ViewModels
{
    public class MainPageViewModel : BindableBase
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
            _eventAggregator.GetEvent<NavigationUriReceivedEvent>().Subscribe(UriReceived);
        }

        private void UriReceived(string uri)
        {
            var navUri = new Uri(uri);
            _navigationService.Navigate(navUri);
        }

        void Navigate()
        {
            var relative = "ViewB?message=DeepLink";
            var http = "http://HelloWorld.com/ViewB?message=DeepLink";
            var nonHttp = "android-app://HelloWorld/ViewA/ViewB?message=DeepLink";

            var uri = new Uri(relative);

            _navigationService.Navigate(uri);
        }
    }

}
