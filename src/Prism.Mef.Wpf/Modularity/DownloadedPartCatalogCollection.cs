

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using Prism.Modularity;

namespace Prism.Mef.Modularity
{
    /// <summary>
    /// Holds a collection of composable part catalogs keyed by module info.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DownloadedPartCatalogCollection
    {
        private Dictionary<ModuleInfo, ComposablePartCatalog> catalogs = new Dictionary<ModuleInfo, ComposablePartCatalog>();

        /// <summary>
        /// Adds the specified catalog using the module info as a key.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="catalog">The catalog.</param>
        public void Add(ModuleInfo moduleInfo, ComposablePartCatalog catalog)
        {
            catalogs.Add(moduleInfo, catalog);
        }

        /// <summary>
        /// Gets the catalog for the specified module info.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <returns></returns>
        public ComposablePartCatalog Get(ModuleInfo moduleInfo)
        {
            return this.catalogs[moduleInfo];
        }

        /// <summary>
        /// Tries to ge the catalog for the specified module info.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="catalog">The catalog.</param>
        /// <returns>true if found; otherwise false;</returns>
        public bool TryGet(ModuleInfo moduleInfo, out ComposablePartCatalog catalog)
        {
            return this.catalogs.TryGetValue(moduleInfo, out catalog);
        }

        /// <summary>
        /// Removes the catalgo for the specified module info.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        public void Remove(ModuleInfo moduleInfo)
        {
            this.catalogs.Remove(moduleInfo);
        }

        /// <summary>
        /// Clears the collection of catalogs.
        /// </summary>
        public void Clear()
        {
            this.catalogs.Clear();
        }
    }
}
