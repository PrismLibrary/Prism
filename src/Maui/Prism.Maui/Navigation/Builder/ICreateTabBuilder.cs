namespace Prism.Navigation.Builder;

/// <summary>Builds a route for a tab created within a tabbed page.</summary>
public interface ICreateTabBuilder
{
    /// <summary>
    /// Adds a page segment to the tab's navigation route.
    /// </summary>
    /// <param name="segmentName">The registered navigation name of the page to add.</param>
    /// <param name="configureSegment">The callback configuring the segment's parameters and options.</param>
    /// <returns>The updated tab route builder.</returns>
    ICreateTabBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment);
}
