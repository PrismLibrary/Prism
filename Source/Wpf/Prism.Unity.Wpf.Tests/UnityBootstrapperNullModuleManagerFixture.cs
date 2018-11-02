

using System.Windows;
using CommonServiceLocator;
using Unity;
using Xunit;
using Prism.Logging;
using Prism.Regions;

namespace Prism.Unity.Wpf.Tests
{
    
    public class UnityBootstrapperNullModuleManagerFixture
    {
        [Fact]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.False(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : UnityBootstrapper
        {
            public bool InitializeModulesCalled;

            protected override void ConfigureContainer()
            {
                //base.RegisterDefaultTypesIfMissing();
                
                this.Container.AddNewExtension<UnityBootstrapperExtension>();

                Container.RegisterInstance<ILoggerFacade>(Logger);

                this.Container.RegisterInstance(this.ModuleCatalog);
                RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
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
