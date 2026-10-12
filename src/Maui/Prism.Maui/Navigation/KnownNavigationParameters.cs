namespace Prism.Navigation;

/// <summary>Defines parameter keys that control Prism page navigation and tab creation.</summary>
public static class KnownNavigationParameters
{
    /// <summary>
    /// Identifies a route for a tab to create when navigating to a <see cref="TabbedPage"/>.
    /// </summary>
    /// <remarks>
    /// Repeat this parameter to create multiple tabs. A route may identify a single page or
    /// a navigation page containing child pages, for example <c>NavigationPage|Details</c>.
    /// Page names must match their navigation registrations.
    /// </remarks>
    public const string CreateTab = "createTab";

    /// <summary>
    /// Identifies the existing tab to select when navigating to a <see cref="TabbedPage"/>.
    /// </summary>
    /// <remarks>
    /// Use a child page's registered navigation name, or its CLR type name for an unregistered
    /// child. For a tab wrapped in a navigation page, use the navigation page name followed by
    /// the root or current child page name, separated by <c>|</c>.
    /// </remarks>
    public const string SelectedTab = "selectedTab";
    
    /// <summary>
    /// Identifies the title to assign to the <see cref="TabbedPage"/> being created.
    /// </summary>
    public const string Title = "title";

    /// <summary>
    /// Identifies whether a navigation operation uses the modal stack.
    /// </summary>
    /// <remarks>
    /// A value of <see langword="true"/> requests modal navigation; <see langword="false"/>
    /// requests navigation within a navigation page. When omitted, the navigation service
    /// determines the stack from the current page and navigation context.
    /// </remarks>
    public const string UseModalNavigation = "useModalNavigation";

    /// <summary>
    /// Identifies whether a navigation operation should be animated.
    /// </summary>
    /// <remarks>Navigation is animated by default. Set this parameter to <see langword="false"/> to disable animation.</remarks>
    public const string Animated = "animated";

    /// <summary>
    /// Identifies the command parameter supplied to a XAML navigation command.
    /// </summary>
    /// <remarks>
    /// A non-null command parameter that is not an <see cref="INavigationParameters"/>
    /// collection or a <see cref="Prism.Xaml.Parameter"/> or <see cref="Prism.Xaml.Parameters"/>
    /// object is stored under this key.
    /// </remarks>
    public const string XamlParam = "xamlParam";
}
