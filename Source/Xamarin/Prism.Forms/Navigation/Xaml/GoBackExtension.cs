using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    [ContentProperty(nameof(GoBackType))]
    public class GoBackExtension : NavigationExtensionBase
    {
        public GoBackType GoBackType { get; set; } = GoBackType.Default;

        protected override async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
        {
            if (GoBackType == GoBackType.ToRoot)
                await navigationService.GoBackToRootAsync(parameters);
            else
                await navigationService.GoBackAsync(parameters);
        }
    }
}