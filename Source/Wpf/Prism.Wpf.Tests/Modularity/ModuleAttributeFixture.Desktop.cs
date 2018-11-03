

using Xunit;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    
    public class ModuleAttributeFixture
    {
        [Fact]
        public void StartupLoadedDefaultsToTrue()
        {
            var moduleAttribute = new ModuleAttribute();

            Assert.Equal(false, moduleAttribute.OnDemand);
        }

        [Fact]
        public void CanGetAndSetProperties()
        {
            var moduleAttribute = new ModuleAttribute();
            moduleAttribute.ModuleName = "Test";
            moduleAttribute.OnDemand = true;

            Assert.Equal("Test", moduleAttribute.ModuleName);
            Assert.Equal(true, moduleAttribute.OnDemand);
        }

        [Fact]
        public void ModuleDependencyAttributeStoresModuleName()
        {
            var moduleDependencyAttribute = new ModuleDependencyAttribute("Test");

            Assert.Equal("Test", moduleDependencyAttribute.ModuleName);
        }
    }
}