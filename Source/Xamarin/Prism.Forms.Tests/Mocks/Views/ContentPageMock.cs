using System;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class ContentPageMock : ContentPage, INavigationAware, IConfirmNavigationAsync
    {
        public bool OnNavigatedToCalled { get; private set; } = false;
        public bool OnNavigatedFromCalled { get; private set; } = false;
        public bool OnNavigatingToCalled { get; private set; } = false;

        public bool OnConfirmNavigationCalled { get; private set; } = false;

        public ContentPageMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            OnNavigatedFromCalled = true;
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            OnNavigatedToCalled = true;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            OnNavigatingToCalled = true;
        }

        public Task<bool> CanNavigateAsync(NavigationParameters parameters)
        {
            return Task.Run(() =>
            {
                OnConfirmNavigationCalled = true;

                if (parameters.ContainsKey("canNavigate"))
                    return (bool)parameters["canNavigate"];

                return true;
            });
        }
    }
}
