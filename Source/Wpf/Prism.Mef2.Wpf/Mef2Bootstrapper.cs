using System;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Prism.Regions.Behaviors;

namespace Prism.Mef2.Wpf
{
    public abstract class Mef2Bootstrapper : Bootstrapper
    {
        [Export]
        public CompositionHost Container { get; set; }
        protected ContainerConfiguration ContainerConfig { get; set; }

        protected ConventionBuilder Builder { set; get; }

        protected Mef2Bootstrapper()
        {
            ContainerConfig = new ContainerConfiguration();
            ConfigureConventions();
            ContainerConfig.WithAssemblies(AppDomain.CurrentDomain.GetAssemblies(), Builder);
        }

        protected override void InitializeModules()
        {
            this.Container.GetExport<IModuleManager>().Run();
        }

        protected virtual void ConfigureConventions()
        {
            Builder = new ConventionBuilder();

            Builder.ForType<EmptyLogger>().ExportInterfaces().Shared();
            Builder.ForType<ModuleCatalog>().ExportInterfaces().Shared();
            Builder.ForType<Mef2ServiceLocator>().ExportInterfaces().Shared();
            Builder.ForType<RegionAdapterMappings>().Export().Shared();

            Builder.ForType<SelectorRegionAdapter>().Export().ExportInterfaces().Shared();
            Builder.ForType<ItemsControlRegionAdapter>().Export().ExportInterfaces().Shared();
            Builder.ForType<ContentControlRegionAdapter>().Export().ExportInterfaces().Shared();

            Builder.ForType<RegionManager>().Export().ExportInterfaces().Shared();
            Builder.ForType<EventAggregator>().ExportInterfaces().Shared();
            Builder.ForType<RegionNavigationJournalEntry>().ExportInterfaces().Shared();
            Builder.ForType<RegionViewRegistry>().ExportInterfaces().Shared();
            Builder.ForType<RegionNavigationJournal>().ExportInterfaces().Shared();
            Builder.ForType<RegionNavigationContentLoader>().ExportInterfaces().Shared();
            Builder.ForType<RegionBehaviorFactory>().ExportInterfaces().Shared();
            Builder.ForType<ModuleManager>().ExportInterfaces().Shared();
            Builder.ForType<ModuleInitializer>().ExportInterfaces().Shared();


            Builder.ForType<AutoPopulateRegionBehavior>().Export().ExportInterfaces();
            Builder.ForType<BindRegionContextToDependencyObjectBehavior>().Export().ExportInterfaces();
            Builder.ForType<ClearChildViewsRegionBehavior>().Export().ExportInterfaces();
            Builder.ForType<DelayedRegionCreationBehavior>().Export().ExportInterfaces();
            Builder.ForType<RegionActiveAwareBehavior>().Export().ExportInterfaces();
            Builder.ForType<RegionManagerRegistrationBehavior>().Export().ExportInterfaces();
            Builder.ForType<RegionMemberLifetimeBehavior>().Export().ExportInterfaces();
            Builder.ForType<SelectorItemsSourceSyncBehavior>().Export().ExportInterfaces();
            Builder.ForType<SyncRegionContextWithHostBehavior>().Export().ExportInterfaces();
        }

        protected override void ConfigureServiceLocator()
        {
            var serviceLocator = Container.GetExport<IServiceLocator>();
            ServiceLocator.SetLocatorProvider((() => serviceLocator));
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var instance = Container.GetExport<RegionAdapterMappings>();
            instance.RegisterMapping(typeof(Selector), Container.GetExport<SelectorRegionAdapter>());
            instance.RegisterMapping(typeof(ItemsControl), Container.GetExport<ItemsControlRegionAdapter>());
            instance.RegisterMapping(typeof(ContentControl), Container.GetExport<ContentControlRegionAdapter>());
            return instance;
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            this.ModuleCatalog = this.CreateModuleCatalog();
            this.ConfigureModuleCatalog();
            this.Container = ContainerConfig.CreateContainer();
            Mef2ServiceLocator.Host = Container;
            this.ConfigureServiceLocator();
            this.ConfigureViewModelLocator();
            this.ConfigureRegionAdapterMappings();
            this.ConfigureDefaultRegionBehaviors();
            this.RegisterFrameworkExceptionTypes();

            this.Shell = this.CreateShell();

            if (this.Shell != null)
            {
                RegionManager.SetRegionManager(this.Shell, this.Container.GetExport<IRegionManager>());      
                RegionManager.UpdateRegions();
                this.InitializeShell();
            }

            var exports = Container.GetExports<IModuleManager>();
            if (exports != null && exports.Any())
            {
                this.InitializeModules();
            }

            var modules = Container.GetExports<IModule>();
            foreach (var module in modules)
            {
                module.Initialize();
            }
        }
    }
}
