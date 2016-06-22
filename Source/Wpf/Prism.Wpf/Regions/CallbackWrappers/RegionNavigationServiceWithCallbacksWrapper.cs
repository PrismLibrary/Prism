

using System;
using System.Threading.Tasks;

using Prism.Common;
using Prism.Regions;

namespace Prism.Regions.CallbackWrappers
{
    /// <summary>
    /// Creates an instance of <see cref="IRegionNavigationService"/> based on an instance of <see cref="IRegionNavigationServiceWithCallbacks"/>
    /// </summary>
    /// <seealso cref="Prism.Regions.IRegionNavigationService" />
    public class RegionNavigationServiceWithCallbacksWrapper : IRegionNavigationService
    {
        private IRegionNavigationServiceWithCallbacks _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationServiceWithCallbacksWrapper"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public RegionNavigationServiceWithCallbacksWrapper(IRegionNavigationServiceWithCallbacks service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the journal.
        /// </summary>
        /// <value>The journal.</value>
        public IRegionNavigationJournal Journal
        {
            get { return _service.Journal; }
        }

        /// <summary>
        /// Gets or sets the region owning this service.
        /// </summary>
        /// <value>A Region.</value>
        public IRegion Region
        {
            get { return _service.Region; }
            set { _service.Region = value; }
        }

        /// <summary>
        /// Raised when the region is navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigated
        {
            add { _service.Navigated += value; }
            remove { _service.Navigated -= value; }
        }

        /// <summary>
        /// Raised when the region is about to be navigated to content.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigating
        {
            add { _service.Navigating += value; }
            remove { _service.Navigating -= value; }
        }

        /// <summary>
        /// Raised when a navigation request fails.
        /// </summary>
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed
        {
            add { _service.NavigationFailed += value; }
            remove { _service.NavigationFailed -= value; }
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri" />.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <returns>The navigation result.</returns>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the
        /// <see cref="NavigationAsyncExtensions" /> class.
        /// </remarks>
        public Task<NavigationResult> RequestNavigateAsync(Uri target)
        {
            return CallbackHelper.AwaitCallbackResult< NavigationResult>(callback => _service.RequestNavigate(target, callback));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <returns>The navigation result.</returns>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        public Task<NavigationResult> RequestNavigateAsync(Uri target, NavigationParameters navigationParameters)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _service.RequestNavigate(target, callback, navigationParameters));
        }
    }
}
