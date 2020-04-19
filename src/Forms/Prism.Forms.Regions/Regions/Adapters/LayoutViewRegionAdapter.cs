using System;
using System.Linq;
using Prism.Properties;
using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// Adapter that creates a new <see cref="AllActiveRegion"/> and monitors its
    /// active view to set it on the adapted <see cref="Layout{View}"/>.
    /// </summary>
    public class LayoutViewRegionAdapter : RegionAdapterBase<Layout<View>>
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
        protected override void Adapt(IRegion region, Layout<View> regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            bool itemsSourceIsSet = regionTarget.Children?.Any() ?? false;

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.LayoutViewHasChildrenException);
            }

            // If control has child items, move them to the region and then bind control to region. Can't set ItemsSource if child items exist.
            //if (regionTarget.Items.Count > 0)
            //{
            //    foreach (object childItem in regionTarget.Items)
            //    {
            //        region.Add(childItem);
            //    }
            //    // Control must be empty before setting ItemsSource
            //    regionTarget.Items.Clear();
            //}

            //regionTarget.ItemsSource = region.Views;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AllActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="AllActiveRegion"/>.</returns>
        protected override IRegion CreateRegion() =>
            new AllActiveRegion();
    }
}
