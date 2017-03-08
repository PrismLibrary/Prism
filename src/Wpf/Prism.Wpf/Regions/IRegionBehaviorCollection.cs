

using System.Collections.Generic;

namespace Prism.Regions
{
    /// <summary>
    /// Defines the interface for a collection of <see cref="IRegionBehavior"/> classes on a Region.
    /// </summary>
    public interface IRegionBehaviorCollection : IEnumerable<KeyValuePair<string, IRegionBehavior>>
    {

        /// <summary>
        /// Adds a <see cref="IRegionBehavior"/> to the collection, using the specified key as an indexer. 
        /// </summary>
        /// <param name="key">
        /// The key that specifies the type of <see cref="IRegionBehavior"/> that's added. 
        /// </param>
        /// <param name="regionBehavior">The <see cref="IRegionBehavior"/> to add.</param>
        void Add(string key, IRegionBehavior regionBehavior);

        /// <summary>
        /// Checks if a <see cref="IRegionBehavior"/> with the specified key is already present. 
        /// </summary>
        /// <param name="key">The key to use to find a particular <see cref="IRegionBehavior"/>.</param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the <see cref="IRegionBehavior"/> with the specified key.
        /// </summary>
        /// <value>The registered <see cref="IRegionBehavior"/></value>
        IRegionBehavior this[string key]{ get; }
    }
}
