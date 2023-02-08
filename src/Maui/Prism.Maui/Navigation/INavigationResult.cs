namespace Prism.Navigation;

public interface INavigationResult
{
    bool Success => Exception is null;

    bool Cancelled =>
        Exception is NavigationException navigationException
        && navigationException.Message == NavigationException.IConfirmNavigationReturnedFalse;

    Exception Exception { get; }
}
