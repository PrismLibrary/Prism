#if WPF

// NOTE: This is for Legacy support for WPF apps only
namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides a way for objects involved in navigation to be notified of navigation activities.
    /// </summary>
    /// <remarks>
    /// Provides compatibility for Legacy Prism.Wpf apps.
    /// </remarks>
    public interface INavigationAware : IRegionAware
    {
    }
}
#endif
