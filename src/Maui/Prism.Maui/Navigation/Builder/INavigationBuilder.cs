namespace Prism.Navigation.Builder;

/// <summary>
/// Represents a builder for configuring navigation in Prism.
/// </summary>
public interface INavigationBuilder
{
    /// <summary>
    /// Gets the URI associated with the navigation.
    /// </summary>
    Uri Uri { get; }

    /// <summary>
    /// Adds a segment to the navigation with the specified segment name and configuration.
    /// </summary>
    /// <param name="segmentName">The name of the segment.</param>
    /// <param name="configureSegment">The configuration action for the segment.</param>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment);

    /// <summary>
    /// Adds a tabbed segment to the navigation with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration action for the tabbed segment.</param>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder AddTabbedSegment(Action<ITabbedSegmentBuilder> configuration);

    /// <summary>
    /// Adds a tabbed segment to the navigation with the specified segment name and configuration.
    /// </summary>
    /// <param name="segmentName">The name of the segment.</param>
    /// <param name="configureSegment">The configuration action for the tabbed segment.</param>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder AddTabbedSegment(string segmentName, Action<ITabbedSegmentBuilder> configureSegment);

    /// <summary>
    /// Sets the navigation parameters for the navigation.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder WithParameters(INavigationParameters parameters);

    /// <summary>
    /// Adds a parameter to the navigation with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder AddParameter(string key, object value);

    /// <summary>
    /// Sets whether to use absolute navigation.
    /// </summary>
    /// <param name="absolute">A flag indicating whether to use absolute navigation.</param>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder UseAbsoluteNavigation(bool absolute);

    /// <summary>
    /// Sets whether to use relative navigation.
    /// </summary>
    /// <returns>The navigation builder.</returns>
    INavigationBuilder UseRelativeNavigation();

    /// <summary>
    /// Navigates back to the specified view asynchronously.
    /// </summary>
    /// <param name="name">The name of the View to navigate back to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<INavigationResult> GoBackToAsync(string name);

    /// <summary>
    /// Navigates to the specified view model asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<INavigationResult> NavigateAsync();

    /// <summary>
    /// Navigates to the specified view model asynchronously and handles any errors with the specified action.
    /// </summary>
    /// <param name="onError">The action to handle errors.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task NavigateAsync(Action<Exception> onError);

    /// <summary>
    /// Navigates to the specified view model asynchronously and executes the specified success and error actions.
    /// </summary>
    /// <param name="onSuccess">The action to execute on success.</param>
    /// <param name="onError">The action to handle errors.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task NavigateAsync(Action onSuccess, Action<Exception> onError);
}
