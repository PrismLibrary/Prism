using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Prism.Modularity;

internal sealed class DirectoryModuleCatalogAssemblyLoadContext : AssemblyLoadContext
{
    private readonly string _modulePath;
    private readonly Assembly[] _sharedAssemblies;

    public DirectoryModuleCatalogAssemblyLoadContext(string modulePath, Assembly[] sharedAssemblies)
        : base(isCollectible: true)
    {
        _modulePath = modulePath;
        _sharedAssemblies = sharedAssemblies;
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        // Share the host's assemblies so IModule and its dependencies retain their type identity,
        // even when copies of Prism assemblies have been deployed beside the modules.
        Assembly sharedAssembly = _sharedAssemblies.FirstOrDefault(assembly =>
            AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), assemblyName));
        if (sharedAssembly != null)
            return sharedAssembly;

        string assemblyPath = Path.Combine(_modulePath, assemblyName.Name + ".dll");
        return File.Exists(assemblyPath) ? LoadFromAssemblyPath(assemblyPath) : null;
    }
}
