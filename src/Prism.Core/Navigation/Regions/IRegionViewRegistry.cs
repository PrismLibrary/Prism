using System;
using System.Collections.Generic;
using Prism.Ioc;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Defines the interface for the registry of region's content.
    /// </summary>
    public interface IRegionViewRegistry
    {
        /// <summary>
        /// Event triggered when a content is registered to a region name.
        /// </summary>
        /// <remarks>
        /// This event uses weak references to the event handler to prevent this service (typically a singleton) of keeping the
        /// target element longer than expected.
        /// </remarks>
        event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

        /// <summary>
        /// Returns the contents associated with a region name.
        /// </summary>
        /// <param name="regionName">Region name for which contents are requested.</param>
        /// <param name="container">The <see cref="IContainerProvider"/> to use to resolve the View.</param>
        /// <returns>Collection of contents associated with the <paramref name="regionName"/>.</returns>
        IEnumerable<object> GetContents(string regionName, IContainerProvider container);

        /// <summary>
        /// Associate a view with a region, by registering a type. When the region get's displayed
        /// this type will be resolved using the ServiceLocator into a concrete instance. The instance
        /// will be added to the Views collection of the region
        /// </summary>
        /// <param name="regionName">The name of the region to associate the view with.</param>
        /// <param name="targetName">The type of the view to register with the </param>
        /// <returns>The <see cref="IRegionManager"/>, for adding several views easily</returns>
        void RegisterViewWithRegion(string regionName, string targetName);

        /// <summary>
        /// Registers a content type with a region name.
        /// </summary>
        /// <param name="regionName">Region name to which the <paramref name="viewType"/> will be registered.</param>
        /// <param name="viewType">Content type to be registered for the <paramref name="regionName"/>.</param>
        void RegisterViewWithRegion(string regionName, Type viewType);

        /// <summary>
        /// Registers a delegate that can be used to retrieve the content associated with a region name.
        /// </summary>
        /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
        /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
        public void RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate);
    }
}
