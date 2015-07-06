

using System.Configuration;

namespace Prism.Modularity
{
    /// <summary>
    /// A <see cref="ConfigurationSection"/> for module configuration.
    /// </summary>
    public class ModulesConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the collection of modules configuration.
        /// </summary>
        /// <value>A <see cref="ModuleConfigurationElementCollection"/> of <see cref="ModuleConfigurationElement"/>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ModuleConfigurationElementCollection Modules
        {
            get { return (ModuleConfigurationElementCollection)base[""]; }
            set { base[""] = value; }
        }
    }
}