using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace ModuleA.ViewModels
{
    public class ViewCViewModel : BindableBase, INavigationAware, IDestructible
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

            NavigateCommand = new DelegateCommand(async () => await Navigate());
        }

        async Task Navigate()
        {
            try
            {
                //await _navigationService.NavigateAsync("ViewB");

                await _navigationService.GoBackToRootAsync();

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
    }
}
