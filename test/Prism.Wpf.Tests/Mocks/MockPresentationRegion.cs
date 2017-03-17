

using System;
using System.ComponentModel;
using Prism.Regions;

namespace Prism.Wpf.Tests.Mocks
{
    class MockPresentationRegion : IRegion
    {
        public MockViewsCollection MockViews = new MockViewsCollection();
        public MockViewsCollection MockActiveViews = new MockViewsCollection();

        public MockPresentationRegion()
        {
            Behaviors = new MockRegionBehaviorCollection();
        }
        public IRegionManager Add(object view)
        {
            MockViews.Items.Add(view);

            return null;
        }

        public void Remove(object view)
        {
            MockViews.Items.Remove(view);
            MockActiveViews.Items.Remove(view);
        }

        public void Activate(object view)
        {
            MockActiveViews.Items.Add(view);
        }

        public IRegionManager Add(object view, string viewName)
        {
            throw new NotImplementedException();
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new NotImplementedException();
        }

        public object GetView(string viewName)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegionManager { get; set; }

        public IRegionBehaviorCollection Behaviors { get; set; }

        public IViewsCollection Views
        {
            get { return MockViews; }
        }

        public IViewsCollection ActiveViews
        {
            get { return MockActiveViews; }
        }

        public void Deactivate(object view)
        {
            MockActiveViews.Items.Remove(view);
        }

        private object context;
        public object Context
        {
            get { return context; }
            set
            {
                context = value;
                OnPropertyChange("Context");
            }
        }

        public NavigationParameters NavigationParameters
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.OnPropertyChange("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Navigate(Uri source)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public IRegionNavigationService NavigationService
        {
            get { throw new NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Comparison<object> SortComparison
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}