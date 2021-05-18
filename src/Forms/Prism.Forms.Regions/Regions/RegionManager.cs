using System;
using System.Globalization;
using System.Threading;
using Prism.Common;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Prism.Navigation;
using Prism.Properties;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Regions
{
    /// <summary>
    /// This class is responsible for maintaining a collection of regions and attaching regions to controls.
    /// </summary>
    /// <remarks>
    /// This class supplies the attached properties that can be used for simple region creation from XAML.
    /// </remarks>
    public partial class RegionManager : IRegionManager
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RegionManager"/>.
        /// </summary>
        public RegionManager()
        {
            Regions = new RegionCollection(this);
        }

        /// <summary>
        /// Gets a collection of <see cref="IRegion"/> that identify each region by name. You can use this collection to add or remove regions to the current region manager.
        /// </summary>
        /// <value>A <see cref="IRegionCollection"/> with all the registered regions.</value>
        public IRegionCollection Regions { get; }

        /// <summary>
        /// Add a view to the Views collection of a Region. Note that the region must already exist in this <see cref="IRegionManager"/>.
        /// </summary>
        /// <param name="regionName">The name of the region to add a view to</param>
        /// <param name="targetName">The view to add to the views collection</param>
        /// <returns>The RegionManager, to easily add several views. </returns>
        public IRegionManager AddToRegion(string regionName, string targetName)
        {
            if (!Regions.ContainsRegionWithName(regionName))
                throw new ArgumentException(string.Format(Thread.CurrentThread.CurrentCulture, Resources.RegionNotFound, regionName), nameof(regionName));

            var view = CreateNewRegionItem(targetName);

            return Regions[regionName].Add(view);
        }

        /// <summary>
        /// Creates a new region manager.
        /// </summary>
        /// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
        public IRegionManager CreateRegionManager() =>
            new RegionManager();

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="viewType">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            var regionViewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();

            regionViewRegistry.RegisterViewWithRegion(regionName, viewType);

            return this;
        }

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="targetName">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public IRegionManager RegisterViewWithRegion(string regionName, string targetName)
        {
            var viewType = ContainerLocator.Current.GetRegistrationType(targetName);

            return RegisterViewWithRegion(regionName, viewType);
        }

        /// <summary>
        /// Navigates the specified region manager.
        /// </summary>
        /// <param name="regionName">The name of the region to call Navigate on.</param>
        /// <param name="target">The URI of the content to display.</param>
        /// <param name="navigationCallback">The navigation callback.</param>
        public void RequestNavigate(string regionName, Uri target, Action<IRegionNavigationResult> navigationCallback) =>
            RequestNavigate(regionName, target, navigationCallback, null);

        /// <summary>
        /// This method allows an <see cref="IRegionManager"/> to locate a specified region and navigate in it to the specified target <see cref="Uri"/>, passing a navigation callback and an instance of <see cref="INavigationParameters"/>, which holds a collection of object parameters.
        /// </summary>
        /// <param name="regionName">The name of the region where the navigation will occur.</param>
        /// <param name="target">A <see cref="Uri"/> that represents the target where the region will navigate.</param>
        /// <param name="navigationCallback">The navigation callback that will be executed after the navigation is completed.</param>
        /// <param name="navigationParameters">An instance of <see cref="INavigationParameters"/>, which holds a collection of object parameters.</param>
        public void RequestNavigate(string regionName, Uri target, Action<IRegionNavigationResult> navigationCallback, INavigationParameters navigationParameters)
        {
            try
            {
                if (string.IsNullOrEmpty(regionName))
                    throw new ArgumentNullException(nameof(regionName));

                var region = Regions[regionName];

                if (region is null)
                    throw new Exception("Region not Found");

                region.NavigationService.RequestNavigate(target, navigationCallback, navigationParameters);
            }
            catch (Exception ex)
            {
                var navigationContext = new NavigationContext(null, target, navigationParameters);
                navigationCallback?.Invoke(new RegionNavigationResult(navigationContext, ex));
            }
        }

        /// <summary>
        /// Provides a new item for the region based on the supplied candidate target contract name.
        /// </summary>
        /// <param name="candidateTargetContract">The target contract to build.</param>
        /// <returns>An instance of an item to put into the <see cref="IRegion"/>.</returns>
        protected virtual VisualElement CreateNewRegionItem(string candidateTargetContract)
        {
            try
            {
                var view = ContainerLocator.Container.Resolve<object>(candidateTargetContract) as VisualElement;

                PageUtilities.SetAutowireViewModel(view);

                return view;
            }
            catch (ContainerResolutionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.CannotCreateNavigationTarget, candidateTargetContract),
                    e);
            }
        }
    }
}
