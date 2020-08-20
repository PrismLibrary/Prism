using System;
using Prism.Navigation;

namespace Prism.Regions.Navigation
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
        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<IRegionNavigationResult> navigationCallback)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);

            navigation.RequestNavigate(targetUri, navigationCallback);
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
        /// <param name="regionParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<IRegionNavigationResult> navigationCallback, INavigationParameters regionParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);

            navigation.RequestNavigate(targetUri, navigationCallback, regionParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="regionParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, Uri target, INavigationParameters regionParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            navigation.RequestNavigate(target, nr => { }, regionParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="regionParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this INavigateAsync navigation, string target, INavigationParameters regionParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            navigation.RequestNavigate(new Uri(target, UriKind.RelativeOrAbsolute), nr => { }, regionParameters);
        }
    }
}
