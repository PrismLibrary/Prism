

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ModuleAttributeFixture
    {
        [TestMethod]
        public void StartupLoadedDefaultsToTrue()
        {
            var moduleAttribute = new ModuleAttribute();

            Assert.AreEqual(false, moduleAttribute.OnDemand);
        }

        [TestMethod]
        public void CanGetAndSetProperties()
        {
            var moduleAttribute = new ModuleAttribute();
            moduleAttribute.ModuleName = "Test";
            moduleAttribute.OnDemand = true;

            Assert.AreEqual("Test", moduleAttribute.ModuleName);
            Assert.AreEqual(true, moduleAttribute.OnDemand);
        }

        [TestMethod]
        public void ModuleDependencyAttributeStoresModuleName()
        {
            var moduleDependencyAttribute = new ModuleDependencyAttribute("Test");

            Assert.AreEqual("Test", moduleDependencyAttribute.ModuleName);
        }
    }
}