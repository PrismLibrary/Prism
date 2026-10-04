using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Prism.Properties;

namespace Prism.Modularity
{
    /// <summary>
    /// Represents a catalog created from a directory on disk.
    /// </summary>
    /// <remarks>
    /// The directory catalog will scan the contents of a directory, locating classes that implement
    /// <see cref="IModule"/> and add them to the catalog based on contents in their associated <see cref="ModuleAttribute"/>.
    /// Assemblies are discovered in a collectible <see cref="AssemblyLoadContext"/> whose unloading is initiated
    /// after discovery. Discovery does not instantiate modules or their attributes.
    ///
    /// The directory catalog does not continue to monitor the directory after it has created the initial catalog.
    /// </remarks>
    public class DirectoryModuleCatalog : ModuleCatalog
    {
        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// Searches the directory for module assemblies in an isolated assembly load context.
        /// </summary>
        protected override void InnerLoad()
        {
            if (string.IsNullOrEmpty(ModulePath))
                throw new InvalidOperationException(Resources.ModulePathCannotBeNullOrEmpty);

            if (!Directory.Exists(ModulePath))
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.DirectoryNotFound, ModulePath));

            try
            {
                var directory = new DirectoryInfo(ModulePath);
                var loadedAssemblies = AssemblyLoadContext.GetLoadContext(typeof(DirectoryModuleCatalog).Assembly)
                    .Assemblies.Concat(AssemblyLoadContext.Default.Assemblies)
                    .Append(typeof(IModule).Assembly)
                    .Where(assembly => !assembly.IsDynamic).Distinct().ToArray();
                var loadContext = new DirectoryModuleCatalogAssemblyLoadContext(directory.FullName, loadedAssemblies);

                try
                {
                    Items.AddRange(GetModuleInfos(directory, loadedAssemblies, loadContext));
                }
                finally
                {
                    loadContext.Unload();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error loading assemblies.", ex);
            }
        }

        private static ModuleInfo[] GetModuleInfos(DirectoryInfo directory, Assembly[] loadedAssemblies,
            AssemblyLoadContext loadContext)
        {
            var loadedFileNames = new HashSet<string>(loadedAssemblies
                .Where(assembly => !string.IsNullOrEmpty(assembly.Location))
                .Select(assembly => Path.GetFileName(assembly.Location)), StringComparer.OrdinalIgnoreCase);
            var assemblies = new List<Assembly>();

            foreach (FileInfo file in directory.GetFiles("*.dll").Where(file => !loadedFileNames.Contains(file.Name)))
            {
                try
                {
                    assemblies.Add(loadContext.LoadFromAssemblyPath(file.FullName));
                }
                catch (BadImageFormatException)
                {
                    // Skip native DLLs and other files that are not managed assemblies.
                }
            }

            // Materialize metadata before unloading so no reflected types escape the discovery context.
            return assemblies.SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => typeof(IModule).IsAssignableFrom(type) && type != typeof(IModule) && !type.IsAbstract)
                .Select(CreateModuleInfo)
                .ToArray();
        }

        private static ModuleInfo CreateModuleInfo(Type type)
        {
            string moduleName = type.Name;
            List<string> dependsOn = new List<string>();
            bool onDemand = false;
            var moduleAttribute =
                CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                    cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);

            if (moduleAttribute != null)
            {
                foreach (CustomAttributeNamedArgument argument in moduleAttribute.NamedArguments)
                {
                    string argumentName = argument.MemberInfo.Name;
                    switch (argumentName)
                    {
                        case "ModuleName":
                            moduleName = (string)argument.TypedValue.Value;
                            break;

                        case "OnDemand":
                            onDemand = (bool)argument.TypedValue.Value;
                            break;

                        case "StartupLoaded":
                            onDemand = !((bool)argument.TypedValue.Value);
                            break;
                    }
                }
            }

            var moduleDependencyAttributes =
                CustomAttributeData.GetCustomAttributes(type).Where(
                    cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName);

            foreach (CustomAttributeData cad in moduleDependencyAttributes)
            {
                dependsOn.Add((string)cad.ConstructorArguments[0].Value);
            }

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, type.AssemblyQualifiedName)
            {
                InitializationMode = onDemand ? InitializationMode.OnDemand : InitializationMode.WhenAvailable,
                Ref = new Uri(type.Assembly.Location).AbsoluteUri,
            };

            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }
    }
}
