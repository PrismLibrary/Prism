

using System.ComponentModel.Composition;
using Prism.Regions.Behaviors;

namespace Prism.Mef.Regions.Behaviors
{
    /// <summary>
    /// Exports the AutoPopulateRegionBehavior using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(RegionMemberLifetimeBehavior))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MefRegionMemberLifetimeBehavior : RegionMemberLifetimeBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefAutoPopulateRegionBehavior"/> class.
        /// </summary>
        [ImportingConstructor]
        public MefRegionMemberLifetimeBehavior()
        {
        }
    }
}
