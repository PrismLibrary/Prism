

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ModuleInfoGroupFixture
    {
        [TestMethod]
        public void ShouldForwardValuesToModuleInfo()
        {
            ModuleInfoGroup group = new ModuleInfoGroup();
            group.Ref = "MyCustomGroupRef";
            ModuleInfo moduleInfo = new ModuleInfo();
            Assert.IsNull(moduleInfo.Ref);

            group.Add(moduleInfo);

            Assert.AreEqual(group.Ref, moduleInfo.Ref);
        }
    }
}
