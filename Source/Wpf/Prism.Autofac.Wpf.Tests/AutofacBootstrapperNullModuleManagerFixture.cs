using System.Windows;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;
using Prism.Regions;

namespace Prism.Autofac.Wpf.Tests
{
    [TestClass]
    public class AutofacBootstrapperNullModuleManagerFixture
    {
        [TestMethod]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.IsFalse(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : AutofacBootstrapper
        {
            public bool InitializeModulesCalled;

            protected override void ConfigureContainerBuilder(ContainerBuilder builder)
            {
                builder.RegisterInstance(Logger).As<ILoggerFacade>();
                builder.RegisterInstance(ModuleCatalog);
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
