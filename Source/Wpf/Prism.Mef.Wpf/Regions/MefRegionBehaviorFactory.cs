

using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace Prism.Mef.Regions
{
    /// <summary>
    /// Exports the RegionBehaviorFactory using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IRegionBehaviorFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefRegionBehaviorFactory : RegionBehaviorFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefRegionBehaviorFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator"><see cref="IServiceLocator"/> used to create the instance of the behavior from its <see cref="Type"/>.</param>
        [ImportingConstructor]
        public MefRegionBehaviorFactory(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }
    }
}