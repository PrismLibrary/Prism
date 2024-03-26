namespace Prism.Navigation.Builder;

public interface INavigationBuilder
{
    Uri Uri { get; }
    INavigationBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment);
    INavigationBuilder AddTabbedSegment(Action<ITabbedSegmentBuilder> configuration);
    INavigationBuilder AddTabbedSegment(string segmentName, Action<ITabbedSegmentBuilder> configureSegment);
    INavigationBuilder WithParameters(INavigationParameters parameters);
    INavigationBuilder AddParameter(string key, object value);

    INavigationBuilder UseAbsoluteNavigation(bool absolute);
    INavigationBuilder UseRelativeNavigation();

    Task<INavigationResult> GoBackAsync<TViewModel>();
    Task<INavigationResult> NavigateAsync();
    Task NavigateAsync(Action<Exception> onError);
    Task NavigateAsync(Action onSuccess, Action<Exception> onError);
}
