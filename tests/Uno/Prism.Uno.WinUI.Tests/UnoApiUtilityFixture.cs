using Prism.Mvvm;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class UnoApiUtilityFixture
{
    [Fact]
    public void ViewModelLocatorExposesAttachedPropertyContract()
    {
        Assert.NotNull(typeof(ViewModelLocator).GetField("AutowireViewModelProperty"));
        Assert.NotNull(typeof(ViewModelLocator).GetMethod(nameof(ViewModelLocator.GetAutowireViewModel)));
        Assert.NotNull(typeof(ViewModelLocator).GetMethod(nameof(ViewModelLocator.SetAutowireViewModel)));
    }

    [Fact]
    public void UnoInternalUtilityTypesExposeExpectedMethods()
    {
        var prismAssembly = typeof(PrismApplicationBase).Assembly;

        var bindingOperations = prismAssembly.GetType("Prism.BindingOperations");
        var designerProperties = prismAssembly.GetType("Prism.DesignerProperties");
        var dependencyObjectExtensions = prismAssembly.GetType("Prism.DependencyObjectExtensions");

        Assert.NotNull(bindingOperations);
        Assert.NotNull(designerProperties);
        Assert.NotNull(dependencyObjectExtensions);

        Assert.NotNull(bindingOperations!.GetMethod("GetBinding", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));
        Assert.NotNull(designerProperties!.GetMethod("GetIsInDesignMode", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));
        Assert.NotNull(dependencyObjectExtensions!.GetMethod("CheckAccess", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));
    }
}
