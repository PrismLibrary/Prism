

using System;
using System.Linq;
using Xunit;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    /// <summary>
    /// Summary description for ModuleInfoGroupExtensionsFixture
    /// </summary>
    
    public class ModuleInfoGroupExtensionsFixture
    {
        [Fact]
        public void ShouldAddModuleToModuleInfoGroup()
        {
            string moduleName = "MockModule";
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule(moduleName, typeof(MockModule));

            Assert.Single(groupInfo);
            Assert.Equal(moduleName, groupInfo.ElementAt(0).ModuleName);
        }

        [Fact]
        public void ShouldSetModuleTypeCorrectly()
        {
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule("MockModule", typeof(MockModule));

            Assert.Single(groupInfo);
            Assert.Equal(typeof(MockModule).AssemblyQualifiedName, groupInfo.ElementAt(0).ModuleType);
        }

        [Fact]
        public void NullTypeThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                ModuleInfoGroup groupInfo = new ModuleInfoGroup();
                groupInfo.AddModule("NullModule", null);
            });

        }

        [Fact]
        public void ShouldSetDependencies()
        {
            string dependency1 = "ModuleA";
            string dependency2 = "ModuleB";

            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule("MockModule", typeof(MockModule), dependency1, dependency2);

            Assert.NotNull(groupInfo.ElementAt(0).DependsOn);
            Assert.Equal(2, groupInfo.ElementAt(0).DependsOn.Count);
            Assert.Contains(dependency1, groupInfo.ElementAt(0).DependsOn);
            Assert.Contains(dependency2, groupInfo.ElementAt(0).DependsOn);
        }

        [Fact]
        public void ShouldUseTypeNameIfNoNameSpecified()
        {
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule(typeof(MockModule));

            Assert.Single(groupInfo);
            Assert.Equal(typeof(MockModule).Name, groupInfo.ElementAt(0).ModuleName);
        }


        public class MockModule : IModule
        {
            public void OnInitialized(IContainerProvider containerProvider)
            {
                
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                
            }
        }
    }
}
