using System;
using System.Collections.Generic;

namespace Prism.Modularity
{
    public interface IModuleCatalog
    {
        IEnumerable<ModuleInfo> Modules { get; }

        ModuleCatalog AddModule(Type moduleType);

        void Initialize();
    }
}
