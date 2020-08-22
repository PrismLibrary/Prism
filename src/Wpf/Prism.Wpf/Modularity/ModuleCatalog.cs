using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the
    /// name, type and location of the module.
    ///
    /// It also verifies that the <see cref="ModuleCatalog"/> is internally valid. That means that
    /// it does not have:
    /// <list>
    ///     <item>Circular dependencies</item>
    ///     <item>Missing dependencies</item>
    ///     <item>
    ///         Invalid dependencies, such as a Module that's loaded at startup that depends on a module
    ///         that might need to be retrieved.
    ///     </item>
    /// </list>
    /// The <see cref="ModuleCatalog"/> also serves as a baseclass for more specialized Catalogs .
    /// </summary>
    [ContentProperty("Items")]
    public class ModuleCatalog : ModuleCatalogBase, IModuleGroupsCatalog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalog() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class while providing an
        /// initial list of <see cref="ModuleInfo"/>s.
        /// </summary>
        /// <param name="modules">The initial list of modules.</param>
        public ModuleCatalog(IEnumerable<ModuleInfo> modules) : base(modules)
        {
        }

        /// <summary>
        /// Creates a valid file uri to locate the module assembly file
        /// </summary>
        /// <param name="filePath">The relative path to the file</param>
        /// <returns>The valid absolute file path</returns>
        protected virtual string GetFileAbsoluteUri(string filePath)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = String.Empty;
            uriBuilder.Scheme = Uri.UriSchemeFile;
            uriBuilder.Path = Path.GetFullPath(filePath);
            Uri fileUri = uriBuilder.Uri;

            return fileUri.ToString();
        }
    }
}
