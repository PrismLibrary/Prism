

using System.ComponentModel.Composition;
using CommonServiceLocator;
using Prism.Regions;

namespace Prism.Mef.Regions
{
    /// <summary>
    /// Exports the MefRegionNavigationService using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IRegionNavigationService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MefRegionNavigationService : RegionNavigationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefRegionNavigationService"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="navigationContentLoader">The navigation content loader.</param>
        /// <param name="journal">The navigation journal.</param>
        [ImportingConstructor]
        public MefRegionNavigationService(IServiceLocator serviceLocator, IRegionNavigationContentLoader navigationContentLoader, IRegionNavigationJournal journal)
            : base(serviceLocator, navigationContentLoader, journal)
        { }
    }
}
