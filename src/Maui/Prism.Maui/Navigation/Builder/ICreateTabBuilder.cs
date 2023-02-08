namespace Prism.Navigation.Builder;

public interface ICreateTabBuilder
{
    /// <summary>
    /// Adds a Segment for the <see cref="TabbedPage"/>
    /// </summary>
    /// <param name="segmentName"></param>
    /// <param name="configureSegment"></param>
    /// <returns></returns>
    ICreateTabBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment);
}
