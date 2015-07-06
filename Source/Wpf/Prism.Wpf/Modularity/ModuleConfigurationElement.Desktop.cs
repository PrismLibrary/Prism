

using System.Configuration;

namespace Prism.Modularity
{
    /// <summary>
    /// A configuration element to declare module metadata.
    /// </summary>
    public class ModuleConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ModuleConfigurationElement"/>.
        /// </summary>
        public ModuleConfigurationElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleConfigurationElement"/>.
        /// </summary>
        /// <param name="assemblyFile">The assembly file where the module is located.</param>
        /// <param name="moduleType">The type of the module.</param>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="startupLoaded">This attribute specifies whether the module is loaded at startup.</param>
        public ModuleConfigurationElement(string assemblyFile, string moduleType, string moduleName, bool startupLoaded)
        {
            base["assemblyFile"] = assemblyFile;
            base["moduleType"] = moduleType;
            base["moduleName"] = moduleName;
            base["startupLoaded"] = startupLoaded;
        }

        /// <summary>
        /// Gets or sets the assembly file.
        /// </summary>
        /// <value>The assembly file.</value>
        [ConfigurationProperty("assemblyFile", IsRequired = true)]
        public string AssemblyFile
        {
            get { return (string)base["assemblyFile"]; }
            set { base["assemblyFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the module type.
        /// </summary>
        /// <value>The module's type.</value>
        [ConfigurationProperty("moduleType", IsRequired = true)]
        public string ModuleType
        {
            get { return (string)base["moduleType"]; }
            set { base["moduleType"] = value; }
        }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        /// <value>The module's name.</value>
        [ConfigurationProperty("moduleName", IsRequired = true)]
        public string ModuleName
        {
            get { return (string)base["moduleName"]; }
            set { base["moduleName"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the module should be loaded at startup.
        /// </summary>
        /// <value>A value indicating whether the module should be loaded at startup.</value>
        [ConfigurationProperty("startupLoaded", IsRequired = false, DefaultValue = true)]
        public bool StartupLoaded
        {
            get { return (bool)base["startupLoaded"]; }
            set { base["startupLoaded"] = value; }
        }

        /// <summary>
        /// Gets or sets the modules this module depends on.
        /// </summary>
        /// <value>The names of the modules that this depends on.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ConfigurationProperty("dependencies", IsDefaultCollection = true, IsKey = false)]
        public ModuleDependencyCollection Dependencies
        {
            get { return (ModuleDependencyCollection)base["dependencies"]; }
            set { base["dependencies"] = value; }
        }
    }
}