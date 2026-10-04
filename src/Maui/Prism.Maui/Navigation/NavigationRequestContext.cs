namespace Prism.Navigation;

public record NavigationRequestContext
{
    public bool Cancelled => Result?.Exception is not null && Result.Exception is NavigationException ne && ne.Message == NavigationException.IConfirmNavigationReturnedFalse;
    public NavigationRequestType Type { get; init; }
    public Uri Uri { get; init; }
    public INavigationParameters Parameters { get; init; }
    public INavigationResult Result { get; init; }
    /// <summary>The tabbed page for a <see cref="NavigationRequestType.TabChanged"/> notification; otherwise null.</summary>
    public TabbedPage TabbedPage { get; init; }
    /// <summary>The previously selected child tab, which may be a navigation page; otherwise null.</summary>
    public Page PreviousTab { get; init; }
    /// <summary>The newly selected child tab, which may be a navigation page; otherwise null.</summary>
    public Page CurrentTab { get; init; }
}
