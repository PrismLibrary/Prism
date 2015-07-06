

using System.Collections.Generic;

namespace Prism.Modularity
{
    /// <summary>
    /// This is the expected catalog definition for the ModuleManager. 
    /// The ModuleCatalog holds information about the modules that can be used by the 
    /// application. Each module is described in a ModuleInfo class, that records the 
    /// name, type and location of the module. 
    /// </summary>
    public interface IModuleCatalog
    {
        /// <summary>
        /// Gets all the <see cref="ModuleInfo"/> classes that are in the <see cref="ModuleCatalog"/>.
        /// </summary>
        IEnumerable<ModuleInfo> Modules { get; }

        /// <summary>
        /// Return the list of <see cref="ModuleInfo"/>s that <paramref name="moduleInfo"/> depends on.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to get the </param>
        /// <returns>An enumeration of <see cref="ModuleInfo"/> that <paramref name="moduleInfo"/> depends on.</returns>
        IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo);

        /// <summary>
        /// Returns the collection of <see cref="ModuleInfo"/>s that contain both the <see cref="ModuleInfo"/>s in 
        /// <paramref name="modules"/>, but also all the modules they depend on. 
        /// </summary>
        /// <param name="modules">The modules to get the dependencies for.</param>
        /// <returns>
        /// A collection of <see cref="ModuleInfo"/> that contains both all <see cref="ModuleInfo"/>s in <paramref name="modules"/>
        /// and also all the <see cref="ModuleInfo"/> they depend on.
        /// </returns>
        IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules);

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to add.</param>
        /// <returns>The <see cref="ModuleCatalog"/> for easily adding multiple modules.</returns>
        void AddModule(ModuleInfo moduleInfo);
    }
}
