using System.Reflection;
using System.Windows.Markup;
using Xunit;

namespace Prism.DryIoc.Uno.WinUI.Tests;

public class XamlNamespaceFixture
{
    [Fact]
    public void CanonicalSchemaExportsCurrentPublicNamespaces()
    {
        var assembly = typeof(PrismApplicationBase).Assembly;
        var namespaces = assembly.GetCustomAttributes<XmlnsDefinitionAttribute>()
            .Where(attribute => attribute.XmlNamespace == "http://prismlibrary.com")
            .Select(attribute => attribute.ClrNamespace)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(new[] { "Prism", "Prism.Dialogs", "Prism.Interactivity", "Prism.Ioc",
            "Prism.Mvvm", "Prism.Navigation.Regions", "Prism.Navigation.Regions.Behaviors" }, namespaces);
        Assert.All(namespaces, ns => Assert.Contains(assembly.GetExportedTypes(), type => type.Namespace == ns));
    }

    [Fact]
    public void PrismAssembliesExportOnlyTheCanonicalSchema()
    {
        foreach (var assembly in new[] { typeof(PrismApplicationBase).Assembly, typeof(global::Prism.DryIoc.PrismApplication).Assembly })
        {
            Assert.All(assembly.GetCustomAttributes<XmlnsDefinitionAttribute>(), attribute =>
                Assert.Equal("http://prismlibrary.com", attribute.XmlNamespace));
        }
    }

    [Fact]
    public void DryIocExportsItsActualPublicNamespace()
    {
        var attribute = Assert.Single(typeof(global::Prism.DryIoc.PrismApplication).Assembly
            .GetCustomAttributes<XmlnsDefinitionAttribute>());
        Assert.Equal(typeof(global::Prism.DryIoc.PrismApplication).Namespace, attribute.ClrNamespace);
    }

    [Fact]
    public void AssembliesDoNotEmitDuplicateNamespaceMappings()
    {
        foreach (var assembly in new[] { typeof(PrismApplicationBase).Assembly, typeof(global::Prism.DryIoc.PrismApplication).Assembly,
            typeof(Xaml.ExplicitNamespaces).Assembly })
        {
            var mappings = assembly.GetCustomAttributes<XmlnsDefinitionAttribute>()
                .Select(attribute => (attribute.XmlNamespace, attribute.ClrNamespace))
                .ToArray();
            Assert.Equal(mappings.Length, mappings.Distinct().Count());
        }
    }

    [Fact]
    public void ConsumerXamlCompilesAgainstReferencedPrismAssemblies()
    {
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(Xaml.ExplicitNamespaces)));
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(Xaml.ExplicitUsingNamespaces)));
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(Xaml.CollisionNamespaces)));
    }

    [Fact]
    public void ConsumerGlobalMappingsFollowTheDisableProperty()
    {
        var assembly = typeof(Xaml.ExplicitNamespaces).Assembly;
        var globalUri = Assert.Single(assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(attribute => attribute.Key == "UnoGlobalXamlNamespaceUri")).Value;
        var namespaces = assembly.GetCustomAttributes<XmlnsDefinitionAttribute>()
            .Where(attribute => attribute.XmlNamespace == globalUri && !attribute.ClrNamespace.StartsWith("Prism.DryIoc.Uno.WinUI.Tests.Xaml."))
            .Select(attribute => attribute.ClrNamespace)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        var implicitPage = assembly.GetType("Prism.DryIoc.Uno.WinUI.Tests.Xaml.ImplicitNamespaces");
        var enabled = Assert.Single(assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(attribute => attribute.Key == "PrismUnoGlobalXmlns")).Value != "false";
        if (!enabled)
        {
            Assert.Empty(namespaces);
            Assert.Null(implicitPage);
        }
        else
        {
            Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(implicitPage));
            Assert.Equal(new[] { "Prism", "Prism.Dialogs", "Prism.DryIoc", "Prism.Interactivity", "Prism.Ioc",
                "Prism.Mvvm", "Prism.Navigation.Regions", "Prism.Navigation.Regions.Behaviors" }, namespaces);
        }
    }
}
