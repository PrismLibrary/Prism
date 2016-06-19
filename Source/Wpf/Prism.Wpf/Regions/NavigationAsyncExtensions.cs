

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
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigation, string target)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return navigation.RequestNavigateAsync(new Uri(target, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigate(this INavigateAsync navigation, string target)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return navigation.RequestNavigateAsync(target);
        }
        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">The navigation target</param>
        public static Task<NavigationResult> RequestNavigate(this INavigateAsync navigation, Uri target)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return navigation.RequestNavigateAsync(target);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static Task<NavigationResult> RequestNavigate(this INavigateAsync navigation, Uri target, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return navigation.RequestNavigateAsync(target, navigationParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigation, string target, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return navigation.RequestNavigateAsync(new Uri(target, UriKind.RelativeOrAbsolute), navigationParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static Task<NavigationResult> RequestNavigate(this INavigateAsync navigation, string target, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return navigation.RequestNavigateAsync(target, navigationParameters);
        }
    }
}
