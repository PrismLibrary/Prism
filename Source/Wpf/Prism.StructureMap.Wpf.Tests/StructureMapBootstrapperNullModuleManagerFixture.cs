using System.Windows;
using CommonServiceLocator;
using Xunit;
using Prism.Logging;
using Prism.Regions;
using StructureMap;
using Prism.Modularity;

namespace Prism.StructureMap.Wpf.Tests
{
    
    public class StructureMapBootstrapperNullModuleManagerFixture
    {
        [Fact]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.False(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : StructureMapBootstrapper
        {
            public bool InitializeModulesCalled;

            protected override IContainer CreateContainer()
            {
                return new Container();
            }

            protected override void ConfigureContainer()
            {
                //base.RegisterDefaultTypesIfMissing();

                //this.Container.AddNewExtension<StructureMapBootstrapperExtension>();

                Container.Configure(config =>
                {
                    config.For<ILoggerFacade>().Use(Logger);
                    config.For<IModuleCatalog>().Use(ModuleCatalog);
                });

                //RegisterTypeIfMissing(typeof(IServiceLocator), typeof(StructureMapServiceLocatorAdapter), true);
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
