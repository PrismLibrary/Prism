

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Prism.Modularity;

namespace Prism.Mef.Modularity
{
    /// <summary>
    /// An attribute that is applied to describe the Managed Extensibility Framework export of an IModule.
    /// </summary>    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Allowing users of the framework to extend the functionality")]
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModuleExportAttribute : ExportAttribute, IModuleExport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleExportAttribute"/> class.
        /// </summary>
        /// <param name="moduleType">The concrete type of the module being exported. Not typeof(IModule).</param>
        public ModuleExportAttribute(Type moduleType)
            : base(typeof(IModule))
        {
            if (moduleType == null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }

            this.ModuleName = moduleType.Name;
            this.ModuleType = moduleType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleExportAttribute"/> class.
        /// </summary>
        /// <param name="moduleName">The contract name of the module.</param>
        /// <param name="moduleType">The concrete type of the module being exported. Not typeof(IModule).</param>
        public ModuleExportAttribute(string moduleName, Type moduleType)
            : base(typeof(IModule))
        {
            this.ModuleName = moduleName;
            this.ModuleType = moduleType;
        }

        #region IModuleExport Members

        /// <summary>
        /// Gets the contract name of the module.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets concrete type of the module being exported. Not typeof(IModule).
        /// </summary>
        public Type ModuleType { get; private set; }

        /// <summary>
        /// Gets or sets when the module should have Initialize() called.
        /// </summary>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Gets or sets the contract names of modules this module depends upon.
        /// </summary>
        [DefaultValue(new string[0])]
        public string[] DependsOnModuleNames { get; set; }

        #endregion
    }
}