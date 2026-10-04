using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Prism.Ioc;
using Prism.Modularity;
using Xunit;

namespace Prism.Wpf.Modularity.Tests;

public class DirectoryModuleCatalogFixture : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "Prism.ModuleCatalog.Tests", Guid.NewGuid().ToString("N"));

    public DirectoryModuleCatalogFixture()
    {
        Directory.CreateDirectory(_directory);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyPathThrows(string path)
    {
        var catalog = new DirectoryModuleCatalog { ModulePath = path };

        Assert.Throws<InvalidOperationException>(() => catalog.Load());
    }

    [Fact]
    public void NonexistentPathThrows()
    {
        var catalog = new DirectoryModuleCatalog { ModulePath = Path.Combine(_directory, "missing") };

        Assert.Throws<InvalidOperationException>(() => catalog.Load());
    }

    [Fact]
    public void EmptyDirectoryHasNoModules()
    {
        Assert.Empty(LoadCatalog(_directory).Modules);
    }

    [Fact]
    public void DiscoversModuleWithoutRunningItsConstructors()
    {
        string path = CompileModule(members: """
            static TestModule() { throw new System.InvalidOperationException("Static constructor ran"); }
            public TestModule() { throw new System.InvalidOperationException("Constructor ran"); }
            """);

        var module = Assert.Single(LoadCatalog(_directory).Modules);

        Assert.Equal("TestModule", module.ModuleName);
        Assert.Equal(InitializationMode.WhenAvailable, module.InitializationMode);
        Assert.Empty(module.DependsOn);
        Assert.StartsWith("TestModule, " + Path.GetFileNameWithoutExtension(path), module.ModuleType);
        Assert.Equal(path, new Uri(module.Ref).LocalPath);
    }

    [Fact]
    public void PreservesNameInitializationModeAndDependencies()
    {
        CompileModule(attributes: """
            [Prism.Modularity.Module(ModuleName = "NamedModule", OnDemand = true)]
            [Prism.Modularity.ModuleDependency("First")]
            [Prism.Modularity.ModuleDependency("Second")]
            """);

        var module = Assert.Single(LoadCatalog(_directory).Modules);

        Assert.Equal("NamedModule", module.ModuleName);
        Assert.Equal(InitializationMode.OnDemand, module.InitializationMode);
        Assert.Equal(new[] { "First", "Second" }, module.DependsOn);
    }

    [Fact]
    public void IgnoresAbstractNonPublicAndUnrelatedTypes()
    {
        ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            ModuleAssemblyCompiler.ModuleSource("public abstract class AbstractModule"),
            ModuleAssemblyCompiler.ModuleSource("internal class InternalModule"),
            "public class UnrelatedType { }",
            ModuleAssemblyCompiler.ModuleSource()
        });

        Assert.Equal("TestModule", Assert.Single(LoadCatalog(_directory).Modules).ModuleName);
    }

    [Fact]
    public void ResolvesDependenciesBesideModules()
    {
        string dependency = ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            ModuleAssemblyCompiler.ModuleSource("public abstract class ModuleBase")
        });
        ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            "public class DerivedModule : ModuleBase { }"
        }, dependency);

        Assert.Equal("DerivedModule", Assert.Single(LoadCatalog(_directory).Modules).ModuleName);
        Assert.DoesNotContain(AssemblyLoadContext.Default.Assemblies,
            assembly => assembly.GetName().Name == Path.GetFileNameWithoutExtension(dependency));
    }

    [Fact]
    public void ResolvesHostDependenciesLoadedWithoutFileLocations()
    {
        string hostDirectory = Path.Combine(_directory, "host");
        string dependency = ModuleAssemblyCompiler.Compile(hostDirectory, new[]
        {
            ModuleAssemblyCompiler.ModuleSource("public abstract class HostModuleBase")
        });
        using (var stream = File.OpenRead(dependency))
            AssemblyLoadContext.Default.LoadFromStream(stream);
        ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            "public class DerivedModule : HostModuleBase { }"
        }, dependency);

        Assert.Equal("DerivedModule", Assert.Single(LoadCatalog(_directory).Modules).ModuleName);
    }

    [Fact]
    public void SharesPrismContractsWhenCopiesAreInModuleDirectory()
    {
        CompileModule();
        foreach (Assembly assembly in new[] { typeof(IModule).Assembly, typeof(ModuleAttribute).Assembly, typeof(IContainerRegistry).Assembly })
            File.Copy(assembly.Location, Path.Combine(_directory, Path.GetFileName(assembly.Location)));

        Assert.Equal("TestModule", Assert.Single(LoadCatalog(_directory).Modules).ModuleName);
    }

    [Fact]
    public void IgnoresInvalidDllsWhileDiscoveringManagedModules()
    {
        CompileModule();
        File.WriteAllText(Path.Combine(_directory, "native.dll"), "This is not a managed assembly.");

        Assert.Single(LoadCatalog(_directory).Modules);
    }

    [Fact]
    public void EscapesSpecialCharactersInFileReference()
    {
        string directory = Path.Combine(_directory, "modules # 100% café");
        string path = ModuleAssemblyCompiler.Compile(directory, new[] { ModuleAssemblyCompiler.ModuleSource() });

        var module = Assert.Single(LoadCatalog(directory).Modules);
        var uri = new Uri(module.Ref);

        Assert.True(uri.IsFile);
        Assert.Equal(path, uri.LocalPath);
        Assert.Contains("%23", module.Ref);
        Assert.Contains("%25", module.Ref);
    }

    [Fact]
    public void SupportsRelativeDirectoryPaths()
    {
        CompileModule();
        string relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), _directory);

        Assert.Single(LoadCatalog(relativePath).Modules);
    }

    [Fact]
    public void IgnoresDynamicAndLocationlessHostAssemblies()
    {
        AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Dynamic_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndCollect);
        string hostDirectory = Path.Combine(_directory, "host");
        string hostAssembly = ModuleAssemblyCompiler.Compile(hostDirectory, new[] { "public class HostType { }" });
        using (var stream = File.OpenRead(hostAssembly))
            AssemblyLoadContext.Default.LoadFromStream(stream);
        CompileModule();

        Assert.Single(LoadCatalog(_directory).Modules);
    }

    [Fact]
    public void RepeatedDiscoveryDoesNotLoadModulesIntoDefaultContext()
    {
        string path = CompileModule();
        string assemblyName = Path.GetFileNameWithoutExtension(path);

        Assert.Single(LoadCatalog(_directory).Modules);
        Assert.Single(LoadCatalog(_directory).Modules);
        Assert.DoesNotContain(AssemblyLoadContext.Default.Assemblies, assembly => assembly.GetName().Name == assemblyName);
    }

    [Fact]
    public void DiscoveryContextCanUnloadWhileCatalogRemainsAlive()
    {
        string path = CompileModule();
        var result = ObserveDiscovery(_directory, Path.GetFileNameWithoutExtension(path));

        Assert.True(result.UnloadingRequested);
        AssertCollected(result.Context);
        Assert.Single(result.Catalog.Modules);
        GC.KeepAlive(result.Catalog);
    }

    [Fact]
    public void DiscoveryFailureUnloadsContextAndDoesNotAddPartialResults()
    {
        string dependencyDirectory = Path.Combine(_directory, "dependencies");
        string dependency = ModuleAssemblyCompiler.Compile(dependencyDirectory, new[]
        {
            ModuleAssemblyCompiler.ModuleSource("public abstract class MissingBase")
        });
        string path = ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            "public class DependentModule : MissingBase { }",
            ModuleAssemblyCompiler.ModuleSource()
        }, dependency);

        var result = ObserveDiscovery(_directory, Path.GetFileNameWithoutExtension(path), expectFailure: true);

        Assert.Empty(result.Catalog.Modules);
        Assert.True(result.UnloadingRequested);
        AssertCollected(result.Context);
        GC.KeepAlive(result.Catalog);
    }

    [Fact]
    public void DiscoveryDoesNotInstantiateUnrelatedAttributes()
    {
        ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            """
            public class ThrowingAttribute : System.Attribute
            {
                public ThrowingAttribute() { throw new System.InvalidOperationException("Attribute constructor ran"); }
            }
            """,
            ModuleAssemblyCompiler.ModuleSource(attributes: "[Throwing]")
        });

        Assert.Single(LoadCatalog(_directory).Modules);
    }

    private string CompileModule(string attributes = "", string members = "")
    {
        return ModuleAssemblyCompiler.Compile(_directory, new[]
        {
            ModuleAssemblyCompiler.ModuleSource(attributes: attributes, members: members)
        });
    }

    private static DirectoryModuleCatalog LoadCatalog(string directory)
    {
        var catalog = new DirectoryModuleCatalog { ModulePath = directory };
        catalog.Load();
        return catalog;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (DirectoryModuleCatalog Catalog, WeakReference Context, bool UnloadingRequested) ObserveDiscovery(
        string directory, string assemblyName, bool expectFailure = false)
    {
        WeakReference contextReference = null;
        bool unloadingRequested = false;
        AssemblyLoadEventHandler handler = (_, args) =>
        {
            if (args.LoadedAssembly.GetName().Name == assemblyName)
            {
                var context = AssemblyLoadContext.GetLoadContext(args.LoadedAssembly);
                Assert.True(context.IsCollectible);
                contextReference = new WeakReference(context);
                context.Unloading += _ => unloadingRequested = true;
            }
        };
        var catalog = new DirectoryModuleCatalog { ModulePath = directory };
        AppDomain.CurrentDomain.AssemblyLoad += handler;
        try
        {
            if (expectFailure)
            {
                var exception = Assert.Throws<Exception>(() => catalog.Load());
                Assert.Equal("There was an error loading assemblies.", exception.Message);
                Assert.True(exception.InnerException is ReflectionTypeLoadException or FileNotFoundException);
            }
            else
                catalog.Load();
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyLoad -= handler;
        }

        Assert.NotNull(contextReference);
        return (catalog, contextReference, unloadingRequested);
    }

    private static void AssertCollected(WeakReference reference)
    {
        for (int attempt = 0; reference.IsAlive && attempt < 10; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        Assert.False(reference.IsAlive);
    }

    public void Dispose()
    {
        // Unloading is cooperative; release file handles before deleting the test assemblies on Windows.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Directory.Delete(_directory, recursive: true);
    }
}
