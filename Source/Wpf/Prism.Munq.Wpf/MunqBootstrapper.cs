    using System;
    using System.Globalization;
    using global::Munq;
    using Microsoft.Practices.ServiceLocation;
    using Prism.Events;
    using Prism.Logging;
    using Prism.Modularity;
    using Prism.Munq.Properties;
    using Prism.Regions;
    using Prism.Munq.Regions;
namespace Prism.Munq
{

    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that
    /// registers most of the Prism Library assets
    /// in a <see cref="MunqContainerWrapper"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class MunqBootstrapper : Bootstrapper
    {
        private bool useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default <see cref="IMunqContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IMunqContainer"/> instance.</value>
        public IMunqContainer Container { get; protected set; }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If <see langword="true"/>, registers default Prism Library services in the container. This is the default behavior.</param>
        public override void Run(bool runWithDefaultConfiguration)
        {
            this.useDefaultConfiguration = runWithDefaultConfiguration;

            this.Logger = this.CreateLogger();
            if (this.Logger == null)
            {
                throw new InvalidOperationException(Resources.NullLoggerFacadeException);
            }

            this.Logger.Log(Resources.LoggerCreatedSuccessfully, Category.Debug, Priority.Low);

            this.Logger.Log(Resources.CreatingModuleCatalog, Category.Debug, Priority.Low);
            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
            {
                throw new InvalidOperationException(Resources.NullModuleCatalogException);
            }

            this.Logger.Log(Resources.ConfiguringModuleCatalog, Category.Debug, Priority.Low);
            this.ConfigureModuleCatalog();

            this.Logger.Log(Resources.CreatingMunqContainer, Category.Debug, Priority.Low);
            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException(Resources.NullMunqContainerException);
            }

            this.Logger.Log(Resources.ConfiguringMunqContainer, Category.Debug, Priority.Low);
            this.ConfigureContainer();

            this.Logger.Log(Resources.ConfiguringServiceLocatorSingleton, Category.Debug, Priority.Low);
            this.ConfigureServiceLocator();

            this.Logger.Log(Resources.ConfiguringViewModelLocator, Category.Debug, Priority.Low);
            this.ConfigureViewModelLocator();

            this.Logger.Log(Resources.ConfiguringRegionAdapters, Category.Debug, Priority.Low);
            this.ConfigureRegionAdapterMappings();

            this.Logger.Log(Resources.ConfiguringDefaultRegionBehaviors, Category.Debug, Priority.Low);
            this.ConfigureDefaultRegionBehaviors();

            this.Logger.Log(Resources.RegisteringFrameworkExceptionTypes, Category.Debug, Priority.Low);
            this.RegisterFrameworkExceptionTypes();

            this.Logger.Log(Resources.CreatingShell, Category.Debug, Priority.Low);
            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                this.Logger.Log(Resources.SettingTheRegionManager, Category.Debug, Priority.Low);
                RegionManager.SetRegionManager(this.Shell, this.Container.Resolve<IRegionManager>());

                this.Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                this.Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                this.InitializeShell();
            }

            if (this.Container.CanResolve<IModuleManager>())
            {
                this.Logger.Log(Resources.InitializingModules, Category.Debug, Priority.Low);
                this.InitializeModules();
            }

            this.Logger.Log(Resources.BootstrapperSequenceCompleted, Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => this.Container.Resolve<IServiceLocator>());
        }

        /// <summary>
        /// Configures the <see cref="IUnityContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            this.Container.RegisterInstance(Logger);

            this.Container.RegisterInstance(this.ModuleCatalog);

            if (useDefaultConfiguration)
            {
                RegisterTypeIfMissing<IServiceLocator>(_ => 
                    new MunqServiceLocatorAdapter(
                        Container), true);

                RegisterTypeIfMissing<IModuleInitializer>(_ => 
                    new ModuleInitializer(
                        _.Resolve<IServiceLocator>(), 
                        _.Resolve<ILoggerFacade>()), true);

                RegisterTypeIfMissing<IModuleManager>(_ =>
                    new ModuleManager(
                        _.Resolve<IModuleInitializer>(), 
                        _.Resolve<IModuleCatalog>(), 
                        _.Resolve<ILoggerFacade>()), true);

                RegisterTypeIfMissing(_ => new RegionAdapterMappings(), true);
                RegisterTypeIfMissing<IRegionManager>(_ => new RegionManager(), true);
                RegisterTypeIfMissing<IEventAggregator>(_ => new EventAggregator(), true);

                RegisterTypeIfMissing<IRegionViewRegistry>(_ => 
                    new RegionViewRegistry(
                        _.Resolve<IServiceLocator>()), true);

                RegisterTypeIfMissing<IRegionBehaviorFactory>(_ => 
                    new RegionBehaviorFactory(
                        _.Resolve<IServiceLocator>()), true);

                RegisterTypeIfMissing<IRegionNavigationJournalEntry>(_ => new RegionNavigationJournalEntry(), false);
                RegisterTypeIfMissing<IRegionNavigationJournal>(_ => new RegionNavigationJournal(), false);

                RegisterTypeIfMissing<IRegionNavigationService>(_ => 
                    new RegionNavigationService(
                        _.Resolve<IServiceLocator>(), 
                        _.Resolve<IRegionNavigationContentLoader>(), 
                        _.Resolve<IRegionNavigationJournal>()), false);

                RegisterTypeIfMissing<IRegionNavigationContentLoader>(_ => 
                    new MunqRegionNavigationContentLoader(
                        _.Resolve<IServiceLocator>(),
                        Container), true);
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
                manager = this.Container.Resolve<IModuleManager>();
            }
            catch (Exception ex)
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
        /// Creates the <see cref="IMunqContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IMunqContainer"/>.</returns>
        protected virtual IMunqContainer CreateContainer()
        {
            var container = new MunqContainerWrapper();

            container.RegisterInstance<IMunqContainer>(container);
            container.RegisterInstance<IDependecyRegistrar>(container);
            container.RegisterInstance<IDependencyResolver>(container);

            return container;
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="factory">The factory method for creating the instance</param>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing<TFrom>(Func<IDependencyResolver, TFrom> factory, bool registerAsSingleton) where TFrom: class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (Container.CanResolve<TFrom>())
            {
                Logger.Log(
                    string.Format(CultureInfo.CurrentCulture,
                                  Resources.TypeMappingAlreadyRegistered,
                                  typeof(TFrom).Name), Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    var registration = Container.Register(factory);
                    if (registration != null)
                        registration.AsContainerSingleton();
                }
                else
                {
                    Container.Register(factory);
                }
            }
        }
    }
}
