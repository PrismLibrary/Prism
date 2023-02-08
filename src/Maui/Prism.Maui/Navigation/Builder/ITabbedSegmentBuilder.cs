namespace Prism.Navigation.Builder;

public interface ITabbedSegmentBuilder
{
    ITabbedSegmentBuilder CreateTab(Action<ICreateTabBuilder> configureSegment);

    ITabbedSegmentBuilder SelectedTab(string segmentName);

    ITabbedSegmentBuilder AddSegmentParameter(string key, object value);

    ITabbedSegmentBuilder UseModalNavigation(bool useModalNavigation);
}
