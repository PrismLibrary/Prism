using System;
using System.ComponentModel;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Prism.Navigation.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockRegion : IRegion
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Func<string, object> GetViewStringDelegate { get; set; }

        private MockViewsCollection _views = new MockViewsCollection();

        public IViewsCollection Views => _views;

        public IViewsCollection ActiveViews => throw new NotImplementedException();

        public object Context
        {
            get;
            set;
        }

        public NavigationParameters NavigationParameters
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Name { get; set; }

        public IRegionManager Add(string viewName) => throw new NotImplementedException();

        public IRegionManager Add(object view)
        {
            _views.Add(view);
            return null;
        }

        public IRegionManager Add(object view, string viewName)
        {
            return Add(view);
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new NotImplementedException();
        }

        public void Remove(object view)
        {
            throw new NotImplementedException();
        }

        public void Activate(object view)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(object view)
        {
            throw new NotImplementedException();
        }

        public object GetView(string viewName)
        {
            return GetViewStringDelegate(viewName);
        }

        public IRegionManager RegionManager { get; set; }

        public IRegionBehaviorCollection Behaviors
        {
            get { throw new NotImplementedException(); }
        }

        public bool Navigate(Uri source)
        {
            throw new NotImplementedException();
        }


        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public IRegionNavigationService NavigationService
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }


        public Comparison<object> SortComparison
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
