using Prism.Navigation;
using Prism.Navigation.Xaml;
using PrismDialogParameters = Prism.Services.Dialogs.DialogParameters;
using XamlDialogParams = Prism.Services.Dialogs.Xaml.DialogParameters;

namespace Prism.Services.Dialogs.Xaml
{
    internal static class DialogParameterExtensions
    {
        public static IDialogParameters ToNavigationParameters(this object parameter)
        {
            parameter = parameter ?? new PrismDialogParameters();
            switch (parameter)
            {
                case IDialogParameters parameters:
                    return parameters;
                case DialogParameter xamlParameter:
                    return new PrismDialogParameters { { xamlParameter.Key, xamlParameter.Value } };
                case XamlDialogParams xamlParameters:
                    var navParams = new PrismDialogParameters();
                    foreach(var param in xamlParameters)
                    {
                        navParams.Add(param.Key, param.Value);
                    }
                    return navParams;
                default:
                    return new PrismDialogParameters { { KnownNavigationParameters.XamlParam, parameter } };
            }
        }
    }
}
