namespace Prism.Navigation;

/// <summary>
/// Provides a way for ViewModels involved in navigation to initialize.
/// </summary>
public interface IInitialize
{
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    void Initialize(INavigationParameters parameters);
}
