using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Prism.AppModel;
using Prism.Navigation.TabbedPages;

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

        async void Navigate()
        {
            try
            {
                //var uri = _navigationService.GetNavigationUriPath();

                //Debug.WriteLine("After _navigationService.NavigateAsync(ViewB) ...");

                //var result = await _navigationService.NavigateAsync("../../");
                //if (!result.Success)
                //{

                //}

                await _navigationService.SelectTabAsync("ViewB?id=3");
            }
            catch(Exception ex)
            {

            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            Debug.WriteLine("OnNavigatedFrom ViewC ...");
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(INavigationParameters parameters)
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
