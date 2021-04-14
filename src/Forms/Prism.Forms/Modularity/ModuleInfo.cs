using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Prism.Mvvm;
using Prism.Properties;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines the metadata that describes a module.
    /// </summary>
#if HAS_UWP
    [Windows.UI.Xaml.Markup.ContentProperty(Name = nameof(DependsOn))]
#elif HAS_WINUI
    [Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(DependsOn))]
#else
    [Xamarin.Forms.ContentProperty(nameof(DependsOn))]
#endif
    public partial class ModuleInfo : IModuleInfo
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="ModuleInfo"/>.
        /// </summary>
        public ModuleInfo()
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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (dependsOn == null)
                throw new ArgumentNullException(nameof(dependsOn));

            ModuleType = Type.GetType(type) ?? throw new ArgumentNullException(nameof(type));
            ModuleName = name;
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
        public ModuleInfo(string name, string type)
            : this(name, type, Array.Empty<string>())
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

        private string _moduleName;
        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public string ModuleName
        {
            get => string.IsNullOrEmpty(_moduleName) ? ModuleType.Name : _moduleName;
            set => _moduleName = value;
        }

        string IModuleInfo.ModuleType
        {
            get => ModuleType.AssemblyQualifiedName;
            set => ModuleType = Type.GetType(value);
        }

        private Type _moduleType;
        /// <summary>
        /// Gets or sets the module <see cref="Type"/>'s AssemblyQualifiedName.
        /// </summary>
        /// <value>The type of the module.</value>
        public Type ModuleType
        {
            get => _moduleType;
            set
            {
                _moduleType = value;
                ModuleName = value.Name;
                foreach (var dependencyAttribute in value.GetCustomAttributes<ModuleDependencyAttribute>())
                {
                    var dependency = dependencyAttribute.ModuleName;
                    if (!DependsOn.Contains(dependency))
                    {
                        DependsOn.Add(dependency);
                    }
                }
            }
        }

        private Collection<string> _dependsOn;
        /// <summary>
        /// Gets or sets the list of modules that this module depends upon.
        /// </summary>
        /// <value>The list of modules that this module depends upon.</value>
        public Collection<string> DependsOn
        {
            get => _dependsOn ?? (_dependsOn = new Collection<string>());
            set => _dependsOn = value;
        }

        /// <summary>
        /// Specifies on which stage the Module will be initialized.
        /// </summary>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Reference to the location of the module assembly. Not Supported by Xamarin.Forms
        /// </summary>
        string IModuleInfo.Ref
        {
            get => throw new NotSupportedException(Resources.ModuleRefLocationNotSupported);
            set { }
        }

        /// <summary>
        /// Gets or sets the state of the <see cref="ModuleInfo"/> with regards to the module loading and initialization process.
        /// </summary>
        public ModuleState State { get; private set; }

        /// <summary>
        /// Gets or sets the state of the <see cref="ModuleInfo"/> with regards to the module loading and initialization process.
        /// </summary>
        ModuleState IModuleInfo.State
        {
            get => State;
            set => State = value;
        }
    }
}
