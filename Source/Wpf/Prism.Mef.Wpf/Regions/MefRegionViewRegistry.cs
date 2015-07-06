

using System.ComponentModel.Composition;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace Prism.Mef.Regions
{
    /// <summary>
    /// Exports the RegionViewRegistry using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IRegionViewRegistry))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefRegionViewRegistry : RegionViewRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefRegionViewRegistry"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        [ImportingConstructor]
        public MefRegionViewRegistry(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }
    }
}