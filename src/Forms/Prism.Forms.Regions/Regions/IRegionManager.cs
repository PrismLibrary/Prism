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
        /// Add a view to the Views collection of a Region. Note that the region must already exist in this <see cref="IRegionManager"/>. 
        /// </summary>
        /// <param name="regionName">The name of the region to add a view to</param>
        /// <param name="targetName">The view to add to the views collection</param>
        /// <returns>The RegionManager, to easily add several views. </returns>
        IRegionManager AddToRegion(string regionName, string targetName);

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="targetName">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        IRegionManager RegisterViewWithRegion(string regionName, string targetName);

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="viewType">The type of the view to register with the  <see cref="IRegion"/>.</param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        IRegionManager RegisterViewWithRegion(string regionName, Type viewType);

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
