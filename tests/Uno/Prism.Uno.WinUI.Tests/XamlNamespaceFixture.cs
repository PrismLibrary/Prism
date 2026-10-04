using System.Reflection;
using System.Windows.Markup;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class XamlNamespaceFixture
{
    private const string GlobalUri = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/global";

    [Theory]
    [InlineData("http://prismlibrary.com")]
    [InlineData("http://prismlibrary.com/")]
    [InlineData(GlobalUri)]
    public void CanonicalAndGlobalSchemasExportCurrentPublicNamespaces(string uri)
    {
        var assembly = typeof(PrismApplicationBase).Assembly;
        var namespaces = assembly.GetCustomAttributes<XmlnsDefinitionAttribute>()
            .Where(attribute => attribute.XmlNamespace == uri)
            .Select(attribute => attribute.ClrNamespace)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(new[] { "Prism", "Prism.Dialogs", "Prism.Interactivity", "Prism.Ioc",
            "Prism.Mvvm", "Prism.Navigation.Regions", "Prism.Navigation.Regions.Behaviors" }, namespaces);
        Assert.All(namespaces, ns => Assert.Contains(assembly.GetExportedTypes(), type => type.Namespace == ns));
    }

    [Fact]
    public void OnlyCanonicalPrismSchemasAndUnoGlobalSchemaAreExported()
    {
        foreach (var assembly in new[] { typeof(PrismApplicationBase).Assembly, typeof(DryIoc.PrismApplication).Assembly })
        {
            Assert.All(assembly.GetCustomAttributes<XmlnsDefinitionAttribute>(), attribute =>
                Assert.Contains(attribute.XmlNamespace, new[] { "http://prismlibrary.com", "http://prismlibrary.com/", GlobalUri }));
        }
    }

    [Theory]
    [InlineData("http://prismlibrary.com")]
    [InlineData("http://prismlibrary.com/")]
    [InlineData(GlobalUri)]
    public void DryIocExportsItsActualPublicNamespace(string uri)
    {
        var attribute = Assert.Single(typeof(DryIoc.PrismApplication).Assembly
            .GetCustomAttributes<XmlnsDefinitionAttribute>().Where(attribute => attribute.XmlNamespace == uri));
        Assert.Equal(typeof(DryIoc.PrismApplication).Namespace, attribute.ClrNamespace);
    }

    [Fact]
    public void ConsumerXamlCompilesAgainstReferencedPrismAssemblies()
    {
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(XamlConsumer.ExplicitNamespaces)));
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(XamlConsumer.ExplicitUsingNamespaces)));
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(XamlConsumer.ImplicitNamespaces)));
        Assert.True(typeof(Microsoft.UI.Xaml.Controls.Page).IsAssignableFrom(typeof(XamlConsumer.CollisionNamespaces)));
    }
}
