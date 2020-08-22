using System.Windows;
using Prism.Ioc;
using Prism.Regions;

namespace Prism.Container.Wpf.Mocks
{
    internal partial class NullModuleManagerBootstrapper
    {
        public bool InitializeModulesCalled;

        protected override void ConfigureContainer()
        {
            //base.RegisterDefaultTypesIfMissing();

            ContainerExtension.RegisterInstance(ModuleCatalog);
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
