using System.Collections.Generic;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the 
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the 
    /// name and type of the module. 
    /// </summary>
#if HAS_UWP
    [Windows.UI.Xaml.Markup.ContentProperty(Name = nameof(Items))]
#elif HAS_WINUI
    [Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Items))]
#else
    [Xamarin.Forms.ContentProperty(nameof(Items))]
#endif
    public class ModuleCatalog : ModuleCatalogBase
    {

    }
}
