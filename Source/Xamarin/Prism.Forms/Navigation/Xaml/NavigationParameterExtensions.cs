using Xamarin.Forms;
using XamlNavParams =  Prism.Navigation.Xaml.NavigationParameters;
using NavParams =  Prism.Navigation.NavigationParameters;

namespace Prism.Navigation.Xaml
{
    internal static class NavigationParameterExtensions
    {
        public static NavParams ToNavigationParameters(this object parameter, BindableObject parent)
        {
            parameter = parameter ?? new NavParams();
            switch (parameter)
            {
                case NavParams parameters:
                    return parameters;
                case NavigationParameter xamlParameter:
                    xamlParameter.BindingContext = xamlParameter.BindingContext ?? parent.BindingContext;
                    return new NavParams { { xamlParameter.Key, xamlParameter.Value } };
                case XamlNavParams xamlParameters:
                    return xamlParameters.ToNavigationParameters(parent);
                default:
                    return new NavParams { { KnownNavigationParameters.XamlParam, parameter } };
            }
        }
    }
}