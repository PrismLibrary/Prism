

using System;
using System.Configuration;
using System.Linq;
using Xunit;
using Prism.Modularity;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Modularity
{
    public class ConfigurationModuleCatalogFixture
    {
        [Fact]
        public void CanInitConfigModuleEnumerator()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog
            {
                Store = store
            };
            Assert.NotNull(catalog);
        }

        [Fact]
        public void NullConfigurationStoreThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = null };
                catalog.Load();
            });

        }

        [Fact]
        public void ShouldReturnAListOfModuleInfo()
        {
            MockConfigurationStore store = new MockConfigurationStore
            {
                Modules = new[] { new ModuleConfigurationElement(@"MocksModules\MockModuleA.dll", "TestModules.MockModuleAClass", "MockModuleA", false) }
            };

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog(){Store = store};
            catalog.Load();

            var modules = catalog.Modules;

            Assert.NotNull(modules);
            Assert.Single(modules);
            Assert.NotEqual(InitializationMode.WhenAvailable, modules.First().InitializationMode);
            Assert.NotNull(modules.First().Ref);
            Assert.StartsWith("file://", modules.First().Ref);
            Assert.Contains(@"MocksModules/MockModuleA.dll", modules.First().Ref);
            Assert.NotNull( modules.First().ModuleType);
            Assert.Equal("TestModules.MockModuleAClass",  modules.First().ModuleType);

        }

        [Fact]
        public void GetZeroModules()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            Assert.Empty(catalog.Modules);
        }

        [Fact]
        public void EnumeratesThreeModulesWithDependencies()
        {
            var store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false)
            {
                Dependencies = new ModuleDependencyCollection(
                new[] { new ModuleDependencyConfigurationElement("Module2") })
            };

            var module2 = new ModuleConfigurationElement("Module2.dll", "Test.Module2", "Module2", false)
            {
                Dependencies = new ModuleDependencyCollection(
                new[] { new ModuleDependencyConfigurationElement("Module3") })
            };

            var module3 = new ModuleConfigurationElement("Module3.dll", "Test.Module3", "Module3", false);
            store.Modules = new[] { module3, module2, module1 };

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            var modules = catalog.Modules;

            Assert.Equal(3, modules.Count());
            Assert.Contains(modules, module => module.ModuleName == "Module1");
            Assert.Contains(modules, module => module.ModuleName == "Module2");
            Assert.Contains(modules, module => module.ModuleName == "Module3");
        }

        [Fact]
        public void EnumerateThrowsIfDuplicateNames()
        {
            var ex = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                MockConfigurationStore store = new MockConfigurationStore();
                var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
                var module2 = new ModuleConfigurationElement("Module2.dll", "Test.Module2", "Module1", false);
                store.Modules = new[] { module2, module1 };
                ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
                catalog.Load();
            });

        }

        [Fact]
        public void EnumerateNotThrowsIfDuplicateAssemblyFile()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
            var module2 = new ModuleConfigurationElement("Module1.dll", "Test.Module2", "Module2", false);
            store.Modules = new[] { module2, module1 };
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            Assert.Equal(2, catalog.Modules.Count());
        }

        [Fact]
        public void GetStartupLoadedModulesDoesntRetrieveOnDemandLoaded()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
            store.Modules = new[] { module1 };

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            Assert.Single(catalog.Modules);
            Assert.Equal<int>(0, catalog.Modules.Count(m => m.InitializationMode != InitializationMode.OnDemand));
        }

        [Fact]
        public void GetModulesNotThrownIfModuleSectionIsNotDeclared()
        {
            MockNullConfigurationStore store = new MockNullConfigurationStore();

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            var modules = catalog.Modules;

            Assert.NotNull(modules);
            Assert.Empty(modules);
        }

        internal class MockNullConfigurationStore : IConfigurationStore
        {
            public ModulesConfigurationSection RetrieveModuleConfigurationSection()
            {
                return null;
            }
        }
    }
}
