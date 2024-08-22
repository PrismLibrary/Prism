namespace Prism.DryIoc.Maui.Tests.Fixtures.Modularity;

public class ModuleCatalogTests(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
{
    public readonly IModuleInfo ModuleA = new ModuleInfo(typeof(MockModuleA));
    public readonly IModuleInfo ModuleB = new ModuleInfo(typeof(MockModuleB));

    [Fact]
    public void UsesCustomModuleCatalog()
    {
        var builder = CreateBuilder(prism =>
        {
            prism.RegisterTypes(c => c.RegisterSingleton<IModuleCatalog, CustomMoudleCatalog>())
                 .ConfigureModuleCatalog(catalog => { });
        });
        var app = builder.Build();

        var moduleCatalog = app.Services.GetService<IModuleCatalog>();
        Assert.NotNull(moduleCatalog);
        Assert.IsType<CustomMoudleCatalog>(moduleCatalog);
    }

    [Fact]
    public void InjectsModulesFromDI()
    {
        var builder = CreateBuilder(prism =>
        {
            prism.RegisterTypes(c => c.RegisterInstance<IModuleInfo>(ModuleA))
                 .ConfigureModuleCatalog(c => { });
        });
        var app = builder.Build();

        var moduleCatalog = app.Services.GetService<IModuleCatalog>();
        Assert.Single(moduleCatalog.Modules);
        Assert.Equal(ModuleA.ModuleType, moduleCatalog.Modules.Single().ModuleType);
    }

    [Fact]
    public void InjectsModulesFromConfigureDelegate()
    {
        var builder = CreateBuilder(prism =>
            prism.ConfigureModuleCatalog(c => c.AddModule<MockModuleA>()));
        var app = builder.Build();

        var moduleCatalog = app.Services.GetService<IModuleCatalog>();
        Assert.Single(moduleCatalog.Modules);
        Assert.Equal(ModuleA.ModuleType, moduleCatalog.Modules.Single().ModuleType);
    }

    [Fact]
    public void CombinesModulesFromConfigureMethodAndDI()
    {
        var builder = CreateBuilder(prism =>
        {
            prism.RegisterTypes(c => c.RegisterInstance<IModuleInfo>(ModuleA))
                 .ConfigureModuleCatalog(c => c.AddModule<MockModuleB>());
        });
        var app = builder.Build();

        var moduleCatalog = app.Services.GetService<IModuleCatalog>();
        Assert.Equal(2, moduleCatalog.Modules.Count());
        Assert.Contains(ModuleA.ModuleType, moduleCatalog.Modules.Select(m => m.ModuleType));
        Assert.Contains(ModuleB.ModuleType, moduleCatalog.Modules.Select(m => m.ModuleType));
    }

    public class CustomMoudleCatalog : ModuleCatalogBase { }

    public class MockModuleA : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }

    public class MockModuleB : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
