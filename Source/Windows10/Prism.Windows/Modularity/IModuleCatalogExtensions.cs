using System;
using System.Collections.Generic;
using System.Text;

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

        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, string name)
            where T : IModule =>
            catalog.AddModule<T>(name, InitializationMode.WhenAvailable);

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <returns>The module.</returns>
        /// <param name="catalog">Catalog.</param>
        /// <param name="name">Name.</param>
        /// <param name="mode"><see cref="IModule"/>.</param>
        /// <typeparam name="T">The <see cref="IModule"/> type parameter.</typeparam>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, string name, InitializationMode mode)
            where T : IModule =>
            catalog.AddModule(new ModuleInfo(typeof(T), name, mode));

        /// <summary>
        /// Adds the <see cref="IModule"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IModule"/></typeparam>
        /// <param name="catalog">The <see cref="IModuleCatalog"/> to add the <see cref="IModule"/> to.</param>
        /// <param name="dependsOn">The names of the <see cref="IModule"/>'s that should be loaded when this <see cref="IModule"/> is loaded.</param>
        /// <returns>The <see cref="IModuleCatalog"/></returns>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, params string[] dependsOn)
            where T : IModule => catalog.AddModule<T>(InitializationMode.WhenAvailable, dependsOn);

        /// <summary>
        /// Adds the <see cref="IModule"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IModule"/></typeparam>
        /// <param name="catalog">The <see cref="IModuleCatalog"/> to add the <see cref="IModule"/> to.</param>
        /// <param name="name">The name of the <see cref="IModule"/></param>
        /// <param name="dependsOn">The names of the <see cref="IModule"/>'s that should be loaded when this <see cref="IModule"/> is loaded.</param>
        /// <returns>The <see cref="IModuleCatalog"/></returns>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, string name, params string[] dependsOn)
            where T : IModule =>
            catalog.AddModule<T>(name, InitializationMode.WhenAvailable, dependsOn);

        /// <summary>
        /// Adds the <see cref="IModule"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IModule"/></typeparam>
        /// <param name="catalog">The <see cref="IModuleCatalog"/> to add the <see cref="IModule"/> to.</param>
        /// <param name="mode"></param>
        /// <param name="dependsOn">The names of the <see cref="IModule"/>'s that should be loaded when this <see cref="IModule"/> is loaded.</param>
        /// <returns>The <see cref="IModuleCatalog"/></returns>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, InitializationMode mode, params string[] dependsOn)
            where T : IModule =>
            catalog.AddModule<T>(typeof(T).Name, mode, dependsOn);

        /// <summary>
        /// Adds the <see cref="IModule"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IModule"/></typeparam>
        /// <param name="catalog">The <see cref="IModuleCatalog"/> to add the <see cref="IModule"/> to.</param>
        /// <param name="name">The name of the <see cref="IModule"/></param>
        /// <param name="mode">The <see cref="InitializationMode"/></param>
        /// <param name="dependsOn">The names of the <see cref="IModule"/>'s that should be loaded when this <see cref="IModule"/> is loaded.</param>
        /// <returns>The <see cref="IModuleCatalog"/></returns>
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog, string name, InitializationMode mode, params string[] dependsOn)
            where T : IModule
        {
            var moduleInfo = new ModuleInfo(name, typeof(T).AssemblyQualifiedName, dependsOn)
            {
                InitializationMode = mode
            };
            return catalog.AddModule(moduleInfo);
        }
    }
}
