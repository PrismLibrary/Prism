using Prism.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Navigation
{
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="navigationService">The INavigatinService instance</param>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        public static async Task PopToRootAsync(this INavigationService navigationService)
        {
            IPageAware pageAware = (IPageAware)navigationService;

            List<Page> pagesToDestroy = pageAware.Page.Navigation.NavigationStack.ToList(); // get all pages to destroy
            pagesToDestroy.Reverse(); // destroy them in reverse order
            pagesToDestroy.Remove(pagesToDestroy.Last()); //don't destroy the root page

            await pageAware.Page.Navigation.PopToRootAsync();

            //BOOM!
            foreach (var destroyPage in pagesToDestroy)
            {
                PageUtilities.DestroyPage(destroyPage);
            }
        }
    }
}
