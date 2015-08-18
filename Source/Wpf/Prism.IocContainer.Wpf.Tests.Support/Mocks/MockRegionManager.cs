

using System;
using Prism.Regions;

namespace Prism.IocContainer.Wpf.Tests.Support.Mocks
{
    public class MockRegionManager : IRegionManager
    {
        #region IRegionManager Members

        public IRegionCollection Regions
        {
            get { throw new NotImplementedException(); }
        }

        public IRegionManager CreateRegionManager()
        {
            throw new NotImplementedException();
        }

        public IRegionManager AddToRegion(string regionName, object view)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri source)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string source)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool Navigate(Uri source)
        {
            throw new NotImplementedException();
        }
    }
}
