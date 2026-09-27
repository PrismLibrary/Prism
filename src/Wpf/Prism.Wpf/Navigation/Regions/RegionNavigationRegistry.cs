using Prism.Mvvm;

namespace Prism.Navigation.Regions;

/// <summary>
/// Manages the views registered for region navigation.
/// </summary>
public class RegionNavigationRegistry : ViewRegistryBase, IRegionNavigationRegistry
{
    /// <summary>
    /// Initializes the region navigation registry.
    /// </summary>
    /// <param name="registrations">The available view registrations.</param>
    public RegionNavigationRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Region, registrations)
    {
    }
}
