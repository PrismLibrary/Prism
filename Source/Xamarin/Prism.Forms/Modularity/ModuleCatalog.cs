using System.Collections.Generic;

using Xamarin.Forms;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the 
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the 
    /// name and type of the module. 
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class ModuleCatalog : ModuleCatalogBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalog() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class while providing an
        /// initial list of <see cref="ModuleInfo"/>s.
        /// </summary>
        /// <param name="modules">The initial list of modules.</param>
        public ModuleCatalog(IEnumerable<ModuleInfo> modules) : base(modules)
        {
        }
    }
}
