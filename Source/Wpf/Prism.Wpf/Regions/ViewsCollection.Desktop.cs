

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Prism.Regions
{
    public partial class ViewsCollection
    {
        private void NotifyAdd(IList items, int newStartingIndex)
        {
            if (items.Count > 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                                            NotifyCollectionChangedAction.Add,
                                            items,
                                            newStartingIndex));
            }
        }

        private void NotifyRemove(IList items, int originalIndex)
        {
            if (items.Count > 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    items, 
                    originalIndex));
            }
        }
    }
}
