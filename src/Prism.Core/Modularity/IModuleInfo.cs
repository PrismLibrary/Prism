using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    /// <summary>
    /// Set of properties for each Module
    /// </summary>
    public interface IModuleInfo : IModuleCatalogItem
    {
        /// <summary>
        /// The module names this instance depends on.
        /// </summary>
        Collection<string> DependsOn { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="InitializationMode" />
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
        /// A string ref is a location reference to load the module as it may not be already loaded in the Appdomain in some cases may need to be downloaded.
        /// </summary>
        /// <Remarks>
        /// This is only used for WPF
        /// </Remarks>
        string Ref { get; set; }

        /// <summary>
        /// Gets or Sets the current <see cref="ModuleState" />
        /// </summary>
        ModuleState State { get; set; }
    }
}
