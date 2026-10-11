#if (WPF || AVALONIA)

// NOTE: This is for Legacy support for WPF/Avalonia apps only
namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides a way for objects involved in navigation to be notified of navigation activities.
    /// </summary>
    /// <remarks>
    /// Provides the legacy navigation contract as an alias for IRegionAware.
    /// </remarks>
    public interface INavigationAware : IRegionAware
    {
    }
}
#endif
