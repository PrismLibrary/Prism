namespace Prism.Navigation.Regions;

public sealed class NavigationViewRegionAdapter : RegionAdapterBase<NavigationView>
{
    public NavigationViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }

    protected override void Adapt(IRegion region, NavigationView regionTarget)
    {
        regionTarget.BackRequested += delegate
        {
            if (region.NavigationService.Journal.CanGoBack)
                region.NavigationService.Journal.GoBack();
        };

        regionTarget.SelectionChanged += delegate
        {
            if (regionTarget.SelectedItem is FrameworkElement item && item.Tag is string navigationTarget && !string.IsNullOrEmpty(navigationTarget))
                region.RequestNavigate(navigationTarget);
        };

        region.ActiveViews.CollectionChanged += delegate
        {
            regionTarget.Content = region.ActiveViews.FirstOrDefault();
        };
    }

    protected override IRegion CreateRegion()
    {
        return new SingleActiveRegion();
    }
}
