using System;
using System.Collections.Specialized;
using Prism.Common;
using Prism.Navigation;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Calls <see cref="IDestructible.Destroy"/> on Views and ViewModels
    /// removed from the <see cref="IRegion.Views"/> collection.
    /// </summary>
    /// <remarks>
    /// The View and/or ViewModels must implement <see cref="IDestructible"/> for this behavior to work.
    /// </remarks>
    public class DestructibleRegionBehavior : RegionBehavior
    {
        /// <summary>
        /// The key of this behavior.
        /// </summary>
        public const string BehaviorKey = "IDestructibleRegionBehavior";

        /// <summary>
        /// Attaches the <see cref="DestructibleRegionBehavior"/> to the <see cref="IRegion.Views"/> collection.
        /// </summary>
        protected override void OnAttach()
        {
            Region.Views.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    Action<IDestructible> invocation = destructible => destructible.Destroy();
                    MvvmHelpers.ViewAndViewModelAction(item, invocation);
                }
            }
        }
    }
}
