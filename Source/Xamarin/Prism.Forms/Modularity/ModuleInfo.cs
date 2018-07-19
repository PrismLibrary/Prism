using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the metadata that describes a module.
    /// </summary>
    public partial class ModuleInfo : IModuleInfo
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="ModuleInfo"/>.
        /// </summary>
        public ModuleInfo()
            : this(null, null, new string[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="name">The module's name.</param>
        /// <param name="type">The module <see cref="Type"/>'s AssemblyQualifiedName.</param>
        /// <param name="dependsOn">The modules this instance depends on.</param>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="dependsOn"/> is <see langword="null"/>.</exception>
        public ModuleInfo(string name, string type, params string[] dependsOn)
        {
            if (Type.GetType(type) == null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (dependsOn == null)
                throw new ArgumentNullException(nameof(dependsOn));

            ModuleName = name;
            ModuleType = type;
            foreach (string dependency in dependsOn)
            {
                if (!DependsOn.Contains(dependency))
                {
                    DependsOn.Add(dependency);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="name">The module's name.</param>
        /// <param name="type">The module's type.</param>
        public ModuleInfo(string name, string type) : this(name, type, new string[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleType">The module's type.</param>
        public ModuleInfo(Type moduleType)
            : this(moduleType, moduleType.Name) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleType">The module's type.</param>
        /// <param name="moduleName">The module's name.</param>
        public ModuleInfo(Type moduleType, string moduleName)
            : this(moduleType, moduleName, InitializationMode.WhenAvailable) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleType">The module's type.</param>
        /// <param name="moduleName">The module's name.</param>
        /// <param name="initializationMode">The module's <see cref="InitializationMode"/>.</param>
        public ModuleInfo(Type moduleType, string moduleName, InitializationMode initializationMode)
            : this(moduleName, moduleType.AssemblyQualifiedName)
        {
            InitializationMode = initializationMode;
        }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the module <see cref="Type"/>'s AssemblyQualifiedName.
        /// </summary>
        /// <value>The type of the module.</value>
        public string ModuleType { get; set; }

        private Collection<string> _dependsOn;
        /// <summary>
        /// Gets or sets the list of modules that this module depends upon.
        /// </summary>
        /// <value>The list of modules that this module depends upon.</value>
        public Collection<string> DependsOn
        {
            get
            {
                if(_dependsOn == null)
                {
                    _dependsOn = new Collection<string>();
                    var moduleType = Type.GetType(ModuleType);
                    foreach(var dependencyAttribute in moduleType.GetTypeInfo().GetCustomAttributes<ModuleDependencyAttribute>())
                    {
                        _dependsOn.Add(dependencyAttribute.ModuleName);
                    }
                }

                return _dependsOn;
            }
            set => _dependsOn = value;
        }

        /// <summary>
        /// Specifies on which stage the Module will be initialized.
        /// </summary>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Reference to the location of the module assembly.
        /// <example>The following are examples of valid <see cref="ModuleInfo.Ref"/> values:
        /// file://c:/MyProject/Modules/MyModule.dll for a loose DLL in WPF.
        /// </example>
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Gets or sets the state of the <see cref="ModuleInfo"/> with regards to the module loading and initialization process.
        /// </summary>
        public ModuleState State { get; set; }
    }
}
