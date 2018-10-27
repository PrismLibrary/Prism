using Windows.UI.Xaml.Markup;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the 
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the 
    /// name and type of the module. 
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public class ModuleCatalog : ModuleCatalogBase
    {

    }
}
