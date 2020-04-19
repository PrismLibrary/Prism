using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Populates the target region with the views registered to it in the <see cref="IRegionViewRegistry"/>.
    /// </summary>
    public class AutoPopulateRegionBehavior : RegionBehavior
    {
        /// <summary>
        /// The key of this behavior.
        /// </summary>
        public const string BehaviorKey = "AutoPopulate";

        private readonly IRegionViewRegistry regionViewRegistry;

        /// <summary>
        /// Creates a new instance of the AutoPopulateRegionBehavior
        /// associated with the <see cref="IRegionViewRegistry"/> received.
        /// </summary>
        /// <param name="regionViewRegistry"><see cref="IRegionViewRegistry"/> that the behavior will monitor for views to populate the region.</param>
        public AutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }

        /// <summary>
        /// Attaches the AutoPopulateRegionBehavior to the Region.
        /// </summary>
        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(Region.Name))
            {
                Region.PropertyChanged += Region_PropertyChanged;
            }
            else
            {
                StartPopulatingContent();
            }
        }

        private void StartPopulatingContent()
        {
            foreach (VisualElement view in CreateViewsToAutoPopulate())
            {
                AddViewIntoRegion(view);
            }

            regionViewRegistry.ContentRegistered += OnViewRegistered;
        }

        /// <summary>
        /// Returns a collection of views that will be added to the
        /// View collection.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<object> CreateViewsToAutoPopulate()
        {
            return regionViewRegistry.GetContents(Region.Name);
        }

        /// <summary>
        /// Adds a view into the views collection of this region.
        /// </summary>
        /// <param name="viewToAdd"></param>
        protected virtual void AddViewIntoRegion(VisualElement viewToAdd)
        {
            Region.Add(viewToAdd);
        }

        private void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(Region.Name))
            {
                Region.PropertyChanged -= Region_PropertyChanged;
                StartPopulatingContent();
            }
        }

        /// <summary>
        /// Handler of the event that fires when a new viewtype is registered to the registry.
        /// </summary>
        /// <remarks>Although this is a public method to support Weak Delegates in Silverlight, it should not be called by the user.</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnViewRegistered(object sender, ViewRegisteredEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.RegionName == Region.Name)
            {
                AddViewIntoRegion((VisualElement)e.GetView());
            }
        }
    }
}
