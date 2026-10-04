namespace Prism.Navigation;

public enum NavigationRequestType
{
    Navigate,
    GoBack,
    GoToRoot,
    /// <summary>The selected tab changed; no navigation-service request is implied.</summary>
    TabChanged,
}
