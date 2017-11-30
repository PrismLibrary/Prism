using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Prism.AppModel;

namespace ModuleA.ViewModels
{
    public class ViewCViewModel : BindableBase, INavigationAware, IDestructible, IPageLifecycleAware
    {
        private readonly INavigationService _navigationService;

        string _title = "View C";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public ViewCViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new DelegateCommand(Navigate);
        }

        void Navigate()
        {
            try
            {
                var uri = _navigationService.GetNavigationUriPath();

                Debug.WriteLine("After _navigationService.NavigateAsync(ViewB) ...");
            }
            catch(Exception ex)
            {

            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            Debug.WriteLine("OnNavigatedFrom ViewC ...");
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public void Destroy()
        {
            
        }

        public void OnAppearing()
        {
            Debug.WriteLine("ViewC is appearing");
        }

        public void OnDisappearing()
        {
            Debug.WriteLine("ViewC is disappearing");
        }
    }
}
