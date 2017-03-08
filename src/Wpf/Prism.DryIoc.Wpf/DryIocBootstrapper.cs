using System;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism;
using DryIoc;
using Prism.DryIoc.Properties;
using DryIoc.CommonServiceLocator;

namespace Prism.DryIoc
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that
    /// registers most of the Prism Library assets
    /// in an DryIoc <see cref="IContainer"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    public abstract class DryIocBootstrapper : Bootstrapper
    {
        private bool _useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default DryIoc <see cref="IContainer"/> for the application.
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

            Logger.Log(Resources.CreatingDryIocContainer, Category.Debug, Priority.Low);
            Container = CreateContainer();
            if (Container == null)
            {
                throw new InvalidOperationException(Resources.NullDryIocContainerException);
            }

            Logger.Log(Resources.ConfiguringDryIocContainer, Category.Debug, Priority.Low);
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
                RegionManager.SetRegionManager(Shell, Container.Resolve<IRegionManager>());

                Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                InitializeShell();
            }

            if (Container.IsRegistered<IModuleManager>())
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
            DryIocServiceLocator serviceLocator = new DryIocServiceLocator(Container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            // register the locator in DryIoc as well
            Container.RegisterInstance<IServiceLocator>(serviceLocator);
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));
        }

        /// <summary>
        /// Registers in the DryIoc <see cref="IContainer"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ContainerException));
        }

        /// <summary>
        /// Configures the <see cref="Container"/>.
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Container.RegisterInstance<ILoggerFacade>(Logger);
            Container.RegisterInstance<IModuleCatalog>(ModuleCatalog);

            if (_useDefaultConfiguration)
            {
                RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(true);
                RegisterTypeIfMissing<IModuleManager, ModuleManager>(true);
                RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(true);
                RegisterTypeIfMissing<IRegionManager, RegionManager>(true);
                RegisterTypeIfMissing<IEventAggregator, EventAggregator>(true);
                RegisterTypeIfMissing<IRegionViewRegistry, RegionViewRegistry>(true);
                RegisterTypeIfMissing<IRegionBehaviorFactory, RegionBehaviorFactory>(true);
                RegisterTypeIfMissing<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(false);
                RegisterTypeIfMissing<IRegionNavigationJournal, RegionNavigationJournal>(false);
                RegisterTypeIfMissing<IRegionNavigationService, RegionNavigationService>(false);
                RegisterTypeIfMissing<IRegionNavigationContentLoader, RegionNavigationContentLoader>(true);
            }
        }

        /// <summary>
        /// Creates the DryIoc <see cref="IContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IContainer"/>.</returns>
        protected virtual IContainer CreateContainer()
        {
            return new Container(Rules.Default.WithAutoConcreteTypeResolution());
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            IModuleManager manager;

            try
            {
                manager = Container.Resolve<IModuleManager>();
            }
            catch (ContainerException ex)
            {
                if (ex.Message.Contains("IModuleCatalog"))
                {
                    throw new InvalidOperationException(Resources.NullModuleCatalogException);
                }

                throw;
            }

            manager.Run();
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <typeparam name="TFrom">The interface type to register.</typeparam>
        /// <typeparam name="TTarget">The type implementing the interface.</typeparam>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing<TFrom, TTarget>(bool registerAsSingleton = false) where TTarget : TFrom
        {
            if(Container!=null && Container.IsRegistered<TFrom>())
            {
                Logger.Log(String.Format(CultureInfo.CurrentCulture, Resources.TypeMappingAlreadyRegistered, typeof(TFrom).Name),
                    Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    Container.Register<TFrom, TTarget>(Reuse.Singleton);
                }
                else
                {
                    Container.Register<TFrom, TTarget>();
                }
            }
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="fromType">The interface type to register.</param>
        /// <param name="toType">The type implementing the interface.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing(Type fromType, Type toType, bool registerAsSingleton)
        {
            if (fromType == null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }
            if (toType == null)
            {
                throw new ArgumentNullException(nameof(toType));
            }
            if (Container.IsRegistered(fromType))
            {
                Logger.Log(String.Format(CultureInfo.CurrentCulture, Resources.TypeMappingAlreadyRegistered, fromType.Name),
                    Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    Container.Register(fromType, toType, Reuse.Singleton);
                }
                else
                {
                    Container.Register(fromType, toType);
                }
            }
        }
    }
}
