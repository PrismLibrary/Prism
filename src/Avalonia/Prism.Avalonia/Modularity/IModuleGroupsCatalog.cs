using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines a model that can get the collection of <see cref="IModuleCatalogItem"/>.
    /// </summary>
    public interface IModuleGroupsCatalog
    {
        /// <summary>
        /// Gets the items in the <see cref="IModuleCatalog"/>. This property is mainly used to add <see cref="IModuleInfoGroup"/>s or
        /// <see cref="IModuleInfo"/>s through XAML.
        /// </summary>
        /// <value>The items in the catalog.</value>
        Collection<IModuleCatalogItem> Items { get; }
    }
}
