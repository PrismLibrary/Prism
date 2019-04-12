using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prism.Modularity
{
    /// <summary>
    /// Represets a catalog created from a directory on disk.
    /// </summary>
    /// <remarks>
    /// The directory catalog will scan the contents of a directory, locating classes that implement
    /// <see cref="IModule"/> and add them to the catalog based on contents in their associated <see cref="ModuleAttribute"/>.
    /// Assemblies are loaded into a new application domain with ReflectionOnlyLoad.  The application domain is destroyed
    /// once the assemblies have been discovered.
    ///
    /// The diretory catalog does not continue to monitor the directory after it has created the initialze catalog.
    /// </remarks>
    public class DirectoryModuleCatalog : ModuleCatalog
    {
        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// Drives the main logic of building the child domain and searching for the assemblies.
        /// </summary>
        protected override void InnerLoad()
        {
            if (string.IsNullOrEmpty(ModulePath))
                throw new InvalidOperationException("ModulePathCannotBeNullOrEmpty");

            if (!Directory.Exists(ModulePath))
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, "DirectoryNotFound", this.ModulePath));

            this.Items.AddRange(GetModuleInfos(ModulePath));
        }

        internal IEnumerable<IModuleCatalogItem> GetModuleInfos(string modulePath)
        {
            var modules = new List<IModuleCatalogItem>();
            var dlls = Directory.GetFiles(modulePath, "*.dll");
            var typeOfIModule = typeof(IModule);

            var mscorlib = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("mscorlib,")).FirstOrDefault();

            using (TypeLoader tl = new TypeLoader())
            {
                List<Assembly> modulesAssemblies = new List<Assembly>();

                tl.CoreAssemblyName = mscorlib.FullName;
                tl.Resolving += Tl_Resolving;

                foreach (var dll in dlls)
                {
                    modulesAssemblies.Add(tl.LoadFromAssemblyPath(dll));
                }

                foreach (var assembly in modulesAssemblies)
                {
                    modules.AddRange(assembly.GetExportedTypes()
                        .Where(t => t.GetInterfaces().Any(x => x.FullName.Equals(typeOfIModule.FullName)))
                        .Where(t => !t.IsAbstract)
                        .Select(type => CreateModuleInfo(type)));
                }
            }

            return modules;
        }

        private Assembly Tl_Resolving(TypeLoader sender, AssemblyName assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Equals(assemblyName.FullName)).FirstOrDefault();

            if (assembly == null)
            {
                return null;  // Don't throw an exception if the assembly doesn't exist. Return null.
            }

            return sender.LoadFromAssemblyPath(assembly.Location);
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
                InitializationMode =
                                                onDemand
                                                    ? InitializationMode.OnDemand
                                                    : InitializationMode.WhenAvailable,
                Ref = new Uri(type.Assembly.Location, UriKind.RelativeOrAbsolute).AbsoluteUri
            };
            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }
    }
}