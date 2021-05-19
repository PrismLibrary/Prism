using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Prism.Forms.Regions.Tests
{
    public class CollectionChangedTracker
    {
        private readonly List<NotifyCollectionChangedEventArgs> eventList = new List<NotifyCollectionChangedEventArgs>();

        public CollectionChangedTracker(INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += OnCollectionChanged;
        }

        public IEnumerable<NotifyCollectionChangedAction> ActionsFired =>
            eventList.Select(e => e.Action);
        public IEnumerable<NotifyCollectionChangedEventArgs> NotifyEvents => eventList;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            eventList.Add(e);
        }
    }
}
