using System;
using System.ComponentModel;
using Prism.Navigation;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockPresentationRegion : IRegion
    {
        public MockViewsCollection MockViews = new MockViewsCollection();
        public MockViewsCollection MockActiveViews = new MockViewsCollection();

        public MockPresentationRegion()
        {
            Behaviors = new MockRegionBehaviorCollection();
        }
        public IRegionManager Add(VisualElement view)
        {
            MockViews.Items.Add(view);

            return null;
        }

        public void Remove(VisualElement view)
        {
            MockViews.Items.Remove(view);
            MockActiveViews.Items.Remove(view);
        }

        public void Activate(VisualElement view)
        {
            MockActiveViews.Items.Add(view);
        }

        public IRegionManager Add(VisualElement view, string viewName)
        {
            throw new NotImplementedException();
        }

        public IRegionManager Add(VisualElement view, string viewName, bool createRegionManagerScope)
        {
            throw new NotImplementedException();
        }

        public VisualElement GetView(string viewName)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegionManager { get; set; }

        public IRegionBehaviorCollection Behaviors { get; set; }

        public IViewsCollection Views => MockViews;

        public IViewsCollection ActiveViews => MockActiveViews;

        public void Deactivate(VisualElement view)
        {
            MockActiveViews.Items.Remove(view);
        }

        private object _context;
        public object Context
        {
            get => _context;
            set
            {
                _context = value;
                OnPropertyChange("Context");
            }
        }

        public NavigationParameters NavigationParameters
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChange("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
