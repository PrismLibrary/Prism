using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace Prism.Regions
{
    public interface IViewsCollection : IEnumerable<VisualElement>, INotifyCollectionChanged
    {
        bool Contains(VisualElement element);
    }
}
