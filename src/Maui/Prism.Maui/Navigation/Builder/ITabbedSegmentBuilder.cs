namespace Prism.Navigation.Builder;

/// <summary>
/// Represents a builder for configuring tabbed segments in Prism navigation.
/// </summary>
public interface ITabbedSegmentBuilder
{
    /// <summary>
    /// Creates a new tab with the specified configuration.
    /// </summary>
    /// <param name="configureSegment">The action to configure the tab.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder CreateTab(Action<ICreateTabBuilder> configureSegment);

    /// <summary>
    /// Sets the selected tab by its name.
    /// </summary>
    /// <param name="segmentName">The name of the tab to select.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder SelectedTab(string segmentName);

    /// <summary>
    /// Adds a parameter to the current tab segment.
    /// </summary>
    /// <param name="key">The key of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder AddSegmentParameter(string key, object value);

    /// <summary>
    /// Sets whether to use modal navigation for the current tab segment.
    /// </summary>
    /// <param name="useModalNavigation">A flag indicating whether to use modal navigation.</param>
    /// <returns>The current instance of the <see cref="ITabbedSegmentBuilder"/>.</returns>
    ITabbedSegmentBuilder UseModalNavigation(bool useModalNavigation);
}
