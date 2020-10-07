using System;
using System.IO;
using System.Windows.Markup;

namespace Prism.Modularity
{
    /// <summary>
    /// A catalog built from a XAML file.
    /// </summary>
    public class XamlModuleCatalog : ModuleCatalog
    {
        private readonly Uri _resourceUri;

        private const string _refFilePrefix = "file://";
        private int _refFilePrefixLength = _refFilePrefix.Length;

        /// <summary>
        /// Creates an instance of a XamlResourceCatalog.
        /// </summary>
        /// <param name="fileName">The name of the XAML file</param>
        public XamlModuleCatalog(string fileName)
            : this(new Uri(fileName, UriKind.Relative))
        {
        }

        /// <summary>
        /// Creates an instance of a XamlResourceCatalog.
        /// </summary>
        /// <param name="resourceUri">The pack url of the XAML file resource</param>
        public XamlModuleCatalog(Uri resourceUri)
        {
            _resourceUri = resourceUri;
        }

        /// <summary>
        /// Loads the catalog from the XAML file.
        /// </summary>
        protected override void InnerLoad()
        {
            var catalog = CreateFromXaml(_resourceUri);

            foreach (IModuleCatalogItem item in catalog.Items)
            {
                if (item is ModuleInfo mi)
                {
                    if (!string.IsNullOrWhiteSpace(mi.Ref))
                        mi.Ref = GetFileAbsoluteUri(mi.Ref);
                }
                else if (item is ModuleInfoGroup mg)
                {
                    if (!string.IsNullOrWhiteSpace(mg.Ref))
                    {
                        mg.Ref = GetFileAbsoluteUri(mg.Ref);
                        mg.UpdateModulesRef();
                    }
                    else
                    {
                        foreach (var module in mg)
                        {
                            module.Ref = GetFileAbsoluteUri(module.Ref);
                        }
                    }
                }

                Items.Add(item);
            }
        }

        /// <inheritdoc />
        protected override string GetFileAbsoluteUri(string path)
        {
            //this is to maintain backwards compatibility with the old file:/// and file:// syntax for Xaml module catalog Ref property
            if (path.StartsWith(_refFilePrefix + "/", StringComparison.Ordinal))
            {
                path = path.Substring(_refFilePrefixLength + 1);
            }
            else if (path.StartsWith(_refFilePrefix, StringComparison.Ordinal))
            {
                path = path.Substring(_refFilePrefixLength);
            }

            return base.GetFileAbsoluteUri(path);
        }

        /// <summary>
        /// Creates a <see cref="ModuleCatalog"/> from XAML.
        /// </summary>
        /// <param name="xamlStream"><see cref="Stream"/> that contains the XAML declaration of the catalog.</param>
        /// <returns>An instance of <see cref="ModuleCatalog"/> built from the XAML.</returns>
        private static ModuleCatalog CreateFromXaml(Stream xamlStream)
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
        private static ModuleCatalog CreateFromXaml(Uri builderResourceUri)
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
