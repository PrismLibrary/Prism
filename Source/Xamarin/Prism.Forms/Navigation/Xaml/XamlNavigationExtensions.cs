using System;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Navigation.Xaml.Prism
{
    public static class XamlNavigationExtensions
    {
        private const string NavParameterMessage = "Command Parameter must be of type NavigationParameters, XamlNavigationParameter, or XamlNavigationParameters";

        public static NavigationParameters ToNavigationParameters(this object parameter, BindableObject parent)
        {
            parameter = parameter ?? new NavigationParameters();
            switch (parameter)
            {
                case NavigationParameters parameters:
                    return parameters;
                case XamlNavigationParameter xamlParameter:
                    xamlParameter.BindingContext = xamlParameter.BindingContext ?? parent.BindingContext;
                    return new NavigationParameters {{xamlParameter.Key, xamlParameter.Value}};
                case XamlNavigationParameters xamlParameters:
                    return xamlParameters.ToNavigationParameters(parent);
            }
            
            throw new ArgumentException(NavParameterMessage, nameof(parameter));
        }
    }
}