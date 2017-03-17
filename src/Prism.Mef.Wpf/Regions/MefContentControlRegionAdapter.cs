

using System.ComponentModel.Composition;
using Prism.Regions;

namespace Prism.Mef.Regions
{
    /// <summary>
    /// Exports the ContentControlRegionAdapter using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the <see cref="MefBootstrapper.ConfigureContainer" /> to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(ContentControlRegionAdapter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefContentControlRegionAdapter : ContentControlRegionAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefContentControlRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The region behavior factory.</param>
        [ImportingConstructor]
        public MefContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }
    }
}