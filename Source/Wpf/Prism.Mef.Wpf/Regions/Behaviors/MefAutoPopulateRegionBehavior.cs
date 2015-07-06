

using System.ComponentModel.Composition;
using Prism.Regions;
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
    [Export(typeof(AutoPopulateRegionBehavior))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MefAutoPopulateRegionBehavior : AutoPopulateRegionBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefAutoPopulateRegionBehavior"/> class.
        /// </summary>
        /// <param name="regionViewRegistry"><see cref="IRegionViewRegistry"/> that the behavior will monitor for views to populate the region.</param>
        [ImportingConstructor]
        public MefAutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry)
            : base(regionViewRegistry)
        {
        }
    }
}