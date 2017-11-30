using System;
using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    public static class IModuleCatalogExtensions
    {
        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <returns>The module.</returns>
        /// <param name="catalog">Catalog</param>
        /// <param name="mode"><see cref="InitializationMode"/></param>
        /// <typeparam name="T">The <see cref="IModule"/> type parameter.</typeparam>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, InitializationMode mode = InitializationMode.WhenAvailable, params string[] dependsOn)
            where T : IModule
        {
            return catalog.AddModule<T>(typeof(T).Name, mode, dependsOn);
        }

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <returns>The module.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <param name="name">Name.</param>
        /// <param name="mode"><see cref="IModule"/>.</param>
        /// <typeparam name="T">The <see cref="IModule"/> type parameter.</typeparam>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, string name, InitializationMode mode = InitializationMode.WhenAvailable, params string[] dependsOn)
            where T : IModule
        {
            return catalog.AddModule(name, typeof(T).AssemblyQualifiedName, mode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public static IModuleCatalog AddModule(this IModuleCatalog catalog, Type moduleType, params string[] dependsOn)
        {
            return catalog.AddModule(moduleType, InitializationMode.WhenAvailable, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public static IModuleCatalog AddModule(this IModuleCatalog catalog, Type moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));

            return catalog.AddModule(moduleType.Name, moduleType.AssemblyQualifiedName, initializationMode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public static IModuleCatalog AddModule(this IModuleCatalog catalog, string moduleName, string moduleType, params string[] dependsOn)
        {
            return catalog.AddModule(moduleName, moduleType, InitializationMode.WhenAvailable, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public static IModuleCatalog AddModule(this IModuleCatalog catalog, string moduleName, string moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            return catalog.AddModule(moduleName, moduleType, null, initializationMode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="refValue">Reference to the location of the module to be added assembly.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public static IModuleCatalog AddModule(this IModuleCatalog catalog, string moduleName, string moduleType, string refValue, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));

            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfo.InitializationMode = initializationMode;
            moduleInfo.Ref = refValue;
            catalog.AddModule(moduleInfo);
            return catalog;
        }
    }
}
