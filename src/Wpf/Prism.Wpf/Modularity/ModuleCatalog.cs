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
        /// Creates a <see cref="ModuleCatalog"/> from XAML.
        /// </summary>
        /// <param name="xamlStream"><see cref="Stream"/> that contains the XAML declaration of the catalog.</param>
        /// <returns>An instance of <see cref="ModuleCatalog"/> built from the XAML.</returns>
        public static ModuleCatalog CreateFromXaml(Stream xamlStream)
        {
            if (xamlStream == null)
            {
                throw new ArgumentNullException(nameof(xamlStream));
            }

            return XamlReader.Load(xamlStream) as ModuleCatalog;
        }

        /// <summary>
        /// Creates a <see cref="ModuleCatalog"/> from a XAML included as an Application Resource.
        /// </summary>
        /// <param name="builderResourceUri">Relative <see cref="Uri"/> that identifies the XAML included as an Application Resource.</param>
        /// <returns>An instance of <see cref="ModuleCatalog"/> build from the XAML.</returns>
        public static ModuleCatalog CreateFromXaml(Uri builderResourceUri)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(builderResourceUri);

            if ((streamInfo != null) && (streamInfo.Stream != null))
            {
                return CreateFromXaml(streamInfo.Stream);
            }

            return null;
        }
    }
}
