

using System;
using Prism.Regions;
using Prism.Common;
using System.Threading.Tasks;

namespace Prism.Regions.CallbackWrappers
{
    /// <summary>
    /// Provides navigation for regions.
    /// </summary>
    public class RegionNavigationServiceWithCallbacksWrapper : IRegionNavigationService
    {
        private IRegionNavigationServiceWithCallbacks _service;

        public RegionNavigationServiceWithCallbacksWrapper(IRegionNavigationServiceWithCallbacks service)
        {
            _service = service;
        }

        public IRegionNavigationJournal Journal
        {
            get { return _service.Journal; }
        }

        public IRegion Region
        {
            get { return _service.Region; }
            set { _service.Region = value; }
        }

        public event EventHandler<RegionNavigationEventArgs> Navigated
        {
            add { _service.Navigated += value; }
            remove { _service.Navigated -= value; }
        }

        public event EventHandler<RegionNavigationEventArgs> Navigating
        {
            add { _service.Navigating += value; }
            remove { _service.Navigating -= value; }
        }

        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed
        {
            add { _service.NavigationFailed += value; }
            remove { _service.NavigationFailed -= value; }
        }

        public Task<NavigationResult> RequestNavigateAsync(Uri target)
        {
            return CallbackHelper.AwaitCallbackResult< NavigationResult>(callback => _service.RequestNavigate(target, callback));
        }

        public Task<NavigationResult> RequestNavigateAsync(Uri target, NavigationParameters navigationParameters)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _service.RequestNavigate(target, callback, navigationParameters));
        }
    }
}
