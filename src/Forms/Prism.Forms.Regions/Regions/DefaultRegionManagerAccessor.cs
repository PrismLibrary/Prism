using System;
using Prism.Ioc;
using Xamarin.Forms;

namespace Prism.Regions
{
    internal class DefaultRegionManagerAccessor : IRegionManagerAccessor
    {
        /// <summary>
        /// Notification used by attached behaviors to update the region managers appropriately if needed to.
        /// </summary>
        /// <remarks>This event uses weak references to the event handler to prevent this static event of keeping the
        /// target element longer than expected.</remarks>
        public event EventHandler UpdatingRegions
        {
            add => Xaml.RegionManager.UpdatingRegions += value;
            remove => Xaml.RegionManager.UpdatingRegions -= value;
        }

        /// <summary>
        /// Gets the value for the RegionName attached property.
        /// </summary>
        /// <param name="element">The object to adapt. This is typically a container (i.e a control).</param>
        /// <returns>The name of the region that should be created when
        /// the RegionManager is also set in this element.</returns>
        public string GetRegionName(VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.GetValue(Xaml.RegionManager.RegionNameProperty) as string;
        }

        /// <summary>
        /// Gets the value of the RegionName attached property.
        /// </summary>
        /// <param name="element">The target element.</param>
        /// <returns>The <see cref="IRegionManager"/> attached to the <paramref name="element"/> element.</returns>
        public IRegionManager GetRegionManager(VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var regionManager = TryGetRegion(element);
            if (regionManager is null)
            {
                regionManager = ContainerLocator.Container.Resolve<IRegionManager>();
                element.SetValue(Xaml.RegionManager.RegionManagerProperty, regionManager);
            }

            return regionManager;
        }

        private IRegionManager TryGetRegion(VisualElement element)
        {
            try
            {
                return element.GetValue(Xaml.RegionManager.RegionManagerProperty) as IRegionManager;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
