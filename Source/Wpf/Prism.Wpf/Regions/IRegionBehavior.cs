

namespace Prism.Regions
{
    /// <summary>
    /// Interface for allowing extensible behavior on regions.
    /// </summary>
    public interface IRegionBehavior
    {
        /// <summary>
        /// The region that this behavior is extending.
        /// </summary>
        IRegion Region { get; set; }

        /// <summary>
        /// Attaches the behavior to the specified region.
        /// </summary>
        void Attach();

    }
}