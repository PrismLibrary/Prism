

namespace Prism.Modularity
{
    /// <summary>
    /// Defines a store for the module metadata.
    /// </summary>
    public interface IConfigurationStore
    {
        /// <summary>
        /// Gets the module configuration data.
        /// </summary>
        /// <returns>A <see cref="ModulesConfigurationSection"/> instance.</returns>
        ModulesConfigurationSection RetrieveModuleConfigurationSection();
    }
}