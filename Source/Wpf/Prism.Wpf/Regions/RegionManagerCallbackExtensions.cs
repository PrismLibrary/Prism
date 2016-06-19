using System;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public static class RegionManagerCallbackExtensions
    {
        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <param name="navigationCallback">The navigation callback.</param>
        /// <returns></returns>
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var task = regionManager.RequestNavigateImpl(regionName, source, navigationCallback);
        }

        private static async Task RequestNavigateImpl(this IRegionManager regionManager, string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, source);
            navigationCallback(result);
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <param name="navigationCallback">The navigation callback.</param>
        /// <returns></returns>
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var task = regionManager.RequestNavigateImpl(regionName, source, navigationCallback);
        }

        private static async Task RequestNavigateImpl(this IRegionManager regionManager, string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, source);
            navigationCallback(result);
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationCallback">The navigation callback that will be executed after the navigation is completed.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var task = regionManager.RequestNavigateImpl(regionName, target, navigationCallback, navigationParameters);
        }

        private static async Task RequestNavigateImpl(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, target, navigationParameters);
            navigationCallback(result);
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target string, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationCallback">The navigation callback that will be executed after the navigation is completed.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns></returns>
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            if (navigationCallback == null)
                throw new ArgumentNullException(nameof(navigationCallback));

            var task = regionManager.RequestNavigateImpl(regionName, target, navigationCallback, navigationParameters);
        }

        private static async Task RequestNavigateImpl(this IRegionManager regionManager, string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, target, navigationParameters);
            navigationCallback(result);
        }
    }
}
