using System;
using System.Collections.Specialized;
using System.Windows;
using Prism.Regions;

namespace Prism.Regions.Behaviors
{
    public class RegionManagerAwareBehavior : RegionBehavior
    {
        public const string BehaviorKey = "RegionManagerAwareBehavior";
        protected override void OnAttach()
        {
            Region.Views.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    IRegionManager regionManager = Region.RegionManager;
                    FrameworkElement frameworkElement = item as FrameworkElement;
                    if (frameworkElement?.GetValue(RegionManager.RegionManagerProperty) is IRegionManager scopedRegionManager)
                        regionManager = scopedRegionManager;

                    SetScopedRegionManager(item, x => x.RegionManager = regionManager);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    SetScopedRegionManager(item, x => x.RegionManager = null);
                }
            }
        }

        private void SetScopedRegionManager(object view, Action<IRegionManagerAware> action)
        {
            if (view is IRegionManagerAware regionManagerAware)
                action(regionManagerAware);

            FrameworkElement frameworkElement = view as FrameworkElement;
            if (frameworkElement?.DataContext is IRegionManagerAware dataContext)
            {
                FrameworkElement parent = frameworkElement.Parent as FrameworkElement;

                if (parent?.DataContext is IRegionManagerAware parantDataContext)
                {
                    if (parantDataContext == dataContext)
                        return;
                }

                action(dataContext);
            }
        }
    }
}