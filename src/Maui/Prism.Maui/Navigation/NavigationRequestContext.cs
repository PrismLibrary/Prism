namespace Prism.Navigation;

#nullable enable

public record NavigationRequestContext
{
    public bool Cancelled => Result?.Exception is NavigationException ne && ne.Message == NavigationException.IConfirmNavigationReturnedFalse;
    public NavigationRequestType Type { get; init; }
    public Uri? Uri { get; init; }
    public INavigationParameters? Parameters { get; init; }
    public INavigationResult? Result { get; init; }
    /// <summary>The navigated name of the tabbed page for a <see cref="NavigationRequestType.TabChanged"/> notification; otherwise null.</summary>
    public string? TabbedPageName { get; init; }
    /// <summary>The previous tab's navigated name, or navigation-page-name|root-page-name for a navigation-page tab; otherwise null.</summary>
    public string? PreviousTabName { get; init; }
    /// <summary>The current tab's navigated name, or navigation-page-name|root-page-name for a navigation-page tab; otherwise null.</summary>
    public string? CurrentTabName { get; init; }
}
