using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Services.Dialogs
{
    internal static class IDialogParametersExtensions
    {
        public static void AddNavigationMode(this INavigationParameters parameters, NavigationMode mode)
        {
            if (parameters is INavigationParametersInternal internalParameters)
            {
                internalParameters.Add(KnownInternalParameters.NavigationMode, mode);
            }
        }
    }
}
