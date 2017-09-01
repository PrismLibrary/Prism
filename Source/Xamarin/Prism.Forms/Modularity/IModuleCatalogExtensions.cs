using System.Linq;

namespace Prism.Modularity
{
    /// <summary>
    /// <see cref="IModuleCatalog"/>  extensions.
    /// </summary>
    public static class IModuleCatalogExtensions
    {
        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <returns>The module.</returns>
        /// <param name="catalog">Catalog</param>
        /// <param name="mode"><see cref="InitializationMode"/></param>
        /// <typeparam name="T">The <see cref="IModule"/> type parameter.</typeparam>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, InitializationMode mode = InitializationMode.WhenAvailable)
            where T : IModule =>
            catalog.AddModule<T>(typeof(T).Name, mode);

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <returns>The module.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <param name="name">Name.</param>
        /// <param name="mode"><see cref="IModule"/>.</param>
        /// <typeparam name="T">The <see cref="IModule"/> type parameter.</typeparam>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, string name, InitializationMode mode = InitializationMode.WhenAvailable)
            where T : IModule =>
            catalog.AddModule(new ModuleInfo(name, typeof(T), mode));

        /// <summary>
        /// Checks to see if the <see cref="IModule"/> exists in the <see cref="IModuleCatalog.Modules"/>  
        /// </summary>
        /// <returns><c>true</c> if the Module exists.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <typeparam name="T">The <see cref="IModule"/> to check for.</typeparam>
        public static bool Exists<T>(this IModuleCatalog catalog)
            where T : IModule =>
            catalog.Modules.Any(mi => mi.ModuleType == typeof(T));

        /// <summary>
        /// Exists the specified catalog and name.
        /// </summary>
        /// <returns><c>true</c> if the Module exists.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <param name="name">Name.</param>
        public static bool Exists(this IModuleCatalog catalog, string name) =>
            catalog.Modules.Any(module => module.ModuleName == name);

        /// <summary>
        /// Checks to see if the <see cref="IModule"/> is already initialized. 
        /// </summary>
        /// <returns><c>true</c>, if initialized, <c>false</c> otherwise.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <typeparam name="T">The <see cref="IModule"/> to check.</typeparam>
        public static bool IsInitialized<T>(this IModuleCatalog catalog)
            where T : IModule =>
            catalog.Modules.FirstOrDefault(mi => mi.ModuleType == typeof(T))?.State == ModuleState.Initialized;

        /// <summary>
        /// Checks to see if the <see cref="IModule"/> is already initialized. 
        /// </summary>
        /// <returns><c>true</c>, if initialized, <c>false</c> otherwise.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <param name="name">Name.</param>
        public static bool IsInitialized(this IModuleCatalog catalog, string name) =>
            catalog.Modules.FirstOrDefault(module => module.ModuleName == name)?.State == ModuleState.Initialized;
    }
}
