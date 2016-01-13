using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Modularity
{
    public class ModuleCatalog : IModuleCatalog
    {
        private readonly List<ModuleInfo> _items = new List<ModuleInfo>();

        public IEnumerable<ModuleInfo> Modules
        {
            get { return _items; }
        }

        public ModuleCatalog AddModule(Type moduleType)
        {
            _items.Add(new ModuleInfo(moduleType));
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

        public virtual void Initialize()
        {
            ValidateUniqueModules();
        }
    }
}
