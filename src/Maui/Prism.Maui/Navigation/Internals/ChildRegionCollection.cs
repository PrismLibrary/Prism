using System.Collections;
using Prism.Regions;

namespace Prism.Navigation.Internals;

public class ChildRegionCollection : IEnumerable<VisualElement>, IDisposable
{
    private readonly List<IRegion> _regions = new();

    public void Add(IRegion region)
    {
        _regions.Add(region);
    }

    internal void Clear() => _regions.Clear();

    public IEnumerator<VisualElement> GetEnumerator() =>
        _regions.SelectMany(x => x.ActiveViews)
            .Where(x => x is not null) // sanity check
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        foreach (var region in _regions)
        {
            var manager = region.RegionManager;
            if (manager is null) continue;

            manager.Regions.Remove(region.Name);
        }
        _regions.Clear();
    }
}
