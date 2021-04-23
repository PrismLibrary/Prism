using System;
using System.Linq;
using Prism.Common;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Services.Dialogs;
using Windows.ApplicationModel;

#if HAS_UWP
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#elif HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endif

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
        IContainerExtension _containerExtension;
        IModuleCatalog _moduleCatalog;

        public PrismApplicationBase()
        {
#if HAS_UWP
            Suspending += (s, e) => OnSuspending(e);
#endif
        }

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider Container => _containerExtension;

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            InitializeInternal();
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        void InitializeInternal()
        {
            ConfigureViewModelLocator();
            Initialize();
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
        /// Runs the initialization sequence to configure the Prism application.
        /// </summary>
        public virtual void Initialize()
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _containerExtension = ContainerLocator.Current;
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
                InitializeShell(shell);

                void FinalizeInitialization()
                {
                    RegionManager.SetRegionManager(shell, _containerExtension.Resolve<IRegionManager>());
                    RegionManager.UpdateRegions();

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

#if HAS_UNO
                    // Uno currently loads items earlier than UWP, so we can check 
                    // for the IsLoaded property. UWP got that property in SDK 17763,
                    // meaning that the condition can be removed once the SDK is updated
                    // in Prism.Uno.
                    if (fe.IsLoaded)
                    {
                        FinalizeInitialization();
                    }
                    else
#endif
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
        /// Initializes the shell.
        /// </summary>
        protected virtual void InitializeShell(UIElement shell)
        {
#if HAS_UWP
            Windows.UI.Xaml.Window.Current.Content = shell;

            // Activate must be called immediately in order for the Loaded event to be raised
            // in the shell.
            Windows.UI.Xaml.Window.Current.Activate();
#elif HAS_WINUI && !NETCOREAPP
            Microsoft.UI.Xaml.Window.Current.Content = shell;

            // Activate must be called immediately in order for the Loaded event to be raised
            // in the shell.
            Microsoft.UI.Xaml.Window.Current.Activate();
#endif
        }

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

#if HAS_UWP
        protected virtual void OnSuspending(SuspendingEventArgs e)
        {
        }
#endif
    }
}
