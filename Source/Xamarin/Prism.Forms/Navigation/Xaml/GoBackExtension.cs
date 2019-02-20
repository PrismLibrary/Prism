using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    [ContentProperty(nameof(GoBackType))]
    public class GoBackExtension : NavigationExtensionBase
    {
        public static readonly BindableProperty GoBackTypeProperty =
            BindableProperty.Create(nameof(GoBackType), typeof(GoBackType), typeof(GoBackExtension), GoBackType.Default);

        public GoBackType GoBackType
        {
            get => (GoBackType)GetValue(GoBackTypeProperty);
            set => SetValue(GoBackTypeProperty, value);
        }

        protected override async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
        {
            var result = GoBackType == GoBackType.ToRoot ?
                await navigationService.GoBackToRootAsync(parameters) :
                await navigationService.GoBackAsync(parameters, animated: Animated, useModalNavigation: UseModalNavigation);

            if (result.Exception != null)
            {
                Log(result.Exception, parameters);
            }
        }
    }
}