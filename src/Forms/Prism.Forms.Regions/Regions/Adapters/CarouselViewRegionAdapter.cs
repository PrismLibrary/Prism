using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// Adapter that creates a new <see cref="SingleActiveRegion"/> and monitors its
    /// active view to set it on the adapted <see cref="CarouselView"/>.
    /// </summary>
    public class CarouselViewRegionAdapter : RegionAdapterBase<CarouselView>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CarouselViewRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public CarouselViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts a <see cref="CarouselView"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, CarouselView regionTarget)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of <see cref="SingleActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="SingleActiveRegion"/>.</returns>
        protected override IRegion CreateRegion() =>
            new SingleActiveRegion();
    }
}
