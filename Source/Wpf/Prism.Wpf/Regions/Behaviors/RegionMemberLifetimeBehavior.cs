

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using Prism.Common;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// The RegionMemberLifetimeBehavior determines if items should be removed from the <see cref="IRegion"/>
    /// when they are deactivated.
    /// </summary>
    /// <remarks>
    /// The <see cref="RegionMemberLifetimeBehavior"/> monitors the <see cref="IRegion.ActiveViews"/>
    /// collection to discover items that transition into a deactivated state.  
    /// <p/>
    /// The behavior checks the removed items for either the <see cref="IRegionMemberLifetime"/>
    /// or the <see cref="RegionMemberLifetimeAttribute"/> (in that order) to determine if it should be kept 
    /// alive on removal.
    /// <p/>
    /// If the item in the collection is a <see cref="System.Windows.FrameworkElement"/>, it will
    /// also check it's DataContext for <see cref="IRegionMemberLifetime"/> or the <see cref="RegionMemberLifetimeAttribute"/>.
    /// <p/>
    /// The order of checks are:
    /// <list type="number">
    ///     <item>Region Item's IRegionMemberLifetime.KeepAlive value.</item>
    ///     <item>Region Item's DataContext's IRegionMemberLifetime.KeepAlive value.</item>
    ///     <item>Region Item's RegionMemberLifetimeAttribute.KeepAlive value.</item>
    ///     <item>Region Item's DataContext's RegionMemberLifetimeAttribute.KeepAlive value.</item>
    /// </list>
    /// </remarks>
    public class RegionMemberLifetimeBehavior : RegionBehavior
    {
        /// <summary>
        /// The key for this behavior.
        /// </summary>
        public const string BehaviorKey = "RegionMemberLifetimeBehavior";

        /// <summary>
        /// Override this method to perform the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            this.Region.ActiveViews.CollectionChanged += this.OnActiveViewsChanged;
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // We only pay attention to items removed from the ActiveViews list.
            // Thus, we expect that any ICollectionView implementation would
            // always raise a remove and we don't handle any resets
            // unless we wanted to start tracking views that used to be active.
            if (e.Action != NotifyCollectionChangedAction.Remove) return;

            var inactiveViews = e.OldItems;
            foreach (var inactiveView in inactiveViews)
            {
                if (!ShouldKeepAlive(inactiveView))
                {
                    if (Region.Views.Contains(inactiveView))
                        Region.Remove(inactiveView);
                }
            }
        }

        private static bool ShouldKeepAlive(object inactiveView)
        {
            IRegionMemberLifetime lifetime = MvvmHelpers.GetImplementerFromViewOrViewModel<IRegionMemberLifetime>(inactiveView);
            if (lifetime != null)
            {
                return lifetime.KeepAlive;
            }

            RegionMemberLifetimeAttribute lifetimeAttribute = GetItemOrContextLifetimeAttribute(inactiveView);
            if (lifetimeAttribute != null)
            {
                return lifetimeAttribute.KeepAlive;
            }

            return true;
        }

        private static RegionMemberLifetimeAttribute GetItemOrContextLifetimeAttribute(object inactiveView)
        {
            var lifetimeAttribute = GetCustomAttributes<RegionMemberLifetimeAttribute>(inactiveView.GetType()).FirstOrDefault();
            if (lifetimeAttribute != null)
            {
                return lifetimeAttribute;
            }

            var frameworkElement = inactiveView as System.Windows.FrameworkElement;
            if (frameworkElement != null && frameworkElement.DataContext != null)
            {
                var dataContext = frameworkElement.DataContext;
                var contextLifetimeAttribute =
                    GetCustomAttributes<RegionMemberLifetimeAttribute>(dataContext.GetType()).FirstOrDefault();
                return contextLifetimeAttribute;
            }

            return null;
        }

        private static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }
}
