

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
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (target == null) throw new ArgumentNullException("target");

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
            if (navigation == null) throw new ArgumentNullException("navigation");

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
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (target == null) throw new ArgumentNullException("target");

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
            if (navigation == null) throw new ArgumentNullException("navigation");

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
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (target == null) throw new ArgumentNullException("target");

            navigation.RequestNavigate(new Uri(target, UriKind.RelativeOrAbsolute), nr => { }, navigationParameters);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The result of the navigation request.</returns>
        public static async Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigateAsync, string target, NavigationParameters navigationParameters = null)
        {
            return await AwaitCallbackResult<NavigationResult>(callback => navigateAsync.RequestNavigate(target, callback, navigationParameters));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="target"/>.
        /// </summary>
        /// <param name="navigation">The navigation object.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The result of the navigation request.</returns>
        public static async Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigateAsync, Uri target, NavigationParameters navigationParameters = null)
        {
            return await AwaitCallbackResult<NavigationResult>(callback => navigateAsync.RequestNavigate(target, callback, navigationParameters));
        }

        private static async Task<TResult> AwaitCallbackResult<TResult>(Action<Action<TResult>> f)
        {
            var tcs = new TaskCompletionSource<TResult>();
            f(n => tcs.SetResult(n));
            return await tcs.Task;
        }
    }
}
