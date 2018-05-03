using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    [ContentProperty(nameof(Name))]
    public class NavigateToExtension : NavigationExtensionBase
    {
        public string Name { get; set; }

        public override async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(Bindable);

            IsNavigating = true;
            RaiseCanExecuteChanged();

            var navigationService = GetNavigationService(SourcePage);
            await HandleNavigation(parameters, navigationService);

            IsNavigating = false;
            RaiseCanExecuteChanged();
        }

        protected override async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
        {
            await navigationService.NavigateAsync(Name, parameters);
        }
    }
}