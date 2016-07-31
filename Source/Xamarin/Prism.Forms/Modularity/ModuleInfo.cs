using System;

namespace Prism.Modularity
{
    public class ModuleInfo
    {
        /// <summary>
        /// Specifies on which stage the Module will be initialized.
        /// </summary>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the state of the <see cref="ModuleInfo"/> with regards to the module loading and initialization process.
        /// </summary>
        public ModuleState State { get; internal set; }

        /// <summary>
        /// Gets or sets the module <see cref="Type"/>.
        /// </summary>
        /// <value>The type of the module.</value>
        public Type ModuleType { get; set; }

        /// <summary>
        /// Initializes a new empty instance of <see cref="ModuleInfo"/>.
        /// </summary>
        public ModuleInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleType">The module's type.</param>
        public ModuleInfo(Type moduleType)
        {
            ModuleType = moduleType;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleName">The module's name.</param>
        /// <param name="moduleType">The module's type.</param>
        public ModuleInfo(string moduleName, Type moduleType) :
            this(moduleType)            
        {
            ModuleName = moduleName;            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleName">The module's name.</param>
        /// <param name="moduleType">The module's type.</param>
        /// <param name="initializationMode">The module's <see cref="InitializationMode"/>.</param>
        public ModuleInfo(string moduleName, Type moduleType, InitializationMode initializationMode) : this (moduleName, moduleType)
        {
            InitializationMode = initializationMode;
        }
    }
}
