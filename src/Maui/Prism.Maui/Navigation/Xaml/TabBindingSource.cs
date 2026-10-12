namespace Prism.Navigation.Xaml;

/// <summary>Identifies the page whose binding context supplies bindings for a tab.</summary>
public enum TabBindingSource
{
    /// <summary>Uses the root page of the tab's navigation stack.</summary>
    RootPage,
    /// <summary>Uses the currently displayed page of the tab's navigation stack.</summary>
    CurrentPage
}
