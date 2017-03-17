

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ConfigurationModuleCatalogFixture
    {
        [TestMethod]
        public void CanInitConfigModuleEnumerator()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog();
            catalog.Store = store;
            Assert.IsNotNull(catalog);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NullConfigurationStoreThrows()
        {
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog(){Store = null};
            catalog.Load();
        }

        [TestMethod]
        public void ShouldReturnAListOfModuleInfo()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            store.Modules = new[] { new ModuleConfigurationElement(@"MocksModules\MockModuleA.dll", "TestModules.MockModuleAClass", "MockModuleA", false) };

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog(){Store = store};
            catalog.Load();

            IEnumerable<ModuleInfo> modules = catalog.Modules;

            Assert.IsNotNull(modules);
            Assert.AreEqual(1, modules.Count());
            Assert.AreNotEqual(InitializationMode.WhenAvailable, modules.First().InitializationMode);
            Assert.IsNotNull(modules.First().Ref);
            StringAssert.StartsWith(modules.First().Ref, "file://");
            Assert.IsTrue( modules.First().Ref.Contains(@"MocksModules/MockModuleA.dll"));
            Assert.IsNotNull( modules.First().ModuleType);
            Assert.AreEqual("TestModules.MockModuleAClass",  modules.First().ModuleType);

        }

        [TestMethod]
        public void GetZeroModules()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            Assert.AreEqual(0, catalog.Modules.Count());
        }

        [TestMethod]
        public void EnumeratesThreeModulesWithDependencies()
        {
            var store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
            module1.Dependencies = new ModuleDependencyCollection(
                new[] { new ModuleDependencyConfigurationElement("Module2") });

            var module2 = new ModuleConfigurationElement("Module2.dll", "Test.Module2", "Module2", false);
            module2.Dependencies = new ModuleDependencyCollection(
                new[] { new ModuleDependencyConfigurationElement("Module3") });

            var module3 = new ModuleConfigurationElement("Module3.dll", "Test.Module3", "Module3", false);
            store.Modules = new[] { module3, module2, module1 };

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            var modules = catalog.Modules;

            Assert.AreEqual(3, modules.Count());
            Assert.IsTrue(modules.Any(module => module.ModuleName == "Module1"));
            Assert.IsTrue(modules.Any(module => module.ModuleName == "Module2"));
            Assert.IsTrue(modules.Any(module => module.ModuleName == "Module3"));
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void EnumerateThrowsIfDuplicateNames()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
            var module2 = new ModuleConfigurationElement("Module2.dll", "Test.Module2", "Module1", false);
            store.Modules = new[] { module2, module1 };
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();
        }

        [TestMethod]
        public void EnumerateNotThrowsIfDuplicateAssemblyFile()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
            var module2 = new ModuleConfigurationElement("Module1.dll", "Test.Module2", "Module2", false);
            store.Modules = new[] { module2, module1 };
            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            Assert.AreEqual(2, catalog.Modules.Count());
        }

        [TestMethod]
        public void GetStartupLoadedModulesDoesntRetrieveOnDemandLoaded()
        {
            MockConfigurationStore store = new MockConfigurationStore();
            var module1 = new ModuleConfigurationElement("Module1.dll", "Test.Module1", "Module1", false);
            store.Modules = new[] { module1 };

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            Assert.AreEqual<int>(1, catalog.Modules.Count());
            Assert.AreEqual<int>(0, catalog.Modules.Count(m => m.InitializationMode != InitializationMode.OnDemand));
        }

        [TestMethod]
        public void GetModulesNotThrownIfModuleSectionIsNotDeclared()
        {
            MockNullConfigurationStore store = new MockNullConfigurationStore();

            ConfigurationModuleCatalog catalog = new ConfigurationModuleCatalog() { Store = store };
            catalog.Load();

            var modules = catalog.Modules;

            Assert.IsNotNull(modules);
            Assert.AreEqual(0, modules.Count());
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
