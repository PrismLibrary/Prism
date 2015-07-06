

using System.Configuration;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines a store for the module metadata.
    /// </summary>
    public class ConfigurationStore : IConfigurationStore
    {
        /// <summary>
        /// Gets the module configuration data.
        /// </summary>
        /// <returns>A <see cref="ModulesConfigurationSection"/> instance.</returns>
        public ModulesConfigurationSection RetrieveModuleConfigurationSection()
        {
            return ConfigurationManager.GetSection("modules") as ModulesConfigurationSection;
        }
    }
}