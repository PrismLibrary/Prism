

using System;
using System.Threading.Tasks;

using Prism.Regions;
using Prism.Common;

namespace Prism.Regions.CallbackWrappers
{
    /// <summary>
    /// Creates instance of <see cref="IRegionManager"/> based on an instance of <see cref="IRegionManagerWithCallbacks"/>
    /// </summary>
    /// <seealso cref="Prism.Regions.IRegionManager" />
    public class RegionManagerWithCallbacksWrapper : IRegionManager
    {
        private IRegionManagerWithCallbacks _regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionManagerWithCallbacksWrapper"/> class.
        /// </summary>
        /// <param name="regionManager">The underlying region manager.</param>
        public RegionManagerWithCallbacksWrapper(IRegionManagerWithCallbacks regionManager)
        {
            _regionManager = regionManager;
        }

        /// <summary>
        /// Gets a collection of <see cref="IRegion"/> that identify each region by name. You can use this collection to add or remove regions to the current region manager.
        /// </summary>
        public IRegionCollection Regions
        {
            get { return _regionManager.Regions; }
        }

        /// <summary>
        ///     Add a view to the Views collection of a Region. Note that the region must already exist in this regionmanager. 
        /// </summary>
        /// <param name="regionName">The name of the region to add a view to</param>
        /// <param name="view">The view to add to the views collection</param>
        /// <returns>The RegionManager, to easily add several views. </returns>
        public IRegionManager AddToRegion(string regionName, object view)
        {
            return _regionManager.AddToRegion(regionName, view);
        }

        /// <summary>
        /// Creates a new region manager.
        /// </summary>
        /// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
        public IRegionManager CreateRegionManager()
        {
            return _regionManager.CreateRegionManager();
        }

        /// <summary>
        /// Associate a view with a region, using a delegate to resolve a concreate instance of the view. 
        /// When the region get's displayed, this delelgate will be called and the result will be added to the
        /// views collection of the region. 
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="getContentDelegate">The delegate used to resolve a concreate instance of the view.</param>
        /// <returns>The regionmanager, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            return _regionManager.RegisterViewWithRegion(regionName, getContentDelegate);
        }

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="viewType">The type of the view to register with the </param>
        /// <returns>The regionmanager, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            return _regionManager.RegisterViewWithRegion(regionName, viewType);
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        public void RequestNavigate(string regionName, string source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        public void RequestNavigate(string regionName, Uri source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target string, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The navigation result.</returns>
        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <returns>The navigation result.</returns>
        public Task<NavigationResult> RequestNavigateAsync(string regionName, string source)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, source, callback));
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="source">The URI of the content to display.</param>
        /// <returns>The navigation result.</returns>
        public Task<NavigationResult> RequestNavigateAsync(string regionName, Uri source)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, source, callback));
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target string, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A string that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The navigation result.</returns>
        public Task<NavigationResult> RequestNavigateAsync(string regionName, string target, NavigationParameters navigationParameters)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, target, callback, navigationParameters));
        }

        /// <summary>
        /// This method allows an IRegionManager to locate a specified region and navigate in it to the specified target Uri, passing a navigation callback and an instance of NavigationParameters, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A Uri that represents the target where the region will navigate.</param>
        /// <param name="navigationParameters">An instance of NavigationParameters, which holds a collection of object parameters.</param>
        /// <returns>The navigation result.</returns>
        public Task<NavigationResult> RequestNavigateAsync(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, target, callback, navigationParameters));
        }
    }
}
