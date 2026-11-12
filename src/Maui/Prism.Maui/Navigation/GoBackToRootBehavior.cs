namespace Prism.Navigation;

/// <summary>Controls the destination of a GoBackToRootAsync request.</summary>
public enum GoBackToRootBehavior
{
    /// <summary>Pops the caller's NavigationPage to its root. From a plain modal, dismisses all modals and preserves the window's active navigation history.</summary>
    Default,

    /// <summary>Dismisses all modals and reveals the current content of the window, preserving navigation history and tab selection.</summary>
    WindowContent,

    /// <summary>Dismisses all modals and pops the window's active NavigationPage to its root, preserving tab selection and inactive histories.</summary>
    WindowNavigationRoot
}
