namespace Prism.Navigation;

/// <summary>Describes a completed page navigation request for navigation observers.</summary>
public record NavigationRequestContext
{
    /// <summary>Gets whether navigation was refused by a navigation confirmation callback.</summary>
    /// <remarks>Other failures are reported by <see cref="Result"/> and do not count as cancellation.</remarks>
    public bool Cancelled => Result?.Exception is not null && Result.Exception is NavigationException ne && ne.Message == NavigationException.IConfirmNavigationReturnedFalse;
    /// <summary>Gets the operation requested by the caller.</summary>
    public NavigationRequestType Type { get; init; }
    /// <summary>Gets the requested route, when the operation specifies a URI.</summary>
    public Uri Uri { get; init; }
    /// <summary>Gets the parameters supplied to the navigation operation.</summary>
    public INavigationParameters Parameters { get; init; }
    /// <summary>Gets the outcome of the navigation operation, including any failure.</summary>
    public INavigationResult Result { get; init; }
}
