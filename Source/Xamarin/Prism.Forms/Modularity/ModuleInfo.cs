using System;

namespace Prism.Modularity
{
    public class ModuleInfo
    {
        public string ModuleName { get; set; }

        public Type ModuleType { get; set; }

        public ModuleInfo(Type moduleType)
        {
            ModuleType = moduleType;
            ModuleName = moduleType.Name;
        }
    }
}
