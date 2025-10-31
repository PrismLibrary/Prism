using System.Collections.Specialized;
using Prism.Common;

namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Provides a Behavior to Destroy the View/ViewModel when the View is removed from the Region's Views
/// </summary>
public class DestructibleRegionBehavior : RegionBehavior
{
    /// <summary>
    /// The key of this behavior.
    /// </summary>
    public const string BehaviorKey = "IDestructibleRegionBehavior";

    /// <summary>
    /// Attaches the behavior to the specified region.
    /// </summary>
    protected override void OnAttach()
    {
        Region.Views.CollectionChanged += Views_CollectionChanged;
    }

    /// <summary>
    /// Detaches the behavior from the region and unsubscribes from events.
    /// </summary>
    protected override void OnDetach()
    {
        if (Region?.Views != null)
        {
            Region.Views.CollectionChanged -= Views_CollectionChanged;
        }

        base.OnDetach();
    }

    private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems)
            {
                MvvmHelpers.InvokeViewAndViewModelAction<IDestructible>(item, v => v.Destroy());
            }
        }
    }
}
