using System.Runtime.Serialization;
using Prism.Ioc;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class PrismApplicationBaseFixture
{
    [Fact]
    public void HostThrowsUntilShellFinalizationRuns()
    {
        var app = (PrismApplicationBase)FormatterServices.GetUninitializedObject(typeof(TestPrismApplication));

        var ex = Assert.Throws<InvalidOperationException>(() => _ = app.Host);
        Assert.Contains("host has not yet been created", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PrismApplicationBaseExposesExpectedExtensibilitySurface()
    {
        Assert.True(typeof(PrismApplicationBase).IsAbstract);
        Assert.True(typeof(PrismApplicationBase).GetMethod("OnLaunched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.IsFinal);
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("CreateShell", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("RegisterTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("ConfigureApp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("ConfigureHost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("ConfigureServices", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("ConfigureWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("InitializeModules", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(PrismApplicationBase).GetMethod("ConfigureModuleCatalog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
    }

    [Fact]
    public void ContainerPropertyReturnsContainerProvider()
    {
        var app = (PrismApplicationBase)FormatterServices.GetUninitializedObject(typeof(TestPrismApplication));
        Assert.Null(app.Container);
    }

    private sealed class TestPrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension() => throw new NotImplementedException();

        protected override UIElement CreateShell() => null!;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
