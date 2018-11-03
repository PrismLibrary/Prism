using System.Linq;
using System.Windows;
using CommonServiceLocator;
using Xunit;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests
{
    
    public class NinjectBootstrapperNullModuleManagerFixture
    {
        [Fact]
        public void RunShouldNotCallInitializeModulesWhenModuleManagerNotFound()
        {
            var bootstrapper = new NullModuleManagerBootstrapper();

            bootstrapper.Run();

            Assert.False(bootstrapper.InitializeModulesCalled);
        }

        private class NullModuleManagerBootstrapper : NinjectBootstrapper
        {
            public bool InitializeModulesCalled;

            protected override void ConfigureKernel()
            {
                Kernel.Bind<ILoggerFacade>().ToConstant(Logger);
                Kernel.Bind<IModuleCatalog>().ToConstant(ModuleCatalog);

                if (!Kernel.GetBindings(typeof(IServiceLocator)).Any())
                    Kernel.Bind<IServiceLocator>().To<NinjectServiceLocatorAdapter>().InSingletonScope();
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
                InitializeModulesCalled = true;
            }
        }
    }
}