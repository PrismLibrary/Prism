using System;

namespace Prism.Modularity
{
    /// <summary>
    /// A catalog built from a XAML file.
    /// </summary>
    public class XamlResourceCatalog : ModuleCatalog
    {
        private readonly Uri _resourceUri;

        /// <summary>
        /// Creates an instance of a XamlResourceCatalog.
        /// </summary>
        /// <param name="fileName">The name of the XAML file</param>
        public XamlResourceCatalog(string fileName)
            : this(new Uri(fileName, UriKind.Relative))
        {
        }

        /// <summary>
        /// Creates an instance of a XamlResourceCatalog.
        /// </summary>
        /// <param name="resourceUri">The pack url of the XAML file resource</param>
        public XamlResourceCatalog(Uri resourceUri)
        {
            _resourceUri = resourceUri;
        }

        /// <summary>
        /// Loads the catalog from the XAML file.
        /// </summary>
        protected override void InnerLoad()
        {
            var catalog = ModuleCatalog.CreateFromXaml(_resourceUri);

            foreach(IModuleCatalogItem item in catalog.Items)
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
                }

                Items.Add(item);
            }
        }
    }
}
