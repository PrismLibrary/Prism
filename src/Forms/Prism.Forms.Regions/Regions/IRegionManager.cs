namespace Prism.Regions
{
    /// <summary>
    /// Defines an interface to manage a set of <see cref="IRegion">regions</see> and to attach regions to objects (typically controls).
    /// </summary>
    public interface IRegionManager
    {
        /// <summary>
        /// Gets a collection of <see cref="IRegion"/> that identify each region by name. You can use this collection to add or remove regions to the current region manager.
        /// </summary>
        IRegionCollection Regions { get; }

        /// <summary>
        /// Creates a new region manager.
        /// </summary>
        /// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
        IRegionManager CreateRegionManager();
    }
}
