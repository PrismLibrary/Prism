

using System;
using System.Collections;
using System.Collections.Generic;
using Prism.Regions;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockRegionManager : IRegionManager
    {
        private IRegionCollection regions = new MockRegionCollection();
        internal MockRegionCollection MockRegionCollection
        {
            get
            {
                return regions as MockRegionCollection;
            }
        }

        public IRegionCollection Regions
        {
            get { return regions; }
        }

        public IRegionManager CreateRegionManager()
        {
            throw new System.NotImplementedException();
        }

        public IRegionManager AddToRegion(string regionName, object view)
        {
            throw new System.NotImplementedException();
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

        public bool Navigate(System.Uri source)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class MockRegionCollection : List<IRegion>, IRegionCollection
    {
        IEnumerator<IRegion> IEnumerable<IRegion>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IRegion this[string regionName]
        {
            get { return this[0]; }
        }

        void IRegionCollection.Add(IRegion region)
        {
            this.Add(region);
        }

        public bool Remove(string regionName)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsRegionWithName(string regionName)
        {
            return true;
        }

        public void Add(string regionName, IRegion region)
        {
            throw new NotImplementedException();
        }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
