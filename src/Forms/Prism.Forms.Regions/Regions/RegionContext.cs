using System;
using Prism.Common;
using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions
{
    /// <summary>
    /// Class that holds methods to Set and Get the RegionContext from a BindableObject.
    ///
    /// RegionContext allows sharing of contextual information between the view that's hosting a <see cref="IRegion"/>
    /// and any views that are inside the Region.
    /// </summary>
    public static class RegionContext
    {
        private static readonly BindableProperty ObservableRegionContextProperty =
            BindableProperty.CreateAttached("ObservableRegionContext", typeof(ObservableObject<object>), typeof(RegionContext), null);

        /// <summary>
        /// Returns an <see cref="ObservableObject{T}"/> wrapper around the RegionContext value. The RegionContext
        /// will be set on any views (BindableObject's) that are inside the <see cref="IRegion.Views"/> collection by
        /// the <see cref="BindRegionContextToVisualElementBehavior"/> Behavior.
        /// The RegionContext will also be set to the control that hosts the Region, by the <see cref="SyncRegionContextWithHostBehavior"/> Behavior.
        ///
        /// If the <see cref="ObservableObject{T}"/> wrapper does not already exist, an empty one will be created. This way, an observer can
        /// notify when the value is set for the first time.
        /// </summary>
        /// <param name="view">Any view that hold the RegionContext value. </param>
        /// <returns>Wrapper around the <see cref="RegionContext"/> value. </returns>
        public static ObservableObject<object> GetObservableContext(VisualElement view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (!(view.GetValue(ObservableRegionContextProperty) is ObservableObject<object> context))
            {
                context = new ObservableObject<object>();
                view.SetValue(ObservableRegionContextProperty, context);
            }

            return context;
        }
    }
}
