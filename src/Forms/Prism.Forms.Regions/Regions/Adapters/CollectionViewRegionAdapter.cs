using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// Adapter that creates a new <see cref="AllActiveRegion"/> and monitors its
    /// active view to set it on the adapted <see cref="CollectionView"/>.
    /// </summary>
    public class CollectionViewRegionAdapter : RegionAdapterBase<CollectionView>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CollectionViewRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public CollectionViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts a <see cref="CollectionView"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, CollectionView regionTarget)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of <see cref="AllActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="AllActiveRegion"/>.</returns>
        protected override IRegion CreateRegion() =>
            new AllActiveRegion();
    }
}
