

using System.ComponentModel.Composition;
using Prism.Regions;
using Prism.Regions.Behaviors;

namespace Prism.Mef.Regions.Behaviors
{
    /// <summary>
    /// Exports the DelayedRegionCreationBehavior using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(DelayedRegionCreationBehavior))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MefDelayedRegionCreationBehavior : DelayedRegionCreationBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefDelayedRegionCreationBehavior"/> class.
        /// </summary>
        /// <param name="regionAdapterMappings">The region adapter mappings, that are used to find the correct adapter for
        /// a given controltype. The controltype is determined by the <see name="TargetElement"/> value.</param>
        [ImportingConstructor]
        public MefDelayedRegionCreationBehavior(RegionAdapterMappings regionAdapterMappings)
            : base(regionAdapterMappings)
        {
        }
    }
}