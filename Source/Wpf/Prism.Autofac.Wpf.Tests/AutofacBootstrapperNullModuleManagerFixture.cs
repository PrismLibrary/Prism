using System.Windows;
using Autofac;
using Xunit;
using Prism.Logging;
using Prism.Regions;

namespace Prism.Autofac.Wpf.Tests
{
    
    public class AutofacBootstrapperNullModuleManagerFixture
    {
        [Fact]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.False(bootstrapper.InitializeModulesCalled);
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
