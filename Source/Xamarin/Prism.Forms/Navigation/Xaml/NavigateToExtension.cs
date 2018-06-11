using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    [ContentProperty(nameof(Name))]
    public class NavigateToExtension : NavigationExtensionBase
    {
        public string Name { get; set; }

        protected override async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
        {
            await navigationService.NavigateAsync(Name, parameters);
        }
    }
}