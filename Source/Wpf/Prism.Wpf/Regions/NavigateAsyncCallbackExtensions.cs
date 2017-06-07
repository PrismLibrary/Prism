using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public static class NavigateAsyncCallbackExtensions
    {
        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        public static Task RequestNavigate(this INavigateAsync navigation, Uri target, Action<NavigationResult> navigationCallback)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            return navigation.RequestNavigateImpl(target, navigationCallback);
        }

        private static async Task RequestNavigateImpl(this INavigateAsync navigation, Uri target, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await navigation.RequestNavigateAsync(target);
            navigationCallback(result);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target" />.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        public static Task RequestNavigate(this INavigateAsync navigation,  Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            return navigation.RequestNavigateImpl(target, navigationCallback, navigationParameters);
        }

        private static async Task RequestNavigateImpl(this INavigateAsync navigation, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await navigation.RequestNavigateAsync(target, navigationParameters);
            navigationCallback(result);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        public static Task RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            return navigation.RequestNavigateImpl(target, navigationCallback);
        }

        private static async Task RequestNavigateImpl(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await navigation.RequestNavigate(target);
            navigationCallback(result);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static Task RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);

            return navigation.RequestNavigate(targetUri, navigationCallback, navigationParameters);
        }
    }
}
