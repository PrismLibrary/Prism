

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Ioc;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    /// <summary>
    /// Summary description for ModuleInfoGroupExtensionsFixture
    /// </summary>
    [TestClass]
    public class ModuleInfoGroupExtensionsFixture
    {
        [TestMethod]
        public void ShouldAddModuleToModuleInfoGroup()
        {
            string moduleName = "MockModule";
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule(moduleName, typeof(MockModule));

            Assert.AreEqual<int>(1, groupInfo.Count);
            Assert.AreEqual<string>(moduleName, groupInfo.ElementAt(0).ModuleName);
        }

        [TestMethod]
        public void ShouldSetModuleTypeCorrectly()
        {
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule("MockModule", typeof(MockModule));

            Assert.AreEqual<int>(1, groupInfo.Count);
            Assert.AreEqual<string>(typeof(MockModule).AssemblyQualifiedName, groupInfo.ElementAt(0).ModuleType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTypeThrows()
        {
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule("NullModule", null);
        }

        [TestMethod]
        public void ShouldSetDependencies()
        {
            string dependency1 = "ModuleA";
            string dependency2 = "ModuleB";

            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule("MockModule", typeof(MockModule), dependency1, dependency2);

            Assert.IsNotNull(groupInfo.ElementAt(0).DependsOn);
            Assert.AreEqual(2, groupInfo.ElementAt(0).DependsOn.Count);
            Assert.IsTrue(groupInfo.ElementAt(0).DependsOn.Contains(dependency1));
            Assert.IsTrue(groupInfo.ElementAt(0).DependsOn.Contains(dependency2));
        }

        [TestMethod]
        public void ShouldUseTypeNameIfNoNameSpecified()
        {
            ModuleInfoGroup groupInfo = new ModuleInfoGroup();
            groupInfo.AddModule(typeof(MockModule));

            Assert.AreEqual<int>(1, groupInfo.Count);
            Assert.AreEqual<string>(typeof(MockModule).Name, groupInfo.ElementAt(0).ModuleName);
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
