

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Prism.Regions
{
    /// <summary>
    /// Defines a view of a collection.
    /// </summary>
    public interface IViewsCollection : IEnumerable<object>, INotifyCollectionChanged
    {
        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the collection.</param>
        /// <returns><see langword="true" /> if <paramref name="value"/> is found in the collection; otherwise, <see langword="false" />.</returns>
        bool Contains(object value);
    }
}