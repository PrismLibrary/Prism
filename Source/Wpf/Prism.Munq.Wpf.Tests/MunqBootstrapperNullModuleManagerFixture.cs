using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;

namespace Prism.Munq.Wpf.Tests
{
    [TestClass]
    public class MunqBootstrapperNullModuleManagerFixture
    {
        [TestMethod]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.IsFalse(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : MunqBootstrapper
        {
            public bool InitializeModulesCalled;

            protected override void ConfigureContainer()
            {
                this.Container.RegisterInstance(this.Logger);
                this.Container.RegisterInstance(this.ModuleCatalog);

                RegisterTypeIfMissing<IServiceLocator>(_ => new MunqServiceLocatorAdapter(Container), true);
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
