

using System;
using System.Windows;

namespace Prism.Regions
{
    internal class DefaultRegionManagerAccessor : IRegionManagerAccessor
    {
        /// <summary>
        /// Notification used by attached behaviors to update the region managers appropriatelly if needed to.
        /// </summary>
        /// <remarks>This event uses weak references to the event handler to prevent this static event of keeping the
        /// target element longer than expected.</remarks>
        public event EventHandler UpdatingRegions
        {
            add { RegionManager.UpdatingRegions += value; }
            remove { RegionManager.UpdatingRegions -= value; }
        }

        /// <summary>
        /// Gets the value for the RegionName attached property.
        /// </summary>
        /// <param name="element">The object to adapt. This is typically a container (i.e a control).</param>
        /// <returns>The name of the region that should be created when
        /// the RegionManager is also set in this element.</returns>
        public string GetRegionName(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.GetValue(RegionManager.RegionNameProperty) as string;
        }

        /// <summary>
        /// Gets the value of the RegionName attached property.
        /// </summary>
        /// <param name="element">The target element.</param>
        /// <returns>The <see cref="IRegionManager"/> attached to the <paramref name="element"/> element.</returns>
        public IRegionManager GetRegionManager(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.GetValue(RegionManager.RegionManagerProperty) as IRegionManager;
        }
    }
}