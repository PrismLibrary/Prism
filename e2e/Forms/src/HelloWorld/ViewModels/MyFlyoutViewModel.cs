using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;

namespace HelloWorld.ViewModels
{
    public class MyFlyoutViewModel : ViewModelBase
    {
        INavigationService _navigationService;

        public string NavigatePath => "MyNavigationPage/ViewA";

        public string Message => "Hello from MyMasterDetailViewModel";

        public DelegateCommand<string> NavigateCommand { get; set; }
        public MyFlyoutViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private async void Navigate(string name)
        {
            var result = await _navigationService.NavigateAsync(name);
            if (!result.Success)
            {

            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
