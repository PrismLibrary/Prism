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
        readonly IModuleInitializer _moduleInitializer;

        /// <summary>
        /// The module catalog.
        /// </summary>
        protected IModuleCatalog ModuleCatalog
        {
            get { return _moduleCatalog; }
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ModuleManager"/> class.
        /// </summary>
        /// <param name="moduleInitializer">Service used for initialization of modules.</param>
        /// <param name="moduleCatalog">Catalog that enumerates the modules to be loaded and initialized.</param>
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog)
        {
            if (moduleInitializer == null)
                throw new ArgumentNullException("moduleInitializer");

            if (moduleCatalog == null)
                throw new ArgumentNullException("moduleCatalog");

            _moduleInitializer = moduleInitializer;
            _moduleCatalog = moduleCatalog;
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
                throw new Exception(String.Format("Module {0} was not found in the catalog.", moduleName));

            if (modules.Count() != 1)
                throw new Exception(String.Format("A duplicated module with name {0} has been found in the catalog.", moduleName));

            LoadModules(modules);
        }

        void LoadModulesWhenAvailable()
        {
            var whenAvailableModules = ModuleCatalog.Modules.Where(m => m.InitializationMode == InitializationMode.WhenAvailable);
            if (whenAvailableModules != null)
                LoadModules(whenAvailableModules);
        }

        void LoadModules(IEnumerable<ModuleInfo> moduleInfos)
        {
            foreach (var moduleInfo in moduleInfos)
            {
                if (moduleInfo.State != ModuleState.Initialized)
                {
                    moduleInfo.State = ModuleState.Initializing;
                    _moduleInitializer.Initialize(moduleInfo);
                    moduleInfo.State = ModuleState.Initialized;
                }
            }
        }
    }
}
