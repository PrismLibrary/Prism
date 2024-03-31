using System.Collections;
using Prism.Navigation.Regions;

namespace Prism.Navigation.Internals;

/// <summary>
/// Represents a collection of child regions.
/// </summary>
public class ChildRegionCollection : IEnumerable<VisualElement>, IDisposable
{
    private readonly List<IRegion> _regions = new();

    /// <summary>
    /// Adds a region to the collection.
    /// </summary>
    /// <param name="region">The region to add.</param>
    public void Add(IRegion region)
    {
        _regions.Add(region);
    }

    internal void Clear() => _regions.Clear();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<VisualElement> GetEnumerator() =>
        _regions.SelectMany(x => x.ActiveViews)
            .OfType<VisualElement>()
            .Where(x => x is not null) // sanity check
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Disposes the collection and removes all regions from their respective region managers.
    /// </summary>
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
