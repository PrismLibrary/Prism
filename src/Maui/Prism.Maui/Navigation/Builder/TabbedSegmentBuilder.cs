using Prism.Common;
using Prism.Mvvm;

namespace Prism.Navigation.Builder;

internal class TabbedSegmentBuilder : ITabbedSegmentBuilder, IConfigurableSegmentName, IUriSegment, IRegistryAware
{
    private INavigationParameters _parameters { get; }
    private INavigationBuilder _builder { get; }

    public TabbedSegmentBuilder(INavigationBuilder builder)
    {
        _builder = builder;
        _parameters = new NavigationParameters();

        if (builder is not IRegistryAware registryAware)
            throw new Exception("The builder does not implement IRegistryAware");

        var registrations = registryAware.Registry.ViewsOfType(typeof(TabbedPage));
        if (!registrations.Any())
            throw new NavigationException(NavigationException.NoPageIsRegistered, nameof(TabbedPage));

        var registration = registrations.Last();
        SegmentName = registration.Name;
    }

    public TabbedSegmentBuilder(INavigationBuilder builder, string segmentName)
    {
        _builder = builder;
        _parameters = new NavigationParameters();

        if (builder is not IRegistryAware registryAware)
            throw new Exception("The builder does not implement IRegistryAware");

        var registration = registryAware.Registry.ViewsOfType(typeof(TabbedPage)).FirstOrDefault(x => x.Name == segmentName);
        if (registration == null)
            throw new NavigationException(NavigationException.NoPageIsRegistered, nameof(TabbedPage));

        SegmentName = registration.Name;
    }

    IViewRegistry IRegistryAware.Registry => ((IRegistryAware)_builder).Registry;

    public string SegmentName { get; set; }

    public string Segment => BuildSegment();

    public ITabbedSegmentBuilder AddSegmentParameter(string key, object value)
    {
        _parameters.Add(key, value);
        return this;
    }

    public ITabbedSegmentBuilder UseModalNavigation(bool useModalNavigation)
    {
        return AddSegmentParameter(KnownNavigationParameters.UseModalNavigation, useModalNavigation);
    }

    public ITabbedSegmentBuilder CreateTab(Action<ICreateTabBuilder> configureSegment)
    {
        if (configureSegment is null)
        {
            throw new ArgumentNullException(nameof(configureSegment));
        }

        var builder = new CreateTabBuilder(((IRegistryAware)_builder).Registry);
        configureSegment(builder);
        return AddSegmentParameter(KnownNavigationParameters.CreateTab, builder.Segment);
    }

    public ITabbedSegmentBuilder SelectedTab(string segmentName)
    {
        return AddSegmentParameter(KnownNavigationParameters.SelectedTab, segmentName);
    }

    private string BuildSegment()
    {
        if (!_parameters.Any())
            return SegmentName;

        return SegmentName + _parameters.ToString();
    }
}