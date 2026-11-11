using Microsoft.Maui.Controls.Compatibility;
using Prism.Properties;

namespace Prism.Navigation.Regions.Adapters;

/// <summary>
/// Adapter that creates a new <see cref="Region"/> and monitors its
/// active view to set it on the adapted <see cref="Layout{View}"/>.
/// </summary>
// Preserve support for existing compatibility layouts alongside LayoutRegionAdapter.
#pragma warning disable CS0618 // Layout<View> is the supported legacy adapter target.
public class LayoutViewRegionAdapter : RegionAdapterBase<Layout<View>>
#pragma warning restore CS0618
{
    /// <summary>
    /// Initializes a new instance of <see cref="LayoutViewRegionAdapter"/>.
    /// </summary>
    /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
    public LayoutViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }

    /// <summary>
    /// Adapts a <see cref="Layout{View}"/> to an <see cref="IRegion"/>.
    /// </summary>
    /// <param name="region">The new region being used.</param>
    /// <param name="regionTarget">The object to adapt.</param>
#pragma warning disable CS0618 // Required override for the legacy adapter target.
    protected override void Adapt(IRegion region, Layout<View> regionTarget)
#pragma warning restore CS0618
    {
        ArgumentNullException.ThrowIfNull(region);
        ArgumentNullException.ThrowIfNull(regionTarget);

        bool itemsSourceIsSet = regionTarget.Children?.Any() ?? false || regionTarget.IsSet(BindableLayout.ItemsSourceProperty);

        if (itemsSourceIsSet)
        {
            throw new InvalidOperationException(Resources.LayoutViewHasChildrenException);
        }

        BindableLayout.SetItemsSource(regionTarget, region.Views);
        BindableLayout.SetItemTemplate(regionTarget, new RegionItemsSourceTemplate());
    }

    /// <summary>
    /// Creates a new instance of <see cref="IRegion"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="Region"/>.</returns>
    protected override IRegion CreateRegion(IContainerProvider container) =>
        container.Resolve<Region>();
}
