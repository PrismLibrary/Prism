using Prism.Regions.Behaviors;
using System;
using System.Windows.Controls.Primitives;

namespace Prism.Regions
{
    /// <summary>
    /// Adapter that creates a new <see cref="Region"/> and binds all
    /// the views to the adapted <see cref="Selector"/>.
    /// It also keeps the <see cref="IRegion.ActiveViews"/> and the selected items
    /// of the <see cref="Selector"/> in sync.
    /// </summary>
    public class SelectorRegionAdapter : RegionAdapterBase<Selector>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SelectorRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public SelectorRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts an <see cref="Selector"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, Selector regionTarget)
        {
        }

        /// <summary>
        /// Attach new behaviors.
        /// </summary>
        /// <param name="region">The region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        /// <remarks>
        /// This class attaches the base behaviors and also listens for changes in the
        /// activity of the region or the control selection and keeps the in sync.
        /// </remarks>
        protected override void AttachBehaviors(IRegion region, Selector regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            // Add the behavior that syncs the items source items with the rest of the items
            region.Behaviors.Add(SelectorItemsSourceSyncBehavior.BehaviorKey, new SelectorItemsSourceSyncBehavior()
                                                                                  {
                                                                                      HostControl = regionTarget
                                                                                  });

            base.AttachBehaviors(region, regionTarget);
        }

        /// <summary>
        /// Creates a new instance of <see cref="Region"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Region"/>.</returns>
        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}