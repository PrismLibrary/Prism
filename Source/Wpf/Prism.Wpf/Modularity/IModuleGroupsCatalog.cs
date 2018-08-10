using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    public interface IModuleGroupsCatalog
    {
        Collection<IModuleCatalogItem> Items { get; }
    }
}
