namespace Prism.Navigation;

/// <summary>
/// Provides page based navigation for ViewModels.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets the absolute navigation path to the page associated with this service.
    /// </summary>
    /// <returns>The current navigation path, or an empty string before the page is attached to a window.</returns>
    /// <remarks>
    /// Uses URI-escaped registered navigation names and the owning window's current page stack,
    /// including tab selection and modal boundaries. Navigation parameters supplied
    /// by the caller are not retained in the path.
    /// </remarks>
    string GetNavigationUriPath();

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
    Task<INavigationResult> GoBackToAsync(string viewName, INavigationParameters parameters);

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
    /// Removes the pages after the nearest named source view and navigates relative to that view.
    /// </summary>
    /// <param name="viewName">The navigation registration name of the source view to retain.</param>
    /// <param name="uri">The relative route to navigate from the source view.</param>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>The result of the navigation request.</returns>
    /// <remarks>
    /// Searches backward through the active navigation path in the calling page's window, including
    /// modal stacks, the selected tab and the flyout detail. Inactive tabs and the flyout menu are
    /// not searched. Duplicate names resolve to the nearest matching view. A missing source or an
    /// absolute route fails without changing the navigation stack. Navigation from a container
    /// follows the same rules as <see cref="NavigateAsync"/>; it does not implicitly select a tab.
    /// Leading ../ segments in the route navigate backward from the named source before any
    /// forward segments are applied. The active departing page is confirmed before either stack changes.
    /// </remarks>
    Task<INavigationResult> NavigateFromAsync(string viewName, Uri uri, INavigationParameters parameters);

    /// <summary>
    /// Selects a Tab of the TabbedPage parent and Navigates to a specified Uri
    /// </summary>
    /// <param name="name">The name of the tab to select</param>
    /// <param name="uri">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    Task<INavigationResult> SelectTabAsync(string name, Uri uri, INavigationParameters parameters);
}
