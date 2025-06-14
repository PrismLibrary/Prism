using System;
using System.ComponentModel;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockRegion : IRegion
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Func<string, object> GetViewStringDelegate { get; set; }

        private MockViewsCollection views = new MockViewsCollection();

        public IViewsCollection Views
        {
            get { return views; }
        }

        public IViewsCollection ActiveViews
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Context
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public INavigationParameters NavigationParameters
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public string Name { get; set; }

        public IRegionManager Add(string viewName) => throw new NotImplementedException();

        public IRegionManager Add(object view)
        {
            views.Add(view);
            return null;
        }

        public IRegionManager Add(object view, string viewName)
        {
            return Add(view);
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(object view)
        {
            throw new System.NotImplementedException();
        }

        public void Activate(object view)
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate(object view)
        {
            throw new System.NotImplementedException();
        }

        public object GetView(string viewName)
        {
            return GetViewStringDelegate(viewName);
        }

        public IRegionManager RegionManager { get; set; }

        public IRegionBehaviorCollection Behaviors
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool Navigate(System.Uri source)
        {
            throw new System.NotImplementedException();
        }

        public void RequestNavigate(System.Uri target, System.Action<NavigationResult> navigationCallback)
        {
            throw new System.NotImplementedException();
        }

        public void RequestNavigate(System.Uri target, System.Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public IRegionNavigationService NavigationService
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public System.Comparison<object> SortComparison
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
