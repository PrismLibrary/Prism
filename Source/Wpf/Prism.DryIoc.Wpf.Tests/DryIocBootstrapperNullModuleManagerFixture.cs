using System.Windows;
using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;
using Prism.Regions;

namespace Prism.DryIoc.Wpf.Tests
{
    [TestClass]
    public class DryIocBootstrapperNullModuleManagerFixture
    {
        [TestMethod]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.IsFalse(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : DryIocBootstrapper
        {
            public bool InitializeModulesCalled;

            protected override void ConfigureContainer()
            {
                Container.RegisterInstance<ILoggerFacade>(Logger);
                Container.RegisterInstance(ModuleCatalog);
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
