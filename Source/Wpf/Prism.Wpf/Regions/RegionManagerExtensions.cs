using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Common;

namespace Prism.Regions
{
    public static class RegionManagerExtensions
    {
        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this IRegionManager regionManager, string regionName, Uri source)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            return CallbackHelper.AwaitCallbackResult< NavigationResult>(callback => regionManager.RequestNavigate(regionName, source, callback));
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this IRegionManager regionManager, string regionName, string source)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => regionManager.RequestNavigate(regionName, source, callback));
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this IRegionManager regionManager, string regionName, Uri target, NavigationParameters navigationParameters)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => regionManager.RequestNavigate(regionName, target, callback, navigationParameters));
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target string, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this IRegionManager regionManager, string regionName, string target, NavigationParameters navigationParameters)
        {
            if (regionManager == null)
                throw new ArgumentNullException(nameof(regionManager));

            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => regionManager.RequestNavigate(regionName, target, callback, navigationParameters));
        }
    }
}
