using System.Collections;
using System.Collections.Generic;

namespace Prism.Modularity
{
    // IList must be supported in Silverlight 2 to be able to add items from XAML
    /// <summary>
    /// A collection of <see cref="IModuleInfo"/> for the Modules used by the application
    /// </summary>
    public interface IModuleInfoGroup : IModuleCatalogItem, IList<IModuleInfo>, IList
    {
        /// <summary>
        /// When Prism should Initialize the module
        /// <see cref="InitializationMode"/>
        /// </summary>
        InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// A string ref is a location reference to load the module as it may not be already loaded in the Appdomain in some cases may need to be downloaded.
        /// </summary>
        /// <Remarks>
        /// This is only used for WPF
        /// </Remarks>
        string Ref { get; set; }
    }
}
