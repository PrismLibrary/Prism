

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace Prism.Mef
{
    ///<summary>
    /// DefaultPrismServiceRegistrationAgent allows the Prism required types to be registered if necessary.
    ///</summary>
    public static class DefaultPrismServiceRegistrar
    {
        /// <summary>
        /// Registers the required Prism types that are not already registered in the <see cref="AggregateCatalog"/>.
        /// </summary>
        ///<param name="aggregateCatalog">The <see cref="AggregateCatalog"/> to register the required types in, if they are not already registered.</param>
        public static AggregateCatalog RegisterRequiredPrismServicesIfMissing(AggregateCatalog aggregateCatalog)
        {
            if (aggregateCatalog == null) throw new ArgumentNullException(nameof(aggregateCatalog));
            IEnumerable<ComposablePartDefinition> partsToRegister =
                GetRequiredPrismPartsToRegister(aggregateCatalog);

            PrismDefaultsCatalog cat = new PrismDefaultsCatalog(partsToRegister);
            aggregateCatalog.Catalogs.Add(cat);
            return aggregateCatalog;
        }

        private static IEnumerable<ComposablePartDefinition> GetRequiredPrismPartsToRegister(AggregateCatalog aggregateCatalog)
        {
            List<ComposablePartDefinition> partsToRegister = new List<ComposablePartDefinition>();
            var catalogWithDefaults = GetDefaultComposablePartCatalog();
            foreach (var part in catalogWithDefaults.Parts)
            {
                foreach (var export in part.ExportDefinitions)
                {
                    bool exportAlreadyRegistered = false;
                    foreach (var registeredPart in aggregateCatalog.Parts)
                    {
                        foreach (var registeredExport in registeredPart.ExportDefinitions)
                        {
                            if (string.Compare(registeredExport.ContractName, export.ContractName, StringComparison.Ordinal) == 0)
                            {
                                exportAlreadyRegistered = true;
                                break;
                            }
                        }
                    }

                    if (exportAlreadyRegistered != false) continue;
                    if (!partsToRegister.Contains(part))
                    {
                        partsToRegister.Add(part);
                    }
                }
            }
            return partsToRegister;
        }

        /// <summary>
        /// Returns an <see cref="AssemblyCatalog" /> for the current assembly
        /// </summary>
        /// <remarks>
        /// To ensure that the calling assembly is this one, the call is in this
        /// private helper method.
        /// </remarks>
        /// <returns>
        /// Returns an <see cref="AssemblyCatalog" /> for the current assembly
        /// </returns>
        private static ComposablePartCatalog GetDefaultComposablePartCatalog()
        {
            return new AssemblyCatalog(Assembly.GetAssembly(typeof(MefBootstrapper)));
        }
    }
}