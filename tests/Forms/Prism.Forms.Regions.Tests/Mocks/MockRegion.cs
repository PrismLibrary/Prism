using System;
using System.ComponentModel;
using Prism.Navigation;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockRegion : IRegion
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Func<string, VisualElement> GetViewStringDelegate { get; set; }

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

        public IRegionManager Add(VisualElement view)
        {
            _views.Add(view);
            return null;
        }

        public IRegionManager Add(VisualElement view, string viewName)
        {
            return Add(view);
        }

        public IRegionManager Add(VisualElement view, string viewName, bool createRegionManagerScope)
        {
            throw new NotImplementedException();
        }

        public void Remove(VisualElement view)
        {
            throw new NotImplementedException();
        }

        public void Activate(VisualElement view)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(VisualElement view)
        {
            throw new NotImplementedException();
        }

        public VisualElement GetView(string viewName)
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


        public void RequestNavigate(Uri target, Action<IRegionNavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(Uri target, Action<IRegionNavigationResult> navigationCallback, INavigationParameters navigationParameters)
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


        public Comparison<VisualElement> SortComparison
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
