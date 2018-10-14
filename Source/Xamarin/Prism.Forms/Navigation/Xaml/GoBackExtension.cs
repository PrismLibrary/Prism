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
            var result = GoBackType == GoBackType.ToRoot ?
                await navigationService.GoBackToRootAsync(parameters) :
                await navigationService.GoBackAsync(parameters);

            if (result.Exception != null)
            {
                Log(result.Exception);
            }
        }
    }
}