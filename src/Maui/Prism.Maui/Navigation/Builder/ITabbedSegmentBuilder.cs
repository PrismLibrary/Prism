namespace Prism.Navigation.Builder;

/// <summary>
/// Represents a builder for configuring tabbed segments in Prism navigation.
/// </summary>
public interface ITabbedSegmentBuilder
{
    /// <summary>
    /// Adds a route describing a tab to create when the tabbed page is navigated to.
    /// </summary>
    /// <param name="configureSegment">The callback that adds the tab's page segments and parameters.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configureSegment"/> is null.</exception>
    /// <remarks>Call this method more than once to create multiple tabs. A tab can contain a single page or a navigation page with child pages.</remarks>
    ITabbedSegmentBuilder CreateTab(Action<ICreateTabBuilder> configureSegment);

    /// <summary>
    /// Specifies the existing tab to select when the navigation route is applied.
    /// </summary>
    /// <param name="segmentName">The tab's navigation name, or a navigation page and child page name separated by <c>|</c>.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder SelectedTab(string segmentName);
    
    /// <summary>
    /// Specifies the title to assign to the tabbed page when it is created.
    /// </summary>
    /// <param name="title">The title of the tabbed page.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder Title(string title);

    /// <summary>
    /// Adds a query parameter to the tabbed page's navigation segment.
    /// </summary>
    /// <param name="key">The parameter name to include in the tabbed page's route.</param>
    /// <param name="value">The value associated with the parameter.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder AddSegmentParameter(string key, object value);

    /// <summary>
    /// Specifies whether the tabbed page segment uses modal navigation.
    /// </summary>
    /// <param name="useModalNavigation"><see langword="true"/> to request the modal stack; <see langword="false"/> to request the navigation page stack.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder UseModalNavigation(bool useModalNavigation);
}
