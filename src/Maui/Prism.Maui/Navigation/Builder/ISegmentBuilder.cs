namespace Prism.Navigation.Builder;

public interface ISegmentBuilder
{
    /// <summary>
    /// Gets the Segment Name like `ViewA`
    /// </summary>
    string SegmentName { get; }

    /// <summary>
    /// Adds a Segment Parameter. This will append the generated URI query parameters NOT the
    /// <see cref="INavigationParameters"/> that are passed to every page.
    /// </summary>
    /// <param name="key">The Query Parameter key.</param>
    /// <param name="value">The Query Parameter value.</param>
    /// <returns>The <see cref="ISegmentBuilder"/>.</returns>
    ISegmentBuilder AddParameter(string key, object value);

    /// <summary>
    /// Specifies whether to force Modal Navigation for the current Navigation Segment
    /// </summary>
    /// <param name="useModalNavigation">If <see langword="true"/> the NavigationService will force Modal Navigation for the Navigation Segment. Modal Navigation may be the default following this segment if it is not a NavigationPage.</param>
    /// <returns>The <see cref="ISegmentBuilder"/>.</returns>
    ISegmentBuilder UseModalNavigation(bool useModalNavigation);
}
