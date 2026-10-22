using System.Collections.Specialized;

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
        void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (region.NavigationService.Journal.CanGoBack)
                region.NavigationService.Journal.GoBack();
        }

        void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (regionTarget.SelectedItem is FrameworkElement item && item.Tag is string target && !string.IsNullOrEmpty(target))
                region.RequestNavigate(target);
        }

        void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            regionTarget.Content = region.ActiveViews.FirstOrDefault();
        }

        regionTarget.BackRequested += OnBackRequested;
        regionTarget.SelectionChanged += OnSelectionChanged;
        region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
#if WINUI && !UNO_WINUI
        NativeRegionScope.RegisterCleanup(region, () =>
        {
            regionTarget.BackRequested -= OnBackRequested;
            regionTarget.SelectionChanged -= OnSelectionChanged;
            region.ActiveViews.CollectionChanged -= OnActiveViewsChanged;
            regionTarget.Content = null;
        });
#endif
    }

    /// <inheritdoc/>
    protected override IRegion CreateRegion() => new SingleActiveRegion();
}
