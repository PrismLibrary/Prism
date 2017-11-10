using Prism.Common;
using System;
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
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        public static async Task GoBackToRootAsync(this INavigationService navigationService, NavigationParameters parameters = null)
        {

            try
            {
                if (parameters == null)
                    parameters = new NavigationParameters();

                parameters.InternalParameters.Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

                IPageAware pageAware = (IPageAware)navigationService;

                var canNavigate = await PageUtilities.CanNavigateAsync(pageAware.Page, parameters);
                if (!canNavigate)
                    return;

                List<Page> pagesToDestroy = pageAware.Page.Navigation.NavigationStack.ToList(); // get all pages to destroy
                pagesToDestroy.Reverse(); // destroy them in reverse order
                var root = pagesToDestroy.Last();
                pagesToDestroy.Remove(root); //don't destroy the root page

                PageUtilities.OnNavigatingTo(root, parameters);

                await pageAware.Page.Navigation.PopToRootAsync();

                //BOOM!
                foreach (var destroyPage in pagesToDestroy)
                {
                    PageUtilities.OnNavigatedFrom(destroyPage, parameters);
                    PageUtilities.DestroyPage(destroyPage);
                }

                PageUtilities.OnNavigatedTo(root, parameters);
            }
            catch(InvalidOperationException ex)
            {                
                throw new InvalidOperationException("GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.", ex);
            }
            catch
            {
                throw;
            }
        }
    }
}
