using System;
using System.Globalization;
using StructureMap;
using Microsoft.Practices.ServiceLocation;
using Prism.StructureMap.Properties;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

namespace Prism.StructureMap
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that
    /// registers most of the Prism Library assets
    /// in an StructureMap <see cref="IContainer"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    public abstract class StructureMapBootstrapper : Bootstrapper
    {
        private bool _useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default StructureMap <see cref="IContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IContainer"/> instance.</value>
        public IContainer Container { get; protected set; }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If <see langword="true"/>, registers default Prism Library services in the container. This is the default behavior.</param>
        public override void Run(bool runWithDefaultConfiguration)
        {
            _useDefaultConfiguration = runWithDefaultConfiguration;

            Logger = CreateLogger();
            if (Logger == null)
            {
                throw new InvalidOperationException(Resources.NullLoggerFacadeException);
            }

            Logger.Log(Resources.LoggerCreatedSuccessfully, Category.Debug, Priority.Low);

            Logger.Log(Resources.CreatingModuleCatalog, Category.Debug, Priority.Low);
            ModuleCatalog = CreateModuleCatalog();
            if (ModuleCatalog == null)
            {
                throw new InvalidOperationException(Resources.NullModuleCatalogException);
            }

            Logger.Log(Resources.ConfiguringModuleCatalog, Category.Debug, Priority.Low);
            ConfigureModuleCatalog();

            Logger.Log(Resources.CreatingStructureMapContainer, Category.Debug, Priority.Low);
            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException(Resources.NullStructureMapContainerException);
            }

            Logger.Log(Resources.ConfiguringStructureMapContainer, Category.Debug, Priority.Low);
            ConfigureContainer();

            Logger.Log(Resources.ConfiguringServiceLocatorSingleton, Category.Debug, Priority.Low);
            ConfigureServiceLocator();

            Logger.Log(Resources.ConfiguringViewModelLocator, Category.Debug, Priority.Low);
            ConfigureViewModelLocator();

            Logger.Log(Resources.ConfiguringRegionAdapters, Category.Debug, Priority.Low);
            ConfigureRegionAdapterMappings();

            Logger.Log(Resources.ConfiguringDefaultRegionBehaviors, Category.Debug, Priority.Low);
            ConfigureDefaultRegionBehaviors();

            Logger.Log(Resources.RegisteringFrameworkExceptionTypes, Category.Debug, Priority.Low);
            RegisterFrameworkExceptionTypes();

            Logger.Log(Resources.CreatingShell, Category.Debug, Priority.Low);
            Shell = CreateShell();
            if (Shell != null)
            {
                Logger.Log(Resources.SettingTheRegionManager, Category.Debug, Priority.Low);
                RegionManager.SetRegionManager(Shell, Container.GetInstance<IRegionManager>());

                Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                InitializeShell();
            }

            if (Container.Model.HasImplementationsFor<IModuleManager>())
            {
                Logger.Log(Resources.InitializingModules, Category.Debug, Priority.Low);
                InitializeModules();
            }

            Logger.Log(Resources.BootstrapperSequenceCompleted, Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Container.GetInstance<IServiceLocator>());
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.GetInstance(type));
        }

        /// <summary>
        /// Registers in the StructureMap <see cref="IContainer"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(StructureMapBuildPlanException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(StructureMapConfigurationException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(StructureMapException));
        }

        /// <summary>
        /// Creates the <see cref="Container"/> that will be used to create the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="Container"/>.</returns>
        protected virtual IContainer CreateContainer()
        {
            return new Container();
        }

        /// <summary>
        /// Configures the <see cref="IContainer"/>. 
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Container.Configure(config =>
                {
                    config.For<ILoggerFacade>().Use(Logger);
                    config.For<IModuleCatalog>().Use(ModuleCatalog);
                });

            if (_useDefaultConfiguration)
            {
                Container.RegisterTypeIfMissing<IServiceLocator, StructureMapServiceLocatorAdapter>(true);
                Container.RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(true);
                Container.RegisterTypeIfMissing<IModuleManager, ModuleManager>(true);
                Container.RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(true);
                Container.RegisterTypeIfMissing<IRegionManager, RegionManager>(true);
                Container.RegisterTypeIfMissing<IEventAggregator, EventAggregator>(true);
                Container.RegisterTypeIfMissing<IRegionViewRegistry, RegionViewRegistry>(true);
                Container.RegisterTypeIfMissing<IRegionBehaviorFactory, RegionBehaviorFactory>(true);
                Container.RegisterTypeIfMissing<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(false);
                Container.RegisterTypeIfMissing<IRegionNavigationJournal, RegionNavigationJournal>(false);
                Container.RegisterTypeIfMissing<IRegionNavigationService, RegionNavigationService>(false);
                Container.RegisterTypeIfMissing<IRegionNavigationContentLoader, RegionNavigationContentLoader>(true);
            }
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            IModuleManager manager;

            try
            {
                manager = Container.GetInstance<IModuleManager>();
            }
            catch (StructureMapException ex)
            {
                if (ex.Message.Contains("IModuleCatalog"))
                {
                    throw new InvalidOperationException(Resources.NullModuleCatalogException);
                }

                throw;
            }

            manager.Run();
        }
    }
}
