

using Prism.Modularity;
using Xunit;

namespace Prism.Wpf.Tests.Modularity
{

    public class ModuleAttributeFixture
    {
        [Fact]
        public void StartupLoadedDefaultsToTrue()
        {
            var moduleAttribute = new ModuleAttribute();

            Assert.False(moduleAttribute.OnDemand);
        }

        [Fact]
        public void CanGetAndSetProperties()
        {
            var moduleAttribute = new ModuleAttribute();
            moduleAttribute.ModuleName = "Test";
            moduleAttribute.OnDemand = true;

            Assert.Equal("Test", moduleAttribute.ModuleName);
            Assert.True(moduleAttribute.OnDemand);
        }

        [Fact]
        public void ModuleDependencyAttributeStoresModuleName()
        {
            var moduleDependencyAttribute = new ModuleDependencyAttribute("Test");

            Assert.Equal("Test", moduleDependencyAttribute.ModuleName);
        }
    }
}
