#if (WPF || AVALONIA)

// NOTE: This is for Legacy support for WPF/Avalonia apps only
namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides a way for objects involved in navigation to be notified of navigation activities.
    /// </summary>
    /// <remarks>
    /// Provides compatibility for Legacy Prism.Wpf and Prism.Avalonia apps.
    /// </remarks>
    public interface INavigationAware : IRegionAware
    {
    }
}
#endif
