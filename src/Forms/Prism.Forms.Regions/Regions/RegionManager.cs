using System;
using Prism.Navigation;
using Prism.Regions.Navigation;

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
        /// Creates a new region manager.
        /// </summary>
        /// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
        public IRegionManager CreateRegionManager() =>
            new RegionManager();

        public void RequestNavigate(string regionName, Uri target, Action<IRegionNavigationResult> navigationCallback) =>
            RequestNavigate(regionName, target, navigationCallback, null);

        public void RequestNavigate(string regionName, Uri target, Action<IRegionNavigationResult> navigationCallback, INavigationParameters navigationParameters)
        {
            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException(nameof(regionName));

            var region = Regions[regionName];

            if (region is null)
                throw new Exception("Region not Found");

            region.NavigationService.RequestNavigate(target, navigationCallback, navigationParameters);
        }
    }
}
