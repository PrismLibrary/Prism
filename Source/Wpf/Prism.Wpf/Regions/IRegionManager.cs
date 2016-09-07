

using System;
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
        ///     Add a view to the Views collection of a Region. Note that the region must already exist in this regionmanager. 
        /// </summary>
        /// <param name="regionName">The name of the region to add a view to</param>
        /// <param name="view">The view to add to the views collection</param>
        /// <returns>The RegionManager, to easily add several views. </returns>
        IRegionManager AddToRegion(string regionName, object view);

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="viewType">The type of the view to register with the </param>
        /// <returns>The regionmanager, for adding several views easily</returns>
        IRegionManager RegisterViewWithRegion(string regionName, Type viewType);

        /// <summary>
        /// Associate a view with a region, using a delegate to resolve a concrete instance of the view. 
        /// When the region get's displayed, this delegate will be called and the result will be added to the
        /// views collection of the region. 
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="getContentDelegate">The delegate used to resolve a concrete instance of the view.</param>
        /// <returns>The regionmanager, for adding several views easily</returns>
        IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate);

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <param name="navigationCallback">The navigation callback.</param>
        void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback);

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        void RequestNavigate(string regionName, Uri source);

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <param name="navigationCallback">The navigation callback.</param>
        void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback);

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        void RequestNavigate(string regionName, string source);

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationCallback">The navigation callback that will be executed after the navigation is completed.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters);

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target string, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationCallback">The navigation callback that will be executed after the navigation is completed.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters);

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters);

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target string, passing an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters);
    }
}
