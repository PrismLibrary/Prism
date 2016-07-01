

using System;
using System.Threading.Tasks;

namespace Prism.Regions
{
    /// <summary>
    /// Provides additional methods to the <see cref="INavigateAsync"/> interface.
    /// </summary>
    public static class NavigationAsyncExtensions
    {
        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        public static void RequestNavigate(this INavigateAsync navigation, string target)
        {
            RequestNavigate(navigation, target, nr => { });
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            navigation.RequestNavigate(target, navigationCallback, null);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        public static void RequestNavigate(this INavigateAsync navigation, Uri target)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            navigation.RequestNavigate(target, nr => { });
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);

            navigation.RequestNavigate(targetUri, navigationCallback, navigationParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, Uri target, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            navigation.RequestNavigate(target, nr => { }, navigationParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, string target, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            navigation.RequestNavigate(new Uri(target, UriKind.RelativeOrAbsolute), nr => { }, navigationParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target" />.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, Uri target, Action<NavigationResult> navigationCallback)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            navigation.RequestNavigate(target, navigationCallback, null);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target" />.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var task = navigation.RequestNavigateImpl(target, navigationCallback, navigationParameters);
        }

        private static async Task RequestNavigateImpl(this INavigateAsync navigation, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await navigation.RequestNavigateAsync(target, navigationParameters);
            navigationCallback(result);
        }

        /// <summary>
        /// Initiates navigation to the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigation, Uri target)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return navigation.RequestNavigateAsync(target, null);
        }
    }
}
