using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prism.Properties;

namespace Prism.Modularity
{
    /// <summary>
    /// Component responsible for coordinating the modules' type loading and module initialization process.
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        /// <summary>
        /// The module catalog.
        /// </summary>
        protected IModuleCatalog ModuleCatalog { get; }

        /// <summary>
        /// Gets all the <see cref="IModuleInfo"/> classes that are in the <see cref="IModuleCatalog"/>.
        /// </summary>
        public IEnumerable<IModuleInfo> Modules => ModuleCatalog.Modules;

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        /// <summary>
        /// Not used by Prism.Forms
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <summary>
        /// The module initializer.
        /// </summary>
        protected IModuleInitializer ModuleInitializer { get; }

        /// <summary>
        /// Initializes an instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Service used for initialization of modules.</param>
        /// <param name="moduleCatalog">Catalog that enumerates the modules to be loaded and initialized.</param>
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog)
        {
            ModuleInitializer = moduleInitializer ?? throw new ArgumentNullException(nameof(moduleInitializer));
            ModuleCatalog = moduleCatalog ?? throw new ArgumentNullException(nameof(moduleCatalog));
        }

        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> in the <see cref="ModuleCatalog"/>.
        /// </summary>
        public void Run()
        {
            LoadModulesWhenAvailable();
        }

        /// <summary>
        /// Loads and initializes the module in the <see cref="IModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        public void LoadModule(string moduleName)
        {
            var modules = ModuleCatalog.Modules
                .Where(m => m.ModuleName == moduleName)
                .ToArray();
            if (!modules.Any())
            {
                throw new ModuleNotFoundException(moduleName, string.Format(CultureInfo.CurrentCulture, Resources.ModuleNotFound, moduleName));
            }
            else if (modules.Length > 1)
            {
                throw new DuplicateModuleException(moduleName, string.Format(CultureInfo.CurrentCulture, Resources.DuplicatedModuleInCatalog, moduleName));
            }

            var modulesToLoad = ModuleCatalog.CompleteListWithDependencies(modules);

            LoadModules(modulesToLoad);
        }

        /// <summary>
        /// Loads the <see cref="IModule"/>'s with <see cref="InitializationMode.WhenAvailable"/>
        /// </summary>
        protected void LoadModulesWhenAvailable()
        {
            var whenAvailableModules = ModuleCatalog.Modules.Where(m => m.InitializationMode == InitializationMode.WhenAvailable && m.State == ModuleState.NotStarted);
            LoadModules(whenAvailableModules);
        }

        /// <summary>
        /// Loads the specified modules.
        /// </summary>
        /// <param name="moduleInfos"><see cref="IModuleInfo"/>.</param>
        protected virtual void LoadModules(IEnumerable<IModuleInfo> moduleInfos)
        {
            foreach (var moduleInfo in moduleInfos)
            {
                if (moduleInfo.State == ModuleState.NotStarted)
                {
                    try
                    {
                        moduleInfo.State = ModuleState.Initializing;
                        ModuleInitializer.Initialize(moduleInfo);
                        moduleInfo.State = ModuleState.Initialized;
                        RaiseLoadModuleCompleted(moduleInfo);
                    }
                    catch (Exception ex)
                    {
                        RaiseLoadModuleCompleted(moduleInfo, ex);
                    }

                }
            }
        }

        /// <summary>
        /// Raises the <see cref="LoadModuleCompleted"/> event.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="IModuleInfo"/> that was just loaded.</param>
        /// <param name="ex">An <see cref="Exception"/> if any that was thrown during the loading of the <see cref="IModule"/></param>
        protected void RaiseLoadModuleCompleted(IModuleInfo moduleInfo, Exception ex = null)
        {
            LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(moduleInfo, ex));
        }
    }
}
