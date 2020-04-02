using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class XamlViewMockAViewModel : BindableBase, INavigationAware
    {
        private string _fizz;
        private string _test = "Initial Value";

        public string Fizz
        {
            get => _fizz;
            set => SetProperty(ref _fizz, value);
        }

        public string Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            parameters.TryGetValue(nameof(Fizz), out _fizz);
            RaisePropertyChanged(nameof(Fizz));
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
        }
    }
}