

using System.ComponentModel.Composition;
using Prism.Regions.Behaviors;

namespace Prism.Mef.Regions.Behaviors
{
    /// <summary>
    /// Exports the SyncRegionContextWithHostBehavior using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(SyncRegionContextWithHostBehavior))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MefSyncRegionContextWithHostBehavior : SyncRegionContextWithHostBehavior
    {
    }
}