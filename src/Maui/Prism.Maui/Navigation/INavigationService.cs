namespace Prism.Navigation;

/// <summary>
/// Provides page based navigation for ViewModels.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    Task<INavigationResult> GoBackAsync(INavigationParameters parameters);

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history for the <paramref name="viewName"/>.
    /// </summary>
    /// <param name="viewName">The name of the View to navigate back to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    Task<INavigationResult> GoBackAsync(string viewName, INavigationParameters parameters);

    /// <summary>
    /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Only works when called from a View within a NavigationPage</remarks>
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters);

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
    /// <example>
    /// NavigateAsync(new Uri("MainPage?id=3&amp;name=Brian", UriKind.RelativeSource), parameters);
    /// </example>
    Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters);

    /// <summary>
    /// Selects a Tab of the TabbedPage parent.
    /// </summary>
    /// <param name="name">The name of the tab to select</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    Task<INavigationResult> SelectTabAsync(string name, INavigationParameters parameters);
}
