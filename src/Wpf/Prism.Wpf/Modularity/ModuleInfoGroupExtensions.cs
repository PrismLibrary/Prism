

using System;
using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    /// <summary>
    /// Defines extension methods for the <see cref="ModuleInfoGroup"/> class.
    /// </summary>
    public static class ModuleInfoGroupExtensions
    {
        /// <summary>
        /// Adds a new module that is statically referenced to the specified module info group.
        /// </summary>
        /// <param name="moduleInfoGroup">The group where to add the module info in.</param>
        /// <param name="moduleName">The name for the module.</param>
        /// <param name="moduleType">The type for the module. This type should be a descendant of <see cref="IModule"/>.</param>
        /// <param name="dependsOn">The names for the modules that this module depends on.</param>
        /// <returns>Returns the instance of the passed in module info group, to provide a fluid interface.</returns>
        public static ModuleInfoGroup AddModule(
                    this ModuleInfoGroup moduleInfoGroup,
                    string moduleName,
                    Type moduleType,
                    params string[] dependsOn)
        {
            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));

            if (moduleInfoGroup == null)
                throw new ArgumentNullException(nameof(moduleInfoGroup));

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType.AssemblyQualifiedName);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfoGroup.Add(moduleInfo);
            return moduleInfoGroup;
        }

        /// <summary>
        /// Adds a new module that is statically referenced to the specified module info group.
        /// </summary>
        /// <param name="moduleInfoGroup">The group where to add the module info in.</param>
        /// <param name="moduleType">The type for the module. This type should be a descendant of <see cref="IModule"/>.</param>
        /// <param name="dependsOn">The names for the modules that this module depends on.</param>
        /// <returns>Returns the instance of the passed in module info group, to provide a fluid interface.</returns>
        /// <remarks>The name of the module will be the type name.</remarks>
        public static ModuleInfoGroup AddModule(
                    this ModuleInfoGroup moduleInfoGroup,
                    Type moduleType,
                    params string[] dependsOn)
        {
            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));

            return AddModule(moduleInfoGroup, moduleType.Name, moduleType, dependsOn);
        }
    }
}
