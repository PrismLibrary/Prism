using Xamarin.Forms;
using XamlNavParams =  Prism.Navigation.Xaml.NavigationParameters;
using PrismNavParameters =  Prism.Navigation.NavigationParameters;

namespace Prism.Navigation.Xaml
{
    internal static class NavigationParameterExtensions
    {
        public static INavigationParameters ToNavigationParameters(this object parameter, BindableObject parent)
        {
            parameter = parameter ?? new PrismNavParameters();
            switch (parameter)
            {
                case INavigationParameters parameters:
                    return parameters;
                case NavigationParameter xamlParameter:
                    xamlParameter.BindingContext = xamlParameter.BindingContext ?? parent.BindingContext;
                    return new PrismNavParameters { { xamlParameter.Key, xamlParameter.Value } };
                case XamlNavParams xamlParameters:
                    return xamlParameters.ToNavigationParameters(parent);
                default:
                    return new PrismNavParameters { { KnownNavigationParameters.XamlParam, parameter } };
            }
        }
    }
}