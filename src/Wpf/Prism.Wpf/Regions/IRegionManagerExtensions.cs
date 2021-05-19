namespace Prism.Regions
{
    /// <summary>
    /// Common Extensions for the RegionManager
    /// </summary>
    public static class IRegionManagerExtensions
    {
        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <typeparam name="T">The type of the view to register with the  <see cref="IRegion"/>.</typeparam>
        /// <param name="regionManager">The current <see cref="IRegionManager"/>.</param>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        public static IRegionManager RegisterViewWithRegion<T>(this IRegionManager regionManager, string regionName) =>
            regionManager.RegisterViewWithRegion(regionName, typeof(T));
    }
}
