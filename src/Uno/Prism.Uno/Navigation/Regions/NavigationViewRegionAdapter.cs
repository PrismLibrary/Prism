namespace Prism.Navigation.Regions;

/// <summary>Connects a navigation view's selection, back button, and content to a single active region.</summary>
/// <remarks>Navigation items identify their navigation target with a nonempty string in their Tag property.</remarks>
public sealed class NavigationViewRegionAdapter : RegionAdapterBase<NavigationView>
{
    /// <summary>Initializes the adapter with the factory for region behaviors.</summary>
    /// <param name="regionBehaviorFactory">The factory used to attach default behaviors to the region.</param>
    public NavigationViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override IRegion CreateRegion()
    {
        return new SingleActiveRegion();
    }
}
