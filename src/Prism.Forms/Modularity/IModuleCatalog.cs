using System.Collections.Generic;

namespace Prism.Modularity
{
    /// <summary>
    /// This is the expected catalog definition for the ModuleManager. 
    /// The ModuleCatalog holds information about the modules that can be used by the 
    /// application. Each module is described in a ModuleInfo class, that records the 
    /// name and type of the module. 
    /// </summary>
    public interface IModuleCatalog
    {
        /// <summary>
        /// Gets all the <see cref="ModuleInfo"/> classes that are in the <see cref="ModuleCatalog"/>.
        /// </summary>
        IEnumerable<ModuleInfo> Modules { get; }

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to add.</param>
        /// <returns>The <see cref="ModuleCatalog"/> for easily adding multiple modules.</returns>
        ModuleCatalog AddModule(ModuleInfo moduleInfo);

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        void Initialize();
    }
}
