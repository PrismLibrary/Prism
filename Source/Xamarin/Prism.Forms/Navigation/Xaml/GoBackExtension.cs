using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Navigation.Xaml
{
    [ContentProperty(nameof(GoBackType))]
    public class GoBackExtension : Navigation
    {
        public bool Animated { get; set; } = true;
        public GoBackType GoBackType { get; set; } = GoBackType.Default;
        public bool? UseModalNavigation { get; set; } = null;

        public override async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(Bindable);

            IsNavigating = true;
            RaiseCanExecuteChanged();

            var navigationService = GetNavigationService(Page);

            if (GoBackType == GoBackType.ToRoot)
                await navigationService.GoBackToRootAsync(parameters);
            else
                await navigationService.GoBackAsync(parameters, UseModalNavigation, Animated);

            IsNavigating = false;
            RaiseCanExecuteChanged();
        }
    }
}