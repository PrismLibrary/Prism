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
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        public static Task<bool> GoBackAsync(this INavigationService navigationService, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            if (parameters == null)
                parameters = new NavigationParameters();

            parameters.Add(KnownNavigationParameters.UseModalNavigation, useModalNavigation);
            parameters.Add(KnownNavigationParameters.Animated, animated);

            return navigationService.GoBackAsync(parameters);
        }

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
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.", ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        public static Task NavigateAsync(this INavigationService navigationService, string name, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            if (parameters == null)
                parameters = new NavigationParameters();

            parameters.Add(KnownNavigationParameters.UseModalNavigation, useModalNavigation);
            parameters.Add(KnownNavigationParameters.Animated, animated);

            return navigationService.NavigateAsync(name, parameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public static Task NavigateAsync(this INavigationService navigationService, Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            if (parameters == null)
                parameters = new NavigationParameters();

            parameters.Add(KnownNavigationParameters.UseModalNavigation, useModalNavigation);
            parameters.Add(KnownNavigationParameters.Animated, animated);

            return navigationService.NavigateAsync(uri, parameters);
        }
    }
}
