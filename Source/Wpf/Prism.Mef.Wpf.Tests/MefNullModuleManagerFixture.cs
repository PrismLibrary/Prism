

using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public class MefNullModuleManagerFixture
    {
        [TestMethod]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.IsFalse(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : MefBootstrapper
        {
            public bool InitializeModulesCalled;

            public override void  RegisterDefaultTypesIfMissing()
            {
 	             //base.RegisterDefaultTypesIfMissing();
            }            

            protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
            {
                return null;
            }

            protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
            {
                return null;
            }           

            protected override DependencyObject CreateShell()
            {
                return null;
            }

            protected override void InitializeModules()
            {
                this.InitializeModulesCalled = true;
            }
        }
    }
}
