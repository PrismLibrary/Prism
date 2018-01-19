using System;
using System.Globalization;
using Autofac;
using Autofac.Features.ResolveAnything;
using AutofacCore = Autofac.Core;
using CommonServiceLocator;
using Prism.Autofac.Properties;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity.Regions;
using Prism.Ioc;
using Prism.Autofac.Ioc;

namespace Prism.Autofac
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that
    /// registers most of the Prism Library assets
    /// in an Autofac <see cref="IContainer"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    [Obsolete("It is recommended to use the new PrismApplication as the app's base class. This will require updating the App.xaml and App.xaml.cs files.", false)]
    public abstract class AutofacBootstrapper : Bootstrapper
    {
        private bool _useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default Autofac <see cref="IContainer"/> for the application.
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

            Logger.Log(Resources.CreatingAutofacContainerBuilder, Category.Debug, Priority.Low);
            ContainerBuilder builder = CreateContainerBuilder();
            if (builder == null)
            {
                throw new InvalidOperationException(Resources.NullAutofacContainerBuilderException);
            }

            _containerExtension = CreateContainerExtension();

            Logger.Log(Resources.ConfiguringAutofacContainerBuilder, Category.Debug, Priority.Low);
            // Make sure any not specifically registered concrete type can resolve.
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            ConfigureContainerBuilder(builder);

            Logger.Log(Resources.CreatingAutofacContainer, Category.Debug, Priority.Low);
            Container = CreateContainer(builder);
            if (Container == null)
            {
                throw new InvalidOperationException(Resources.NullAutofacContainerException);
            }

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
            var serviceLocator = new AutofacServiceLocatorAdapter(Container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            // register the locator in Autofac as well
            RegisterInstance(serviceLocator, typeof(IServiceLocator), registerAsSingleton: true);
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));
        }

        /// <summary>
        /// Registers in the Autofac <see cref="IContainer"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.DependencyResolutionException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.Registration.ComponentNotRegisteredException));
        }

        /// <summary>
        /// Creates the <see cref="ContainerBuilder"/> that will be used to create the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="ContainerBuilder"/>.</returns>
        protected virtual ContainerBuilder CreateContainerBuilder()
        {
            return new ContainerBuilder();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new AutofacContainerExtension(Container);
        }

        /// <summary>
        /// Configures the <see cref="ContainerBuilder"/>.
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterInstance(_containerExtension).As<IContainerExtension>();
            builder.RegisterInstance(Logger).As<ILoggerFacade>();
            builder.RegisterInstance(ModuleCatalog);

            if (_useDefaultConfiguration)
            {
                RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(builder, true);
                RegisterTypeIfMissing<IModuleManager, ModuleManager>(builder, true);
                RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(builder, true);
                RegisterTypeIfMissing<IRegionManager, RegionManager>(builder, true);
                RegisterTypeIfMissing<IEventAggregator, EventAggregator>(builder, true);
                RegisterTypeIfMissing<IRegionViewRegistry, RegionViewRegistry>(builder, true);
                RegisterTypeIfMissing<IRegionBehaviorFactory, RegionBehaviorFactory>(builder, true);
                RegisterTypeIfMissing<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(builder, false);
                RegisterTypeIfMissing<IRegionNavigationJournal, RegionNavigationJournal>(builder, false);
                RegisterTypeIfMissing<IRegionNavigationService, RegionNavigationService>(builder, false);
                RegisterTypeIfMissing<IRegionNavigationContentLoader, AutofacRegionNavigationContentLoader>(builder, true);
            }
        }

        /// <summary>
        /// Creates the Autofac <see cref="IContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IContainer"/>.</returns>
        protected virtual IContainer CreateContainer(ContainerBuilder containerBuilder)
        {
            IContainer container = containerBuilder.Build();

            // Register container instance
            var updater = new ContainerBuilder();
            updater.RegisterInstance(container);
            updater.Update(container);

            return container;
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
            catch (AutofacCore.DependencyResolutionException ex)
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
        /// <param name="builder">The <see cref="ContainerBuilder"/> instance.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing<TFrom, TTarget>(ContainerBuilder builder, bool registerAsSingleton = false)
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
                    builder.RegisterType<TTarget>().As<TFrom>().SingleInstance();
                }
                else
                {
                    builder.RegisterType<TTarget>().As<TFrom>();
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
                ContainerBuilder builder = CreateContainerBuilder();
                if (registerAsSingleton)
                {
                    builder.RegisterType(toType).As(fromType).SingleInstance();
                }
                else
                {
                    builder.RegisterType(toType).As(fromType);
                }
                builder.Update(Container);
            }
        }

        /// <summary>
        /// Registers an object instance in the container.
        /// </summary>
        /// <param name="instance">Object instance.</param>
        /// <param name="fromType">The interface type to register.</param>
        /// <param name="key">Optional key for registration.</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterInstance<T>(T instance, Type fromType, string key = "", bool registerAsSingleton = false)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (fromType == null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            ContainerBuilder containerUpdater = CreateContainerBuilder();

            var registration = containerUpdater.RegisterInstance(instance);
            // named instance
            if (!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key, fromType);
            }
            else
            {
                registration = registration.As(fromType);
            }
            // singleton
            if (registerAsSingleton)
            {
                registration.SingleInstance();
            }

            containerUpdater.Update(Container);
        }
    }
}
