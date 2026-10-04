using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prism.Ioc;
using Prism.Modularity;
using Xunit;

namespace Prism.Wpf.Modularity.Tests;

internal static class ModuleAssemblyCompiler
{
    public static string Compile(string directory, string[] sources, params string[] dependencies)
    {
        Directory.CreateDirectory(directory);
        string assemblyName = "TestModules_" + Guid.NewGuid().ToString("N");
        string assemblyPath = Path.Combine(directory, assemblyName + ".dll");
        string[] platformAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            .Split(Path.PathSeparator);
        var references = platformAssemblies
            .Concat(new[]
            {
                typeof(IModule).Assembly.Location,
                typeof(ModuleAttribute).Assembly.Location,
                typeof(IContainerRegistry).Assembly.Location
            })
            .Concat(dependencies)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => MetadataReference.CreateFromFile(path));

        var compilation = CSharpCompilation.Create(assemblyName,
            sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var result = compilation.Emit(assemblyPath);
        Assert.True(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        return assemblyPath;
    }

    public static string ModuleSource(string declaration = "public class TestModule", string attributes = "",
        string members = "", string baseType = "Prism.Modularity.IModule")
    {
        return $$"""
            {{attributes}}
            {{declaration}} : {{baseType}}
            {
                public void RegisterTypes(Prism.Ioc.IContainerRegistry registry) { }
                public void OnInitialized(Prism.Ioc.IContainerProvider provider) { }
                {{members}}
            }
            """;
    }
}
