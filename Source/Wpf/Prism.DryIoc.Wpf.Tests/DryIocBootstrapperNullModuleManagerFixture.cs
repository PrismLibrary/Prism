using System.Windows;
using DryIoc;
using Xunit;
using Prism.Logging;
using Prism.Regions;

namespace Prism.DryIoc.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class DryIocBootstrapperNullModuleManagerFixture
    {
        [Fact]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.False(bootstrapper.InitializeModulesCalled);
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
