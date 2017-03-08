using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Prism;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Ninject.Properties;
using Prism.Regions;

namespace Prism.Ninject
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that
    /// registers most of the Prism Library assets
    /// in a <see cref="IKernel"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    public abstract class NinjectBootstrapper : Bootstrapper
    {
        private bool useDefaultConfiguration = true;

        /// <summary>
        /// Gets the default <see cref="IKernel"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IKernel"/> instance.</value>
        public IKernel Kernel { get; protected set; }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If <see langword="true"/>, registers default Prism Library services in the container. This is the default behavior.</param>
        public override void Run(bool runWithDefaultConfiguration)
        {
            this.useDefaultConfiguration = runWithDefaultConfiguration;

            this.Logger = this.CreateLogger();
            if(this.Logger == null)
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

            this.Logger.Log(Resources.CreatingNinjectKernel, Category.Debug, Priority.Low);
            this.Kernel = this.CreateKernel();
            if (this.Kernel == null)
            {
                throw new InvalidOperationException(Resources.NullNinjectKernelException);
            }

            this.Logger.Log(Resources.ConfiguringNinjectKernel, Category.Debug, Priority.Low);
            this.ConfigureKernel();

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
                RegionManager.SetRegionManager(this.Shell, this.Kernel.Get<IRegionManager>());

                this.Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                this.Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                this.InitializeShell();
            }

            if (this.Kernel.IsRegistered<IModuleManager>())
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
            ServiceLocator.SetLocatorProvider(() => this.Kernel.Get<IServiceLocator>());
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(t => this.Kernel.Get(t));
        }

        /// <summary>
        /// Registers in the <see cref="IKernel"/> the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();

            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(global::Ninject.ActivationException));
        }

        /// <summary>
        /// Creates the <see cref="IKernel"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IKernel"/>.</returns>
        protected virtual IKernel CreateKernel()
        {
            return new StandardKernel();
        }

        /// <summary>
        /// Configures the <see cref="IKernel"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureKernel()
        {
            this.Kernel.Bind<ILoggerFacade>().ToConstant(this.Logger).InSingletonScope();
            this.Kernel.Bind<IModuleCatalog>().ToConstant(this.ModuleCatalog).InSingletonScope();

            if (this.useDefaultConfiguration)
            {
                this.Kernel.RegisterTypeIfMissing<IServiceLocator, NinjectServiceLocatorAdapter>(true);
                this.Kernel.RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(true);
                this.Kernel.RegisterTypeIfMissing<IModuleManager, ModuleManager>(true);
                this.Kernel.RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionManager, RegionManager>(true);
                this.Kernel.RegisterTypeIfMissing<IEventAggregator, EventAggregator>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionViewRegistry, RegionViewRegistry>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionBehaviorFactory, RegionBehaviorFactory>(true);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(false);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationJournal, RegionNavigationJournal>(false);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationService, RegionNavigationService>(false);
                this.Kernel.RegisterTypeIfMissing<IRegionNavigationContentLoader, RegionNavigationContentLoader>(true);
            }
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            IModuleManager manager= this.Kernel.Get<IModuleManager>();
            
            if (manager == null)
            {
                throw new InvalidOperationException("Could not resolve IModuleManager");
            }

            manager.Run();
        }
    }
}