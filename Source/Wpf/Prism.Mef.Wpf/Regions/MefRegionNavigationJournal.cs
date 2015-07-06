

using System.ComponentModel.Composition;
using Prism.Regions;

namespace Prism.Mef.Regions
{
    /// <summary>
    /// Exports the MefRegionNavigationJournal using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IRegionNavigationJournal))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MefRegionNavigationJournal : RegionNavigationJournal
    {
    }
}
