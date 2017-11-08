

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using CommonServiceLocator;
using Prism.Logging;
using Prism.Modularity;

namespace Prism.Mef.Modularity
{
    /// <summary>
    /// Exports the ModuleInitializer using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IModuleInitializer))]
    public class MefModuleInitializer : ModuleInitializer
    {
        private DownloadedPartCatalogCollection downloadedPartCatalogs;
        private AggregateCatalog aggregateCatalog { get; set; }

        // disable the warning that the field is never assigned to, and will always have its default value null
        // as it is imported by MEF
#pragma warning disable 0649

        /// <summary>
        /// Import the available modules from the MEF container
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<IModule, IModuleExport>> ImportedModules { get; set; }
#pragma warning restore 0649

        /// <summary>
        /// Initializes a new instance of the <see cref="MefModuleInitializer"/> class.
        /// </summary>
        /// <param name="serviceLocator">The container that will be used to resolve the modules by specifying its type.</param>
        /// <param name="loggerFacade">The logger to use.</param>
        /// <param name="downloadedPartCatalogs">The downloaded part catalogs.</param>
        /// <param name="aggregateCatalog">The aggregate catalog.</param>
        [ImportingConstructor()]
        public MefModuleInitializer(IServiceLocator serviceLocator, ILoggerFacade loggerFacade, DownloadedPartCatalogCollection downloadedPartCatalogs, AggregateCatalog aggregateCatalog)
            : base(serviceLocator, loggerFacade)
        {
            if (downloadedPartCatalogs == null)
            {
                throw new ArgumentNullException(nameof(downloadedPartCatalogs));
            }

            if (aggregateCatalog == null)
            {
                throw new ArgumentNullException(nameof(aggregateCatalog));
            }

            this.downloadedPartCatalogs = downloadedPartCatalogs;
            this.aggregateCatalog = aggregateCatalog;
        }

        /// <summary>
        /// Uses the container to resolve a new <see cref="IModule"/> by specifying its <see cref="Type"/>.
        /// </summary>
        /// <param name="moduleInfo">The module to create.</param>
        /// <returns>
        /// A new instance of the module specified by <paramref name="moduleInfo"/>.
        /// </returns>
        protected override IModule CreateModule(ModuleInfo moduleInfo)
        {
            // If there is a catalog that needs to be integrated with the AggregateCatalog as part of initialization, I add it to the container's catalog.
            ComposablePartCatalog partCatalog;
            if (this.downloadedPartCatalogs.TryGet(moduleInfo, out partCatalog))
            {
                if (!this.aggregateCatalog.Catalogs.Contains(partCatalog))
                {
                    this.aggregateCatalog.Catalogs.Add(partCatalog);
                }

                this.downloadedPartCatalogs.Remove(moduleInfo);
            }

            if (this.ImportedModules != null && this.ImportedModules.Count() != 0)
            {
                Lazy<IModule, IModuleExport> lazyModule =
                    this.ImportedModules.FirstOrDefault(x => (x.Metadata.ModuleName == moduleInfo.ModuleName));
                if (lazyModule != null)
                {
                    return lazyModule.Value;
                }
            }

            // This does not fall back to the base implementation because the type must be in the MEF container and not just in the application domain.
            throw new ModuleInitializeException(
                string.Format(CultureInfo.CurrentCulture, Properties.Resources.FailedToGetType, moduleInfo.ModuleType));
        }
    }
}