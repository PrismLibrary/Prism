using System;
using Prism.Ioc;
using Prism.Properties;
using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// Adapter that creates a new <see cref="Region"/> and monitors its
    /// active view to set it on the adapted <see cref="CollectionView"/>.
    /// </summary>
    public class CollectionViewRegionAdapter : RegionAdapterBase<CollectionView>
    {
        private IContainerProvider _container { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CollectionViewRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        /// <param name="container">The <see cref="IContainerProvider"/> used to resolve a new Region.</param>
        public CollectionViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory, IContainerProvider container)
            : base(regionBehaviorFactory)
        {
            _container = container;
        }

        /// <summary>
        /// Adapts a <see cref="CollectionView"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, CollectionView regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            bool itemsSourceIsSet = regionTarget.ItemsSource != null || regionTarget.IsSet(ItemsView.ItemsSourceProperty);

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.CollectionViewHasItemsSourceException);
            }

            regionTarget.ItemsSource = region.Views;
            regionTarget.ItemTemplate = new RegionItemsSourceTemplate();
        }

        /// <summary>
        /// Creates a new instance of <see cref="IRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Region"/>.</returns>
        protected override IRegion CreateRegion() =>
            _container.Resolve<Region>();
    }
}
