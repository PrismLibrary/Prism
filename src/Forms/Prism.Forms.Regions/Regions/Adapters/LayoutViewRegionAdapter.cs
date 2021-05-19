using System;
using System.Linq;
using Prism.Ioc;
using Prism.Properties;
using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// Adapter that creates a new <see cref="Region"/> and monitors its
    /// active view to set it on the adapted <see cref="Layout{View}"/>.
    /// </summary>
    public class LayoutViewRegionAdapter : RegionAdapterBase<Layout<View>>
    {
        private IContainerProvider _container { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="LayoutViewRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        /// <param name="container">The <see cref="IContainerProvider"/> used to resolve a new Region.</param>
        public LayoutViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory, IContainerProvider container)
            : base(regionBehaviorFactory)
        {
            _container = container;
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

            bool itemsSourceIsSet = regionTarget.Children?.Any() ?? false || regionTarget.IsSet(BindableLayout.ItemsSourceProperty);

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.LayoutViewHasChildrenException);
            }

            BindableLayout.SetItemsSource(regionTarget, region.Views);
            BindableLayout.SetItemTemplate(regionTarget, new RegionItemsSourceTemplate());
        }

        /// <summary>
        /// Creates a new instance of <see cref="IRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Region"/>.</returns>
        protected override IRegion CreateRegion() =>
            _container.Resolve<Region>();
    }
}
