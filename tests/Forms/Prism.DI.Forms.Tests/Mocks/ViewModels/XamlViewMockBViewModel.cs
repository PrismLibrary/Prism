using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class XamlViewMockBViewModel : BindableBase, INavigationAware
    {
        private string _test = "Initial Value";
        private string _foo;  

        public string Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }

        public string Foo
        {
            get => _foo;
            set => SetProperty(ref _foo, value);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            parameters.TryGetValue(nameof(Foo), out _foo);
            RaisePropertyChanged(nameof(Foo));
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
        }
    }
}