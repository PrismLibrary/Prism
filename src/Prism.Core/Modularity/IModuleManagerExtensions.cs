using System.Linq;

namespace Prism.Modularity
{
    /// <summary>
    /// Common extensions for the <see cref="IModuleManager"/>
    /// </summary>
    public static class IModuleManagerExtensions
    {
        /// <summary>
        /// Checks to see if the <see cref="IModule"/> exists in the <see cref="IModuleCatalog.Modules"/>  
        /// </summary>
        /// <returns><c>true</c> if the Module exists.</returns>
        /// <param name="manager">The <see cref="IModuleManager"/>.</param>
        /// <typeparam name="T">The <see cref="IModule"/> to check for.</typeparam>
        public static bool ModuleExists<T>(this IModuleManager manager)
            where T : IModule =>
            manager.Modules.Any(mi => mi.ModuleType == typeof(T).AssemblyQualifiedName);

        /// <summary>
        /// Exists the specified catalog and name.
        /// </summary>
        /// <returns><c>true</c> if the Module exists.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <param name="name">Name.</param>
        public static bool ModuleExists(this IModuleManager catalog, string name) =>
            catalog.Modules.Any(module => module.ModuleName == name);

        /// <summary>
        /// Gets the current <see cref="ModuleState"/> of the <see cref="IModule"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="IModule"/> to check.</typeparam>
        /// <param name="manager">The <see cref="IModuleManager"/>.</param>
        /// <returns></returns>
        public static ModuleState GetModuleState<T>(this IModuleManager manager)
            where T : IModule =>
            manager.Modules.FirstOrDefault(mi => mi.ModuleType == typeof(T).AssemblyQualifiedName).State;

        /// <summary>
        /// Gets the current <see cref="ModuleState"/> of the <see cref="IModule"/>.
        /// </summary>
        /// <param name="manager">The <see cref="IModuleManager"/>.</param>
        /// <param name="name">Name.</param>
        /// <returns></returns>
        public static ModuleState GetModuleState(this IModuleManager manager, string name) =>
            manager.Modules.FirstOrDefault(module => module.ModuleName == name).State;

        /// <summary>
        /// Checks to see if the <see cref="IModule"/> is already initialized. 
        /// </summary>
        /// <returns><c>true</c>, if initialized, <c>false</c> otherwise.</returns>
        /// <param name="manager">The <see cref="IModuleManager"/>.</param>
        /// <typeparam name="T">The <see cref="IModule"/> to check.</typeparam>
        public static bool IsModuleInitialized<T>(this IModuleManager manager)
            where T : IModule =>
            manager.Modules.FirstOrDefault(mi => mi.ModuleType == typeof(T).AssemblyQualifiedName)?.State == ModuleState.Initialized;

        /// <summary>
        /// Checks to see if the <see cref="IModule"/> is already initialized. 
        /// </summary>
        /// <returns><c>true</c>, if initialized, <c>false</c> otherwise.</returns>
        /// <param name="manager">The <see cref="IModuleManager"/>.</param>
        /// <param name="name">Name.</param>
        public static bool IsModuleInitialized(this IModuleManager manager, string name) =>
            manager.Modules.FirstOrDefault(module => module.ModuleName == name)?.State == ModuleState.Initialized;

        /// <summary>
        /// Loads and initializes the module in the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="IModule"/> to load.</typeparam>
        /// <param name="manager">The <see cref="IModuleManager"/>.</param>
        public static void LoadModule<T>(this IModuleManager manager)
            where T : IModule =>
            manager.LoadModule(typeof(T).Name);
    }
}
