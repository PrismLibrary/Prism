using Prism.Common;
using Prism.Modularity;
using Prism.Navigation.Regions;

namespace Prism
{
    /// <summary>
    /// Base application class that provides a basic initialization sequence
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// </remarks>
    public abstract class PrismApplicationBase : Application
    {
        private IContainerExtension _containerExtension;
        private IModuleCatalog _moduleCatalog;

        /// <summary>Main window.</summary>
        public AvaloniaObject MainWindow { get; private set; }

        /// <summary>The dependency injection container used to resolve objects.</summary>
        public IContainerProvider Container => _containerExtension;

        /// <summary>Configures the <see cref="Prism.Mvvm.ViewModelLocator"/> used by Prism.</summary>
        protected virtual void ConfigureViewModelLocator()
        {
            PrismInitializationExtensions.ConfigureViewModelLocator();
        }

        /// <summary>
        /// Runs the initialization sequence to configure the Prism application.
        /// </summary>
        /// <remarks>
        ///   Though, Prism.WPF v8.1 uses, `protected virtual void Initialize()`
        ///   Avalonia's AppBuilderBase.cs calls, `.Setup() { ... Instance.Initialize(); ... }`
        ///   Therefore, we need this as a `public override void` in PrismApplicationBase.cs
        /// </remarks>
        public override void Initialize()
        {
            base.Initialize();

            ConfigureViewModelLocator();

            ContainerLocator.SetContainerExtension(CreateContainerExtension());
            _containerExtension = ContainerLocator.Current;
            _moduleCatalog = CreateModuleCatalog();
            RegisterRequiredTypes(_containerExtension);
            RegisterTypes(_containerExtension);

            ConfigureModuleCatalog(_moduleCatalog);

            var regionAdapterMappings = _containerExtension.Resolve<RegionAdapterMappings>();
            ConfigureRegionAdapterMappings(regionAdapterMappings);

            var defaultRegionBehaviors = _containerExtension.Resolve<IRegionBehaviorFactory>();
            ConfigureDefaultRegionBehaviors(defaultRegionBehaviors);

            RegisterFrameworkExceptionTypes();

            var shell = CreateShell();
            if (shell != null)
            {
                MvvmHelpers.AutowireViewModel(shell);
                RegionManager.SetRegionManager(shell, _containerExtension.Resolve<IRegionManager>());
                RegionManager.UpdateRegions();
                InitializeShell(shell);
            }

            InitializeModules();

            OnInitialized();
        }

        /// <summary>Framework initialization has completed.</summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                desktopLifetime.MainWindow = MainWindow as Window;
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
                singleViewLifetime.MainView = MainWindow as Control;

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>Creates the container used by Prism.</summary>
        /// <returns>The container</returns>
        protected abstract IContainerExtension CreateContainerExtension();

        /// <summary>Creates the <see cref="IModuleCatalog"/> used by Prism.</summary>
        ///  <remarks>The base implementation returns a new ModuleCatalog.</remarks>
        protected virtual IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog();
        }

        /// <summary>Registers all types that are required by Prism to function with the container.</summary>
        /// <param name="containerRegistry"></param>
        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterRequiredTypes(_moduleCatalog);
        }

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Configures the <see cref="IRegionBehaviorFactory"/>.
        /// This will be the list of default behaviors that will be added to a region.
        /// </summary>
        protected virtual void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            regionBehaviors?.RegisterDefaultRegionBehaviors();
        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// May be overwritten in a derived class to add specific mappings required by the application.
        /// </summary>
        /// <returns>The <see cref="RegionAdapterMappings"/> instance containing all the mappings.</returns>
        protected virtual void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            regionAdapterMappings?.RegisterDefaultRegionAdapterMappings();
        }

        /// <summary>
        /// Registers the <see cref="Type"/>s of the Exceptions that are not considered
        /// root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected virtual void RegisterFrameworkExceptionTypes()
        {
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        protected abstract AvaloniaObject CreateShell();

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        protected virtual void InitializeShell(AvaloniaObject shell)
        {
            MainWindow = shell;
        }

        /// <summary>
        /// Contains actions that should occur last.
        /// </summary>
        protected virtual void OnInitialized()
        {
            (MainWindow as Window)?.Show();
        }

        /// <summary>
        /// Configures the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

        /// <summary>
        /// Initializes the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            PrismInitializationExtensions.RunModuleManager(Container);
        }
    }
}
