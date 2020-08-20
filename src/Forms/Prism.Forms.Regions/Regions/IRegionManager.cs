using System;
using Prism.Navigation;
using Prism.Regions.Navigation;

namespace Prism.Regions
{
    /// <summary>
    /// Defines an interface to manage a set of <see cref="IRegion">regions</see> and to attach regions to objects (typically controls).
    /// </summary>
    public interface IRegionManager
    {
        /// <summary>
        /// Gets a collection of <see cref="IRegion"/> that identify each region by name. You can use this collection to add or remove regions to the current region manager.
        /// </summary>
        IRegionCollection Regions { get; }

        /// <summary>
        /// Creates a new region manager.
        /// </summary>
        /// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
        IRegionManager CreateRegionManager();

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="regionName">The name of the Region to navigate to.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        void RequestNavigate(string regionName, Uri target, Action<IRegionNavigationResult> navigationCallback);

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="regionName">The name of the Region to navigate to.</param>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        void RequestNavigate(string regionName, Uri target, Action<IRegionNavigationResult> navigationCallback, INavigationParameters navigationParameters);
    }
}
