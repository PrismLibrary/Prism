using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the 
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the 
    /// name and type of the module. 
    /// </summary>
    public class ModuleCatalog : IModuleCatalog
    {
        private readonly List<ModuleInfo> _items = new List<ModuleInfo>();

        /// <summary>
        /// Gets all the <see cref="ModuleInfo"/> classes that are in the <see cref="ModuleCatalog"/>.
        /// </summary>
        public IEnumerable<ModuleInfo> Modules
        {
            get { return _items; }
        }

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to add.</param>
        /// <returns>The <see cref="ModuleCatalog"/> for easily adding multiple modules.</returns>
        public IModuleCatalog AddModule(ModuleInfo moduleInfo)
        {
            if (_items.Any(mi => mi.ModuleName == moduleInfo.ModuleName))
                throw new Exception($"A duplicated module with name {moduleInfo.ModuleName} has already been added.");

            if (_items.Any(mi => mi.ModuleType == moduleInfo.ModuleType))
                throw new Exception($"A duplicate module of type {moduleInfo.ModuleType.Name} has already been added.");

            _items.Add(moduleInfo);
            return this;
        }

        /// <summary>
        /// Makes sure all modules have an Unique name. 
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the names of one or more modules are not unique. 
        /// </exception>
        protected virtual void ValidateUniqueModules()
        {
            List<string> moduleNames = this.Modules.Select(m => m.ModuleName).ToList();

            string duplicateModule = moduleNames.FirstOrDefault(m => moduleNames.Count(m2 => m2 == m) > 1);

            if (duplicateModule != null)
                throw new Exception(String.Format("A duplicated module with name {0} has been found by the loader.", duplicateModule));
        }

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        public virtual void Initialize()
        {
            
        }
    }
}
