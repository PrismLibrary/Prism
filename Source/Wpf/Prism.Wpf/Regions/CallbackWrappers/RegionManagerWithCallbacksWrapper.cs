

using System;

using Prism.Regions;
using Prism.Common;
using System.Threading.Tasks;

namespace Prism.Regions.CallbackWrappers
{
    /// <summary>
    /// Defines an interface to manage a set of <see cref="IRegion">regions</see> and to attach regions to objects (typically controls).
    /// </summary>
    public class RegionManagerWithCallbacksWrapper : IRegionManager
    {
        private IRegionManagerWithCallbacks _regionManager;

        public RegionManagerWithCallbacksWrapper(IRegionManagerWithCallbacks regionManager)
        {
            _regionManager = regionManager;
        }

        public IRegionCollection Regions
        {
            get { return _regionManager.Regions; }
        }

        public IRegionManager AddToRegion(string regionName, object view)
        {
            return _regionManager.AddToRegion(regionName, view);
        }

        public IRegionManager CreateRegionManager()
        {
            return _regionManager.CreateRegionManager();
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            return _regionManager.RegisterViewWithRegion(regionName, getContentDelegate);
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            return _regionManager.RegisterViewWithRegion(regionName, viewType);
        }

        public void RequestNavigate(string regionName, string source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        public void RequestNavigate(string regionName, Uri source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        public Task<NavigationResult> RequestNavigateAsync(string regionName, string source)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, source, callback));
        }

        public Task<NavigationResult> RequestNavigateAsync(string regionName, Uri source)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, source, callback));
        }

        public Task<NavigationResult> RequestNavigateAsync(string regionName, string target, NavigationParameters navigationParameters)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, target, callback, navigationParameters));
        }

        public Task<NavigationResult> RequestNavigateAsync(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => _regionManager.RequestNavigate(regionName, target, callback, navigationParameters));
        }
    }
}
