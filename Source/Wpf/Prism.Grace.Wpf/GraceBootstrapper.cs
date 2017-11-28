using CommonServiceLocator;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Prism.Events;
using Prism.Grace.Wpf.Properties;
using Prism.Grace.Wpf.Regions;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Globalization;

namespace Prism.Grace.Wpf
{
    public abstract class GraceBootstrapper : Bootstrapper
    {
        private bool useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default <see cref="DependencyInjectionContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="DependencyInjectionContainer"/> instance.</value>
        public DependencyInjectionContainer Container { get; protected set; }

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

            this.Logger.Log(Resources.CreatingGraceContainer, Category.Debug, Priority.Low);
            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException(Resources.NullGraceContainerException);
            }

            this.Logger.Log(Resources.ConfiguringGraceContainer, Category.Debug, Priority.Low);
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
                RegionManager.SetRegionManager(this.Shell, this.Container.Locate<IRegionManager>());

                this.Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                this.Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                this.InitializeShell();
            }

            if (this.Container.CanLocate(typeof(IModuleManager)))
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
            ServiceLocator.SetLocatorProvider(() => this.Container.Locate<IServiceLocator>());
        }

        /// <summary>
        /// Registers in the <see cref="DependencyInjectionContainer"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(LocateException));
        }

        /// <summary>
        /// Configures the <see cref="DependencyInjectionContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            this.Container.Configure(c => 
            {
                c.ExportInstance(this.Container);
                c.ExportInstance<ILoggerFacade>(this.Logger);
                c.ExportInstance<IModuleCatalog>(this.ModuleCatalog);
            });

            if (useDefaultConfiguration)
            {
                this.RegisterTypeIfMissing(typeof(IServiceLocator), typeof(GraceServiceLocator), true);
                this.RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);
                this.RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);
                this.RegisterTypeIfMissing(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings), true);
                this.RegisterTypeIfMissing(typeof(IRegionManager), typeof(RegionManager), true);
                this.RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
                this.RegisterTypeIfMissing(typeof(IRegionViewRegistry), typeof(RegionViewRegistry), true);
                this.RegisterTypeIfMissing(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory), true);
                this.RegisterTypeIfMissing(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry), false);
                this.RegisterTypeIfMissing(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal), false);
                this.RegisterTypeIfMissing(typeof(IRegionNavigationService), typeof(RegionNavigationService), false);
                this.RegisterTypeIfMissing(typeof(IRegionNavigationContentLoader), typeof(GraceRegionNavigationContentLoader), true);
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
                manager = this.Container.Locate<IModuleManager>();
            }
            catch (LocateException ex)
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
        /// Creates the <see cref="DependencyInjectionContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="DependencyInjectionContainer"/>.</returns>
        protected virtual DependencyInjectionContainer CreateContainer()
        {
            return new DependencyInjectionContainer();
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

            this.Container.Configure(c => 
            {
                if (c.IsExported(fromType))
                {
                    this.Logger.Log(
                    String.Format(CultureInfo.CurrentCulture,
                                  Resources.TypeMappingAlreadyRegistered,
                                  fromType.Name), Category.Debug, Priority.Low);
                }
                else
                {
                    if (registerAsSingleton)
                    {
                        c.Export(toType).As(fromType).Lifestyle.Singleton();
                    }
                    else
                    {
                        c.Export(toType).As(fromType);
                    }
                }
            });
        }
    }
}