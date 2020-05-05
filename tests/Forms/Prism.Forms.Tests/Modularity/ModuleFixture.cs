using System;
using System.Linq;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Modules;
using Prism.Modularity;
using Xunit;

namespace Prism.Forms.Tests.Modularity
{
    public class ModuleFixture
    {
        public ModuleFixture()
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        [Fact]
        public void BasicCatalogHasThreeModules()
        {
            var catalog = new BasicCatalog();

            Assert.Equal(3, catalog.Modules.Count());
        }

        [Fact]
        public void ModuleAUsesCustomModuleName()
        {
            var catalog = new BasicCatalog();

            Assert.Single(catalog.Modules, mi => mi.ModuleName == "ModuleATest");

            var mi = catalog.Modules.First(x => x.ModuleName == "ModuleATest");

            Assert.Equal(typeof(ModuleA), Type.GetType(mi.ModuleType));
        }

        [Fact]
        public void ModuleBUsesTypeName()
        {
            var catalog = new BasicCatalog();

            Assert.Contains(catalog.Modules, mi => mi.ModuleName == typeof(ModuleB).Name);
        }

        [Fact]
        public void ModuleCUsesTypeNameNotExplicitEmptyString()
        {
            var catalog = new BasicCatalog();

            Assert.Empty(catalog.Modules.Where(mi => string.IsNullOrEmpty(mi.ModuleName)));
            Assert.Single(catalog.Modules, mi => mi.ModuleName == typeof(ModuleC).Name);
        }

        [Theory]
        [InlineData("ModuleATest", InitializationMode.WhenAvailable)]
        [InlineData("ModuleB", InitializationMode.WhenAvailable)]
        [InlineData("ModuleC", InitializationMode.OnDemand)]
        public void ModuleHasSpecifiedInitializationMode(string moduleName, InitializationMode mode)
        {
            var catalog = new BasicCatalog();
            Assert.Single(catalog.Modules, mi => mi.ModuleName == moduleName && mi.InitializationMode == mode);
        }

        [Fact]
        public void DependenciesAreSpecifiedInXaml()
        {
            var catalog = new BasicDependencyCatalog();

            var mi = catalog.Modules.First(x => x.ModuleName == nameof(ModuleA));

            Assert.Contains(mi.DependsOn, d => d == nameof(ModuleB));
        }

        [Fact]
        public void DependsOnAttributeAddsDependencies()
        {
            var catalog = new BasicDependencyCatalog();

            var mi = catalog.Modules.First(x => x.ModuleName == nameof(MasterModule));

            Assert.Contains(mi.DependsOn, d => d == nameof(DependentModuleA));
            Assert.Contains(mi.DependsOn, d => d == nameof(DependentModuleB));
        }

        [Fact]
        public void BadModuleThrowsException()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule<BadModule>();
            var containerMock = ContainerMock.CreateMock();
            var initializer = new ModuleInitializer(containerMock.Object);
            var manager = new ModuleManager(initializer, catalog);
            manager.LoadModuleCompleted += OnModuleLoaded;
            void OnModuleLoaded(object sender, LoadModuleCompletedEventArgs args)
            {
                Assert.Equal(nameof(BadModule), args.ModuleInfo.ModuleName);
                Assert.NotNull(args.Error);
                Assert.Equal(nameof(BadModule.RegisterTypes), args.Error.Message);
                manager.LoadModuleCompleted -= OnModuleLoaded;
            }
            var ex = Record.Exception(() => manager.LoadModule(nameof(BadModule)));
        }

        [Fact]
        public void BadInitializationModuleThrowsException()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule<BadInitializationModule>();
            var containerMock = ContainerMock.CreateMock();
            var initializer = new ModuleInitializer(containerMock.Object);
            var manager = new ModuleManager(initializer, catalog);
            manager.LoadModuleCompleted += OnModuleLoaded;
            void OnModuleLoaded(object sender, LoadModuleCompletedEventArgs args)
            {
                Assert.Equal(nameof(BadInitializationModule), args.ModuleInfo.ModuleName);
                Assert.NotNull(args.Error);
                Assert.Equal(nameof(BadInitializationModule.OnInitialized), args.Error.Message);
                manager.LoadModuleCompleted -= OnModuleLoaded;
            }
            var ex = Record.Exception(() => manager.LoadModule(nameof(BadInitializationModule)));
        }
    }
}
