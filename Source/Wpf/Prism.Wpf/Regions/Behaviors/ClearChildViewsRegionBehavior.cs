

using System;
using System.Windows;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Behavior that removes the RegionManager attached property of all the views in a region once the RegionManager property of a region becomes null.
    /// This is useful when removing views with nested regions, to ensure these nested regions get removed from the RegionManager as well.
    /// <remarks>
    /// This behavior does not apply by default.
    /// In order to activate it, the ClearChildViews attached property must be set to True in the view containing the affected child regions.
    /// </remarks>
    /// </summary>
    public class ClearChildViewsRegionBehavior : RegionBehavior
    {
        /// <summary>
        /// The behavior key.
        /// </summary>
        public const string BehaviorKey = "ClearChildViews";

        /// <summary>
        /// This attached property can be defined on a view to indicate that regions defined in it must be removed from the region manager when the parent view gets removed from a region.
        /// </summary>
        public static readonly DependencyProperty ClearChildViewsProperty =
            DependencyProperty.RegisterAttached("ClearChildViews", typeof(bool), typeof(ClearChildViewsRegionBehavior), new PropertyMetadata(false));

        /// <summary>
        /// Gets the ClearChildViews attached property from a DependencyObject.
        /// </summary>
        /// <param name="target">The object from which to get the value.</param>
        /// <returns>The value of the ClearChildViews attached property in the target specified.</returns>
        public static bool GetClearChildViews(DependencyObject target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return (bool)target.GetValue(ClearChildViewsRegionBehavior.ClearChildViewsProperty);
        }

        /// <summary>
        /// Sets the ClearChildViews attached property in a DependencyObject.
        /// </summary>
        /// <param name="target">The object in which to set the value.</param>
        /// <param name="value">The value of to set in the target object's ClearChildViews attached property.</param>
        public static void SetClearChildViews(DependencyObject target, bool value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.SetValue(ClearChildViewsRegionBehavior.ClearChildViewsProperty, value);
        }

        /// <summary>
        /// Subscribes to the <see cref="Region"/>'s PropertyChanged method to monitor its RegionManager property.
        /// </summary>
        protected override void OnAttach()
        {
            this.Region.PropertyChanged += Region_PropertyChanged;
        }

        private static void ClearChildViews(IRegion region)
        {
            foreach (var view in region.Views)
            {
                DependencyObject dependencyObject = view as DependencyObject;
                if (dependencyObject != null)
                {
                    if (GetClearChildViews(dependencyObject))
                    {
                        dependencyObject.ClearValue(RegionManager.RegionManagerProperty);
                    }
                }
            }
        }

        private void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RegionManager")
            {
                if (this.Region.RegionManager == null)
                {
                    ClearChildViews(this.Region);
                }
            }
        }
    }
}