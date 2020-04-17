using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    /// <summary>
    /// Set of properties for each Module
    /// </summary>
public interface IModuleInfo : IModuleCatalogItem
    {
        /// <summary>
        /// The modules this instance depends on.
        /// </summary>
        Collection<string> DependsOn { get; set; }
        /// <summary>
        /// When Prism should Initialize the module
        /// <see cref="InitializationMode"/>
        /// </summary>
        InitializationMode InitializationMode { get; set; }
        /// <summary>
        /// The name of the module
        /// </summary>
        string ModuleName { get; set; }
        /// <summary>
        /// The module's type
        /// </summary>
        string ModuleType { get; set; }
        /// <summary>
        /// A string ref for the module for registration/resolution
        /// </summary>
        string Ref { get; set; }
        /// <summary>
        /// An enum that lists the modules state
        /// <see cref="ModuleState"/>
        /// </summary>
        ModuleState State { get; set; }
    }
}