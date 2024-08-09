namespace Prism.Navigation;

/// <summary>
/// Provides a way for ViewModels involved in navigation to asynchronously initialize.
/// </summary>
public interface IInitializeAsync
{
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    Task InitializeAsync(INavigationParameters parameters);
}
