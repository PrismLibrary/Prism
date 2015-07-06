

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

        #endregion

        public bool Navigate(Uri source)
        {
            throw new NotImplementedException();
        }
    }
}
