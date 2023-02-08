using System.Collections.Specialized;

namespace Prism.Regions;

public interface IViewsCollection : IEnumerable<VisualElement>, INotifyCollectionChanged
{
    bool Contains(VisualElement element);
}
