using System.Threading;
using Microsoft.UI.Dispatching;
using Prism.Events;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Prism.Windows;
using Application = Microsoft.UI.Xaml.Application;

namespace Prism;

/// <summary>Initializes a native WinUI application with shared services and independently scoped windows.</summary>
/// <remarks>
/// All managed windows use the original WinUI UI thread. The application container,
/// module catalog, module initialization and event aggregator are shared. Resolve views,
/// dialog services and region managers through a window context, never the root container.
/// </remarks>
public abstract class PrismApplicationBase : Application
{
    private readonly IContainerExtension _containerExtension;
    private bool _initializationStarted;
    private bool _modulesInitialized;

    /// <summary>Creates the shared application container.</summary>
    protected PrismApplicationBase()
    {
        _containerExtension = CreateContainerExtension();
        ContainerLocator.SetContainerExtension(_containerExtension);
    }

    /// <summary>Gets the application container for shared services, not window-owned services.</summary>
    public IContainerProvider Container => _containerExtension;

    /// <summary>Gets the manager used to open or explicitly target independently scoped windows.</summary>
    public IWindowManager Windows => Container.Resolve<IWindowManager>();

    /// <inheritdoc/>
    protected sealed override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        if (_initializationStarted)
            return;
        _initializationStarted = true;
        ConfigureViewModelLocator();
        Initialize();
    }

    /// <summary>Configures view-model creation using each view's window-owned container.</summary>
    protected virtual void ConfigureViewModelLocator() => PrismInitializationExtensions.ConfigureViewModelLocator();

    /// <summary>Creates the initial native window on the application's UI thread.</summary>
    /// <returns>An unused window whose content has not been assigned.</returns>
    protected virtual Window CreateWindow() => new Window();

    /// <summary>Configures the initial window before its shell is created.</summary>
    /// <param name="window">The initial window.</param>
    protected virtual void ConfigureWindow(Window window) { }

    /// <summary>Registers shared services and opens the initial, independently scoped window.</summary>
    protected virtual void Initialize()
    {
        var dispatcher = DispatcherQueue.GetForCurrentThread() ??
            throw new InvalidOperationException("Prism must be initialized on the WinUI dispatcher thread.");
        if (SynchronizationContext.Current is null)
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(dispatcher));

        RegisterRequiredTypes(_containerExtension);
        RegisterTypes(_containerExtension);
        // Construct these on the startup UI thread, before modules or background work.
        _ = Container.Resolve<IEventAggregator>();
        var windows = Windows;
        ConfigureModuleCatalog(Container.Resolve<IModuleCatalog>());
        ConfigureRegionAdapterMappings(Container.Resolve<RegionAdapterMappings>());
        ConfigureDefaultRegionBehaviors(Container.Resolve<IRegionBehaviorFactory>());
        RegisterFrameworkExceptionTypes();

        var window = CreateWindow() ?? throw new InvalidOperationException("CreateWindow must return a window.");
        ConfigureWindow(window);
        windows.OpenWindow(window, CreateShell, context =>
        {
            if (!_modulesInitialized)
            {
                // ModuleInfo state is application-wide: never initialize it once per window.
                _modulesInitialized = true;
                InitializeModules();
            }
            OnInitialized(context);
        });
    }

    /// <summary>Creates the shared container used by this application.</summary>
    /// <returns>The container extension.</returns>
    protected abstract IContainerExtension CreateContainerExtension();

    /// <summary>Registers shared services and window-scoped Prism services.</summary>
    /// <param name="containerRegistry">The application container registry.</param>
    protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterRequiredTypes();

    /// <summary>Registers application services and views.</summary>
    /// <param name="containerRegistry">The application container registry.</param>
    protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

    /// <summary>Configures the default region behaviors.</summary>
    /// <param name="regionBehaviors">The region behavior factory.</param>
    protected virtual void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors) => regionBehaviors.RegisterDefaultRegionBehaviors();

    /// <summary>Configures control-to-region adapter mappings.</summary>
    /// <param name="regionAdapterMappings">The region adapter mappings.</param>
    protected virtual void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings) => regionAdapterMappings.RegisterDefaultRegionAdapterMappings();

    /// <summary>Registers container-specific exception types for exception unwrapping.</summary>
    protected virtual void RegisterFrameworkExceptionTypes() { }

    /// <summary>Creates the initial shell using its window-owned container.</summary>
    /// <param name="container">The initial window's scoped provider. Resolve the shell from this provider.</param>
    /// <returns>The shell to attach to the initial window.</returns>
    protected abstract UIElement CreateShell(IContainerProvider container);

    /// <summary>Runs after the initial shell loads and shared modules initialize, before loading completes.</summary>
    /// <param name="window">The initial window's context for navigation and dialogs.</param>
    protected virtual void OnInitialized(IWindowContext window) { }

    /// <summary>Configures the application's shared module catalog.</summary>
    /// <param name="moduleCatalog">The module catalog.</param>
    protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

    /// <summary>Initializes the application's modules once using the application container.</summary>
    protected virtual void InitializeModules() => PrismInitializationExtensions.RunModuleManager(Container);
}
