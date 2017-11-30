﻿using System;
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
    }
}
