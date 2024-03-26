using Prism.Common;

namespace Prism.Navigation;

/// <summary>
/// Common extensions for the <see cref="INavigationService"/>
/// </summary>
public static class INavigationServiceExtensions
{
    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService) =>
        navigationService.GoBackAsync(new NavigationParameters());

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, params (string Key, object Value)[] parameters)
    {
        return navigationService.GoBackAsync(GetNavigationParameters(parameters));
    }

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history for the <paramref name="viewName"/>.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="viewName">The name of the View to navigate back to</param>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, string viewName) => navigationService.GoBackAsync(viewName, new NavigationParameters());

    /// <summary>
    /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
    /// </summary>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Only works when called from a View within a NavigationPage</remarks>
    public static Task<INavigationResult> GoBackToRootAsync(this INavigationService navigationService) =>
        navigationService.GoBackToRootAsync(null);

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="uri">The Uri to navigate to</param>
    /// <example>
    /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource));
    /// </example>
    public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri) =>
        navigationService.NavigateAsync(uri, null);

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="uri">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
    /// <example>
    /// NavigateAsync(new Uri("MainPage?id=3&amp;name=dan", UriKind.RelativeSource), ("person", person), ("foo", bar));
    /// </example>
    public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri, params (string Key, object Value)[] parameters)
    {
        return navigationService.NavigateAsync(uri, GetNavigationParameters(parameters));
    }

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="name"/>.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="name">The name of the target to navigate to.</param>
    public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name) =>
        navigationService.NavigateAsync(name, default(INavigationParameters));

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="name"/>.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="name">The name of the target to navigate to.</param>
    /// <param name="parameters">The navigation parameters</param>
    public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name, INavigationParameters parameters)
    {
        if (name.StartsWith(PageNavigationService.RemovePageRelativePath))
            name = name.Replace(PageNavigationService.RemovePageRelativePath, PageNavigationService.RemovePageInstruction);

        return navigationService.NavigateAsync(UriParsingHelper.Parse(name), parameters);
    }

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="name"/>.
    /// </summary>
    /// <param name="navigationService">Service for handling navigation between views</param>
    /// <param name="name">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
    /// <example>
    /// NavigateAsync("MainPage?id=3&amp;name=dan", ("person", person), ("foo", bar));
    /// </example>
    public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, string name, params (string Key, object Value)[] parameters)
    {
        return navigationService.NavigateAsync(name, GetNavigationParameters(parameters));
    }

    /// <summary>
    /// Provides an easy to use way to provide an Error Callback without using await NavigationService
    /// </summary>
    /// <param name="navigationTask">The current Navigation Task</param>
    /// <param name="errorCallback">The <see cref="Exception"/> handler</param>
    public static void OnNavigationError(this Task<INavigationResult> navigationTask, Action<Exception> errorCallback)
    {
        navigationTask.Await(r =>
        {
            if (!r.Success)
                errorCallback?.Invoke(r.Exception);
        });
    }

    /// <summary>
    /// Selects a tab programatically
    /// </summary>
    /// <param name="navigationService"></param>
    /// <param name="tabName">The name of the tab to select</param>
    /// <returns>The <see cref="INavigationResult"/>.</returns>
    public static Task<INavigationResult> SelectTabAsync(this INavigationService navigationService, string tabName) =>
        navigationService.SelectTabAsync(tabName, new NavigationParameters());

    private static INavigationParameters GetNavigationParameters((string Key, object Value)[] parameters)
    {
        var navParams = new NavigationParameters();
        foreach (var (Key, Value) in parameters)
        {
            navParams.Add(Key, Value);
        }
        return navParams;
    }
}
