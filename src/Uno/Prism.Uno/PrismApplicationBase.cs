using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Prism.Common;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Uno.Extensions.Hosting;
using Application = Microsoft.UI.Xaml.Application;

#nullable enable
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
        private IContainerExtension? _containerExtension;
        private IModuleCatalog? _moduleCatalog;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider Container => _containerExtension ??
            throw new InvalidOperationException("The Container has not yet been initialized.");

        private IHost? _host;
        /// <summary>
        /// Gets the <see cref="IHost" /> built when the Shell is loaded.
        /// </summary>
        public IHost Host => _host ??
            throw new InvalidOperationException("The host has not yet been created. The Shell must first be loaded before the Host is created.");

        private IRegionManager? _regionManager;
        /// <summary>
        /// Gets the <see cref="IRegionManager" /> which can be used in the OnInitialized method to Navigation in the Shell once it has loaded
        /// and the <see cref="IHost" /> has been built and all <see cref="IModule" />'s have been loaded by the <see cref="IModuleManager" />
        /// </summary>
        protected IRegionManager RegionManager => _regionManager ??= Container.Resolve<IRegionManager>();

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var builder = this.CreateBuilder(args);
            base.OnLaunched(args);
            InitializeInternal(builder);
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        void InitializeInternal(IApplicationBuilder builder)
        {
            ConfigureViewModelLocator();
            Initialize(builder);
        }

        /// <summary>
        /// Configures the <see cref="Prism.Mvvm.ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                return Container.Resolve(type);
            });
        }

        /// <summary>
        /// Allows you to configure the Application Host using the <see cref="IApplicationBuilder" />
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" />.</param>
        protected virtual void ConfigureApp(IApplicationBuilder builder) { }

        /// <summary>
        /// Allows you to configure the <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder" />.</param>
        protected virtual void ConfigureHost(IHostBuilder builder) { }

        /// <summary>
        /// Register Services with the <see cref="IServiceCollection" />
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /></param>
        protected virtual void ConfigureServices(IServiceCollection services) { }

        /// <summary>
        /// Runs the initialization sequence to configure the Prism application.
        /// </summary>
        protected virtual void Initialize(IApplicationBuilder builder)
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _containerExtension = ContainerLocator.Current;
            ConfigureApp(builder);
            builder.Configure(ConfigureHost)
                .Configure(x => x.ConfigureServices(ConfigureServices)
                    .UseServiceProviderFactory(new PrismServiceProviderFactory(_containerExtension)));

            _moduleCatalog = CreateModuleCatalog();
            RegisterRequiredTypes(_containerExtension);
            RegisterTypes(_containerExtension);
            _containerExtension.FinalizeExtension();

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
                builder.Window.Content = shell;
                builder.Window.Activate();

                void FinalizeInitialization()
                {
                    Regions.RegionManager.SetRegionManager(shell, _containerExtension.Resolve<IRegionManager>());
                    Regions.RegionManager.UpdateRegions();
                    _host = builder.Build();
                    InitializeModules();
                    OnInitialized();
                }

                if (shell is FrameworkElement fe)
                {
                    void OnLoaded(object sender, object args)
                    {
                        FinalizeInitialization();
                        fe.Loaded -= OnLoaded;
                    }

                    if (fe.IsLoaded)
                    {
                        FinalizeInitialization();
                    }
                    else
                    {
                        // We need to delay the initialization after the shell has been loaded, otherwise
                        // the visual tree is not materialized for the RegionManager to be available.
                        // See https://github.com/PrismLibrary/Prism/issues/2102 for more details.
                        fe.Loaded += OnLoaded;
                    }
                }
                else
                {
                    FinalizeInitialization();
                }
            }
        }

        /// <summary>
        /// Creates the container used by Prism.
        /// </summary>
        /// <returns>The container</returns>
        protected abstract IContainerExtension CreateContainerExtension();

        /// <summary>
        /// Creates the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        ///  <remarks>
        /// The base implementation returns a new ModuleCatalog.
        /// </remarks>
        protected virtual IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog();
        }

        /// <summary>
        /// Registers all types that are required by Prism to function with the container.
        /// </summary>
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
        protected abstract UIElement CreateShell();

        /// <summary>
        /// Contains actions that should occur last.
        /// </summary>
        protected virtual void OnInitialized()
        {
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
