

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CommonServiceLocator;
using Prism.Logging;
using Prism.Mef.Properties;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Mef
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that registers most of the Prism Library assets in a MEF <see cref="CompositionContainer"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    public abstract class MefBootstrapper : Bootstrapper
    {
        /// <summary>
        /// Gets or sets the default <see cref="AggregateCatalog"/> for the application.
        /// </summary>
        /// <value>The default <see cref="AggregateCatalog"/> instance.</value>
        protected AggregateCatalog AggregateCatalog { get; set; }

        /// <summary>
        /// Gets or sets the default <see cref="CompositionContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="CompositionContainer"/> instance.</value>
        protected CompositionContainer Container { get; set; }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If <see langword="true"/>, registers default 
        /// Prism Library services in the container. This is the default behavior.</param>
        public override void Run(bool runWithDefaultConfiguration)
        {
            this.Logger = this.CreateLogger();

            if (this.Logger == null)
            {
                throw new InvalidOperationException(Resources.NullLoggerFacadeException);
            }

            this.Logger.Log(Resources.LoggerWasCreatedSuccessfully, Category.Debug, Priority.Low);

            this.Logger.Log(Resources.CreatingModuleCatalog, Category.Debug, Priority.Low);
            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
            {
                throw new InvalidOperationException(Resources.NullModuleCatalogException);
            }

            this.Logger.Log(Resources.ConfiguringModuleCatalog, Category.Debug, Priority.Low);
            this.ConfigureModuleCatalog();

            this.Logger.Log(Resources.CreatingCatalogForMEF, Category.Debug, Priority.Low);
            this.AggregateCatalog = this.CreateAggregateCatalog();

            this.Logger.Log(Resources.ConfiguringCatalogForMEF, Category.Debug, Priority.Low);
            this.ConfigureAggregateCatalog();

            this.RegisterDefaultTypesIfMissing();

            this.Logger.Log(Resources.CreatingMefContainer, Category.Debug, Priority.Low);
            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException(Resources.NullCompositionContainerException);
            }

            this.Logger.Log(Resources.ConfiguringMefContainer, Category.Debug, Priority.Low);
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
                RegionManager.SetRegionManager(this.Shell, this.Container.GetExportedValue<IRegionManager>());

                this.Logger.Log(Resources.UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                this.Logger.Log(Resources.InitializingShell, Category.Debug, Priority.Low);
                this.InitializeShell();
            }

            IEnumerable<Lazy<object, object>> exports = this.Container.GetExports(typeof(IModuleManager), null, null);
            if ((exports != null) && (exports.Count() > 0))
            {
                this.Logger.Log(Resources.InitializingModules, Category.Debug, Priority.Low);
                this.InitializeModules();
            }

            this.Logger.Log(Resources.BootstrapperSequenceCompleted, Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new AggregateCatalog.
        /// </remarks>
        /// <returns>An <see cref="AggregateCatalog"/> to be used by the bootstrapper.</returns>
        protected virtual AggregateCatalog CreateAggregateCatalog()
        {
            return new AggregateCatalog();
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected virtual void ConfigureAggregateCatalog()
        {
        }

        /// <summary>
        /// Creates the <see cref="CompositionContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="CompositionContainer"/>.</returns>
        /// <remarks>
        /// The base implementation registers a default MEF catalog of exports of key Prism types.
        /// Exporting your own types will replace these defaults.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "The default export provider is in the container and disposed by MEF.")]
        protected virtual CompositionContainer CreateContainer()
        {
            CompositionContainer container = new CompositionContainer(this.AggregateCatalog);
            return container;
        }

        /// <summary>
        /// Configures the <see cref="CompositionContainer"/>. 
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
        /// If the method is overwritten, the new implementation should call the base class version.
        /// </remarks>
        protected virtual void ConfigureContainer()
        {
            this.RegisterBootstrapperProvidedTypes();
        }

        /// <summary>
        /// Helper method for configuring the <see cref="CompositionContainer"/>. 
        /// Registers defaults for all the types necessary for Prism to work, if they are not already registered.
        /// </summary>
        public virtual void RegisterDefaultTypesIfMissing()
        {
            this.AggregateCatalog = DefaultPrismServiceRegistrar.RegisterRequiredPrismServicesIfMissing(this.AggregateCatalog);
        }

        /// <summary>
        /// Helper method for configuring the <see cref="CompositionContainer"/>. 
        /// Registers all the types direct instantiated by the bootstrapper with the container.
        /// </summary>
        protected virtual void RegisterBootstrapperProvidedTypes()
        {
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
            this.Container.ComposeExportedValue<IModuleCatalog>(this.ModuleCatalog);
            this.Container.ComposeExportedValue<IServiceLocator>(new MefServiceLocatorAdapter(this.Container));
            this.Container.ComposeExportedValue<AggregateCatalog>(this.AggregateCatalog);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="Microsoft.Practices.ServiceLocation.ServiceLocator" />.
        /// </summary>
        /// <remarks>
        /// The base implementation also sets the ServiceLocator provider singleton.
        /// </remarks>
        protected override void ConfigureServiceLocator()
        {
            IServiceLocator serviceLocator = this.Container.GetExportedValue<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <remarks>
        /// The base implementation ensures the shell is composed in the container.
        /// </remarks>
        protected override void InitializeShell()
        {
            this.Container.ComposeParts(this.Shell);
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            IModuleManager manager = this.Container.GetExportedValue<IModuleManager>();
            manager.Run();
        }
    }
}