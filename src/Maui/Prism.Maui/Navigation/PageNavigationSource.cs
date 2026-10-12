namespace Prism.Navigation;

/// <summary>Identifies the source of a page navigation operation.</summary>
public enum PageNavigationSource
{
    /// <summary>The operation was initiated through Prism's navigation service.</summary>
    NavigationService,
    /// <summary>The operation was initiated by platform navigation, such as a back gesture.</summary>
    Device,
    /// <summary>The operation was initiated by Prism's dialog service.</summary>
    DialogService
}
