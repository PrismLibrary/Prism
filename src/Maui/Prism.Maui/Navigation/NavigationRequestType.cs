namespace Prism.Navigation;

/// <summary>Identifies the page navigation operation reported to navigation observers.</summary>
public enum NavigationRequestType
{
    /// <summary>Navigation to a requested URI.</summary>
    Navigate,
    /// <summary>Navigation back to the preceding page.</summary>
    GoBack,
    /// <summary>Navigation back to the root of a navigation page.</summary>
    GoToRoot,
}
