namespace Prism.Navigation;

public record NavigationRequestContext
{
    public bool Cancelled => Result?.Exception is not null && Result.Exception is NavigationException ne && ne.Message == NavigationException.IConfirmNavigationReturnedFalse;
    public NavigationRequestType Type { get; init; }
    public Uri Uri { get; init; }
    public INavigationParameters Parameters { get; init; }
    public INavigationResult Result { get; init; }
}