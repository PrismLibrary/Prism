

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ConfigurationStoreFixture
    {
        [TestMethod]
        public void ShouldRetrieveModuleConfiguration()
        {
            ConfigurationStore store = new ConfigurationStore();
            var section = store.RetrieveModuleConfigurationSection();

            Assert.IsNotNull(section);
            Assert.IsNotNull(section.Modules);
            Assert.AreEqual(1, section.Modules.Count);
            Assert.IsNotNull(section.Modules[0].AssemblyFile);
            Assert.AreEqual("MockModuleA", section.Modules[0].ModuleName);
            Assert.IsNotNull(section.Modules[0].AssemblyFile);
            Assert.IsTrue(section.Modules[0].AssemblyFile.Contains(@"MocksModules\MockModuleA.dll"));
            Assert.IsNotNull(section.Modules[0].ModuleType);
            Assert.IsTrue(section.Modules[0].StartupLoaded);
            Assert.AreEqual("Prism.Wpf.Tests.Mocks.Modules.MockModuleA", section.Modules[0].ModuleType);
        }
    }
}