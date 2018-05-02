using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    [ContentProperty(nameof(Name))]
    public class NavigateToExtension : Navigation
    {
        public bool Animated { get; set; } = true;
        public string Name { get; set; }
        public bool? UseModalNavigation { get; set; } = null;

        public override async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(Bindable);
             
            IsNavigating = true;
            RaiseCanExecuteChanged();
            
            var navigationService = GetNavigationService(Page);
            await navigationService.NavigateAsync(Name, parameters, UseModalNavigation, Animated);

            IsNavigating = false;
            RaiseCanExecuteChanged();
        }
    }
}