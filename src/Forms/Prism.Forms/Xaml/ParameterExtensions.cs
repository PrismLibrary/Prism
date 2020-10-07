using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Forms;

namespace Prism.Xaml
{
    internal static class ParameterExtensions
    {
        public static INavigationParameters ToNavigationParameters(this object parameter, BindableObject parent)
        {
            parameter = parameter ?? new NavigationParameters();
            switch (parameter)
            {
                case INavigationParameters parameters:
                    return parameters;
                case Parameter xamlParameter:
                    xamlParameter.BindingContext = xamlParameter.BindingContext ?? parent.BindingContext;
                    return new NavigationParameters { { xamlParameter.Key, xamlParameter.Value } };
                case Parameters xamlParameters:
                    xamlParameters.BindingContext = xamlParameters.BindingContext ?? parent.BindingContext;
                    return xamlParameters.ToParameters<NavigationParameters>(parent);
                default:
                    return new NavigationParameters { { KnownNavigationParameters.XamlParam, parameter } };
            }
        }

        public static IDialogParameters ToDialogParameters(this object parameter, BindableObject parent)
        {
            parameter = parameter ?? new DialogParameters();
            switch (parameter)
            {
                case IDialogParameters parameters:
                    return parameters;
                case Parameter xamlParameter:
                    xamlParameter.BindingContext = xamlParameter.BindingContext ?? parent.BindingContext;
                    return new DialogParameters { { xamlParameter.Key, xamlParameter.Value } };
                case Parameters xamlParameters:
                    xamlParameters.BindingContext = xamlParameters.BindingContext ?? parent.BindingContext;
                    return xamlParameters.ToParameters<DialogParameters>(parent);
                default:
                    return new DialogParameters { { KnownDialogParameters.XamlParam, parameter } };
            }
        }
    }
}
