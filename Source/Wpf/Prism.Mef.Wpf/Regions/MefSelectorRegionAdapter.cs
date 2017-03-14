

using System.ComponentModel.Composition;
using Prism.Regions;

namespace Prism.Mef.Regions
{
    /// <summary>
    /// Exports the SelectorRegionAdapter using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(SelectorRegionAdapter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefSelectorRegionAdapter : SelectorRegionAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefSelectorRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        [ImportingConstructor]
        public MefSelectorRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }
    }
}