using System.Windows;
using Prism.Ioc;
using Prism.Logging;
using Prism.Regions;

namespace Prism.Container.Wpf.Mocks
{
    internal partial class NullModuleManagerBootstrapper
    {
        public bool InitializeModulesCalled;

        protected override void ConfigureContainer()
        {
            //base.RegisterDefaultTypesIfMissing();
            ContainerExtension.RegisterInstance<ILoggerFacade>(Logger);

            ContainerExtension.RegisterInstance(this.ModuleCatalog);
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
