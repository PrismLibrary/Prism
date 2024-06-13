using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Properties;

namespace Prism.Modularity
{

    /// <summary>
    /// A catalog built from a configuration file.
    /// </summary>
    public class ConfigurationModuleCatalog : ModuleCatalog
    {
        /// <summary>
        /// Builds an instance of ConfigurationModuleCatalog with a <see cref="ConfigurationStore"/> as the default store.
        /// </summary>
        public ConfigurationModuleCatalog()
        {
            Store = new ConfigurationStore();
        }

        /// <summary>
        /// Gets or sets the store where the configuration is kept.
        /// </summary>
        public IConfigurationStore Store { get; set; }

        /// <summary>
        /// Loads the catalog from the configuration.
        /// </summary>
        protected override void InnerLoad()
        {
            if (Store == null)
            {
                throw new InvalidOperationException(Resources.ConfigurationStoreCannotBeNull);
            }

            EnsureModulesDiscovered();
        }

        private void EnsureModulesDiscovered()
        {
            ModulesConfigurationSection section = Store.RetrieveModuleConfigurationSection();

            if (section != null)
            {
                foreach (ModuleConfigurationElement element in section.Modules)
                {
                    IList<string> dependencies = new List<string>();

                    if (element.Dependencies.Count > 0)
                    {
                        foreach (ModuleDependencyConfigurationElement dependency in element.Dependencies)
                        {
                            dependencies.Add(dependency.ModuleName);
                        }
                    }

                    ModuleInfo moduleInfo = new ModuleInfo(element.ModuleName, element.ModuleType)
                    {
                        Ref = GetFileAbsoluteUri(element.AssemblyFile),
                        InitializationMode = element.StartupLoaded ? InitializationMode.WhenAvailable : InitializationMode.OnDemand
                    };
                    moduleInfo.DependsOn.AddRange(dependencies.ToArray());
                    AddModule(moduleInfo);
                }
            }
        }
    }
}
