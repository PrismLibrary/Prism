using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Modularity
{
    /// <summary>
    /// Component responsible for coordinating the modules' type loading and module initialization process. 
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        readonly IModuleCatalog _moduleCatalog;
        /// <summary>
        /// The module catalog.
        /// </summary>
        protected IModuleCatalog ModuleCatalog => _moduleCatalog;

        readonly IModuleInitializer _moduleInitializer;
        /// <summary>
        /// The module initializer.
        /// </summary>
        protected IModuleInitializer ModuleInitializer => _moduleInitializer;

        /// <summary>
        /// Initializes an instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Service used for initialization of modules.</param>
        /// <param name="moduleCatalog">Catalog that enumerates the modules to be loaded and initialized.</param>
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog)
        {
            _moduleInitializer = moduleInitializer ?? throw new ArgumentNullException(nameof(moduleInitializer));
            _moduleCatalog = moduleCatalog ?? throw new ArgumentNullException(nameof(moduleCatalog));
        }

        /// <summary>
        /// Initializes the modules marked as <see cref="InitializationMode.WhenAvailable"/> in the <see cref="ModuleCatalog"/>.
        /// </summary>
        public void Run()
        {
            _moduleCatalog.Initialize();
            LoadModulesWhenAvailable();
        }

        /// <summary>
        /// Loads and initializes the module in the <see cref="ModuleCatalog"/> with the name <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">Name of the module requested for initialization.</param>
        public void LoadModule(string moduleName)
        {
            var modules = ModuleCatalog.Modules.Where(m => m.ModuleName == moduleName);
            if (modules == null || modules.Count() == 0)
                throw new Exception($"Module {moduleName} was not found in the catalog.");

            if (modules.Count() != 1)
                throw new Exception($"A duplicated module with name {moduleName} has been found in the catalog.");

            LoadModules(modules);
        }

        /// <summary>
        /// Loads the <see cref="IModule"/>'s with <see cref="InitializationMode.WhenAvailable"/>
        /// </summary>
        protected void LoadModulesWhenAvailable()
        {
            var whenAvailableModules = ModuleCatalog.Modules.Where(m => m.InitializationMode == InitializationMode.WhenAvailable && m.State == ModuleState.NotStarted);
            if (whenAvailableModules != null)
                LoadModules(whenAvailableModules);
        }

        /// <summary>
        /// Loads the specified modules.
        /// </summary>
        /// <param name="moduleInfos"><see cref="ModuleInfo"/>.</param>
        protected virtual void LoadModules(IEnumerable<ModuleInfo> moduleInfos)
        {
            foreach (var moduleInfo in moduleInfos)
            {
                if (moduleInfo.State == ModuleState.NotStarted)
                {
                    moduleInfo.State = ModuleState.Initializing;
                    _moduleInitializer.Initialize(moduleInfo);
                    moduleInfo.State = ModuleState.Initialized;
                }
            }
        }
    }
}
