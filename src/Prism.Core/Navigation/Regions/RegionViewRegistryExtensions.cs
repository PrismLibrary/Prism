using System;
using System.Collections.Generic;
using Prism.Ioc;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides extensions for the <see cref="IRegionViewRegistry"/>.
    /// </summary>
    public static class RegionViewRegistryExtensions
    {
        /// <summary>
        /// Registers a delegate that can be used to retrieve the content associated with a region name.
        /// </summary>
        /// <param name="viewRegistry">The <see cref="IRegionViewRegistry"/> instance.</param>
        /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
        /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
        public static void RegisterViewWithRegion(this IRegionViewRegistry viewRegistry, string regionName, Func<object> getContentDelegate) =>
            viewRegistry.RegisterViewWithRegion(regionName, _ => getContentDelegate());

        /// <summary>
        /// Returns the contents associated with a region name.
        /// </summary>
        /// <param name="viewRegistry">The current <see cref="IRegionViewRegistry"/> instance.</param>
        /// <param name="regionName">Region name for which contents are requested.</param>
        /// <returns>Collection of contents associated with the <paramref name="regionName"/>.</returns>
        public static IEnumerable<object> GetContents(this IRegionViewRegistry viewRegistry, string regionName) =>
            viewRegistry.GetContents(regionName, ContainerLocator.Container);
    }
}
