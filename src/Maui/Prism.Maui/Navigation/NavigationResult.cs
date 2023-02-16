namespace Prism.Navigation;

public record NavigationResult : INavigationResult
{
    public bool Success => Exception is null;

    public bool Cancelled =>
        Exception is NavigationException navigationException
        && navigationException.Message == NavigationException.IConfirmNavigationReturnedFalse;

    public Exception Exception { get; init; }
}
