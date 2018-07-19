using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    public interface IModuleInfo : IModuleCatalogItem
    {
        Collection<string> DependsOn { get; set; }
        InitializationMode InitializationMode { get; set; }
        string ModuleName { get; set; }
        string ModuleType { get; set; }
        string Ref { get; set; }
        ModuleState State { get; set; }
    }
}