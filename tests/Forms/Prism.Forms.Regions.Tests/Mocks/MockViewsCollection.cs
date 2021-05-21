using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Prism.Regions;
using Xamarin.Forms;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockViewsCollection : IViewsCollection
    {
        public ObservableCollection<VisualElement> Items = new ObservableCollection<VisualElement>();

        public void Add(VisualElement view)
        {
            Items.Add(view);
        }

        public bool Contains(VisualElement value)
        {
            return Items.Contains(value);
        }

        public IEnumerator<VisualElement> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Items.CollectionChanged += value;
            remove => Items.CollectionChanged -= value;
        }
    }
}
