using System.Threading.Tasks;

namespace Prism.Navigation.TabbedPages
{
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Selects a Tab of the TabbedPage parent.
        /// </summary>
        /// <param name="name">The name of the tab to select</param>
        /// <param name="parameters">The navigation parameters</param>
        public static Task<INavigationResult> SelectTabAsync(this INavigationService navigationService, string name, INavigationParameters parameters = null)
        {
            return ((IPlatformNavigationService)navigationService).SelectTabAsync(name, parameters);
        }
    }
}
