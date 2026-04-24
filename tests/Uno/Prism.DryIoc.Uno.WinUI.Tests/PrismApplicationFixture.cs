using System.Reflection;
using System.Runtime.Serialization;
using DryIoc;
using Prism.Container.DryIoc;
using Prism.Ioc;
using Xunit;
using static Prism.Container.Uno.Tests.ContainerHelper;
using ExceptionExtensions = System.ExceptionExtensions;

namespace Prism.DryIoc.Uno.WinUI.Tests;

public class PrismApplicationFixture
{
    [Fact]
    public void CreateContainerExtensionReturnsDryIocContainerExtension()
    {
        var app = (PrismApplication)FormatterServices.GetUninitializedObject(typeof(PrismApplicationProxy));
        var method = typeof(PrismApplication).GetMethod("CreateContainerExtension", BindingFlags.NonPublic | BindingFlags.Instance);

        var container = method!.Invoke(app, null);

        Assert.IsType<DryIocContainerExtension>(container!);
    }

    [Fact]
    public void RegisterFrameworkExceptionTypesRegistersContainerException()
    {
        var app = (PrismApplication)FormatterServices.GetUninitializedObject(typeof(PrismApplicationProxy));
        var method = typeof(PrismApplication).GetMethod("RegisterFrameworkExceptionTypes", BindingFlags.NonPublic | BindingFlags.Instance);

        method!.Invoke(app, null);

        Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(RegisteredFrameworkException));
    }

    [Fact]
    public void CreateContainerRulesReturnsDryIocRules()
    {
        var app = (PrismApplication)FormatterServices.GetUninitializedObject(typeof(PrismApplicationProxy));
        var method = typeof(PrismApplication).GetMethod("CreateContainerRules", BindingFlags.NonPublic | BindingFlags.Instance);

        var rules = method!.Invoke(app, null);

        Assert.NotNull(rules);
        Assert.IsType<Rules>(rules);
    }

    [Fact]
    public void PrismApplicationRemainsAbstractAndExtendsPrismApplicationBase()
    {
        Assert.True(typeof(PrismApplication).IsAbstract);
        Assert.Equal(typeof(PrismApplicationBase), typeof(PrismApplication).BaseType);
    }

    private sealed class PrismApplicationProxy : PrismApplication
    {
        protected override UIElement CreateShell() => null!;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
