using Prism.Ioc;

namespace Prism.Regions;

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
    /// <returns>Collection of contents associated with the <paramref name="regionName"/>.</returns>
    IEnumerable<object> GetContents(string regionName, IContainerProvider container);

    /// <summary>
    /// Registers a content type with a region name.
    /// </summary>
    /// <param name="regionName">Region name to which the <paramref name="viewType"/> will be registered.</param>
    /// <param name="viewType">Content type to be registered for the <paramref name="regionName"/>.</param>
    void RegisterViewWithRegion(string regionName, Type viewType);

    /// <summary>
    /// Registers a content type with a region name.
    /// </summary>
    /// <param name="regionName">Region name to which the <paramref name="targetName"/> will be registered.</param>
    /// <param name="targetName">Content type to be registered for the <paramref name="regionName"/>.</param>
    void RegisterViewWithRegion(string regionName, string targetName);

    /// <summary>
    /// Registers a delegate that can be used to retrieve the content associated with a region name. 
    /// </summary>
    /// <param name="regionName">Region name to which the <paramref name="getContentDelegate"/> will be registered.</param>
    /// <param name="getContentDelegate">Delegate used to retrieve the content associated with the <paramref name="regionName"/>.</param>
    void RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate);
}
