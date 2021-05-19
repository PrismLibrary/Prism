using System;
using System.Collections.Specialized;
using System.Linq;
using Prism.Ioc;
using Prism.Properties;
using Prism.Regions.Behaviors;
using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// Adapter that creates a new <see cref="Region"/> and monitors its
    /// active view to set it on the adapted <see cref="ScrollView"/>.
    /// </summary>
    public class ScrollViewRegionAdapter : RegionAdapterBase<ScrollView>
    {
        private IContainerProvider _container { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ScrollViewRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        /// <param name="container">The <see cref="IContainerProvider"/> used to resolve a new Region.</param>
        public ScrollViewRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory, IContainerProvider container)
            : base(regionBehaviorFactory)
        {
            _container = container;
        }

        /// <summary>
        /// Adapts a <see cref="ScrollView"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, ScrollView regionTarget)
        {
            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            // No binding check required as the ContentProperty is not Bindable
            bool contentIsSet = regionTarget.Content != null;

            if (contentIsSet)
                throw new InvalidOperationException(Resources.ScrollViewHasContentException);

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault() as View;
            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems[0] as VisualElement);
                    }
                };
        }

        /// <summary>
        /// Creates a new instance of <see cref="Region"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Region"/>.</returns>
        protected override IRegion CreateRegion() =>
            _container.Resolve<Region>();
    }
}
