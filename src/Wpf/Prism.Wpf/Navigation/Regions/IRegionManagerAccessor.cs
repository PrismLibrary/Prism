namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides an abstraction on top of the RegionManager static members.
    /// </summary>
    public interface IRegionManagerAccessor
    {
        /// <summary>
        /// Notification used by attached behaviors to update the region managers appropriately if needed to.
        /// </summary>
        /// <remarks>This event uses weak references to the event handler to prevent this static event of keeping the
        /// target element longer than expected.</remarks>
        event EventHandler UpdatingRegions;

        /// <summary>
        /// Gets the value for the RegionName attached property.
        /// </summary>
        /// <param name="element">The object to adapt. This is typically a container (i.e a control).</param>
        /// <returns>The name of the region that should be created when
        /// the RegionManager is also set in this element.</returns>
#if !AVALONIA
        string GetRegionName(DependencyObject element);
#else
        string GetRegionName(AvaloniaObject element);
#endif

        /// <summary>
        /// Gets the value of the RegionName attached property.
        /// </summary>
        /// <param name="element">The target element.</param>
        /// <returns>The <see cref="IRegionManager"/> attached to the <paramref name="element"/> element.</returns>
#if !AVALONIA
        IRegionManager GetRegionManager(DependencyObject element);
#else
        IRegionManager GetRegionManager(AvaloniaObject element);
#endif
    }
}
