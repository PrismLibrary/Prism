using System.Collections;
using System.Collections.Generic;

namespace Prism.Modularity
{
    // IList must be supported in Silverlight 2 to be able to add items from XAML
    /// <summary>
    /// A collection of <see cref="ModuleInfo"/> for the Modules used by the application
    /// </summary>
    public interface IModuleInfoGroup : IModuleCatalogItem, IList<IModuleInfo>, IList
    {
        /// <summary>
        /// When Prism should Initialize the module
        /// <see cref="InitializationMode"/>
        /// </summary>
        InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// A string ref for the module for registration/resolution
        /// </summary>
        string Ref { get; set; }
    }
}
