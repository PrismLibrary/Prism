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
        }

        void Navigate()
        {
            var basic = "ViewA?id=23";

            //skips MainPage because it is the current root and not registered for navigation
            var nonHttp = "android-app://HelloWorld/MainPage/MyNavigationPage/ViewA/MyTabbedPage?selectedItem=Orangutan/ViewA/ViewB/ViewC";

            var uri = new Uri(nonHttp);
            _navigationService.Navigate(uri);
        }
    }

}
