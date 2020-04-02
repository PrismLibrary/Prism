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
        /// Gets all the <see cref="IModuleInfo"/> classes that are in the <see cref="IModuleCatalog"/>.
        /// </summary>
        IEnumerable<IModuleInfo> Modules { get; }

        /// <summary>
        /// Return the list of <see cref="IModuleInfo"/>s that <paramref name="moduleInfo"/> depends on.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="IModuleInfo"/> to get the </param>
        /// <returns>An enumeration of <see cref="IModuleInfo"/> that <paramref name="moduleInfo"/> depends on.</returns>
        IEnumerable<IModuleInfo> GetDependentModules(IModuleInfo moduleInfo);

        /// <summary>
        /// Returns the collection of <see cref="IModuleInfo"/>s that contain both the <see cref="IModuleInfo"/>s in 
        /// <paramref name="modules"/>, but also all the modules they depend on. 
        /// </summary>
        /// <param name="modules">The modules to get the dependencies for.</param>
        /// <returns>
        /// A collection of <see cref="IModuleInfo"/> that contains both all <see cref="IModuleInfo"/>s in <paramref name="modules"/>
        /// and also all the <see cref="IModuleInfo"/> they depend on.
        /// </returns>
        IEnumerable<IModuleInfo> CompleteListWithDependencies(IEnumerable<IModuleInfo> modules);

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds a <see cref="IModuleInfo"/> to the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="IModuleInfo"/> to add.</param>
        /// <returns>The <see cref="IModuleCatalog"/> for easily adding multiple modules.</returns>
        IModuleCatalog AddModule(IModuleInfo moduleInfo);
    }
}
