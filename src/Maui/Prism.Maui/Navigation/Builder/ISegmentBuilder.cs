namespace Prism.Navigation.Builder;

/// <summary>
/// Represents a builder for constructing navigation segments.
/// </summary>
public interface ISegmentBuilder
{
    /// <summary>
    /// Gets the segment name, such as `ViewA`.
    /// </summary>
    string SegmentName { get; }

    /// <summary>
    /// Adds a segment parameter. This will append the generated URI query parameters, not the <see cref="INavigationParameters"/> that are passed to every page.
    /// </summary>
    /// <param name="key">The query parameter key.</param>
    /// <param name="value">The query parameter value.</param>
    /// <returns>The <see cref="ISegmentBuilder"/>.</returns>
    ISegmentBuilder AddParameter(string key, object value);

    /// <summary>
    /// Specifies whether to force modal navigation for the current navigation segment.
    /// </summary>
    /// <param name="useModalNavigation">If <see langword="true"/>, the NavigationService will force modal navigation for the navigation segment. Modal navigation may be the default following this segment if it is not a NavigationPage.</param>
    /// <returns>The <see cref="ISegmentBuilder"/>.</returns>
    ISegmentBuilder UseModalNavigation(bool useModalNavigation);
}
