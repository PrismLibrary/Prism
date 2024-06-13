using System;
using Avalonia.Controls;
using Prism.Properties;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Adapter that creates a new <see cref="AllActiveRegion"/> and binds all
    /// the views to the adapted <see cref="ItemsControl"/>.
    /// </summary>
    public class ItemsControlRegionAdapter : RegionAdapterBase<ItemsControl>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ItemsControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public ItemsControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts an <see cref="ItemsControl"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, ItemsControl regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            // NOTE: In Avalonia, Items will never be null
            bool itemsSourceIsSet = regionTarget.ItemCount > 0;
            itemsSourceIsSet = itemsSourceIsSet || regionTarget.HasBinding(ItemsControl.ItemsSourceProperty);

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.ItemsControlHasItemsSourceException);
            }

            // If control has child items, move them to the region and then bind control to region. Can't set ItemsSource if child items exist.
            if (regionTarget.ItemCount > 0)
            {
                foreach (object childItem in regionTarget.Items)
                {
                    region.Add(childItem);
                }

                // Control must be empty before setting ItemsSource
                regionTarget.Items.Clear();
            }

            // Avalonia v11-Preview5 needs IRegion implement IList. Enforcing it to return AvaloniaList<object> fixes this.
            // Avalonia v11-Preview8 ItemsControl.Items is readonly (#10827).
            ////regionTarget.Items = region.Views as Avalonia.Collections.AvaloniaList<object>;
            regionTarget.ItemsSource = region.Views as Avalonia.Collections.AvaloniaList<object>;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AllActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="AllActiveRegion"/>.</returns>
        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}
