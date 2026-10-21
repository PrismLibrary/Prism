using System.Threading;
using Microsoft.UI.Dispatching;
using Prism.Common;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Application = Microsoft.UI.Xaml.Application;

#nullable enable
namespace Prism;

/// <summary>Initializes a native WinUI application without an external hosting framework.</summary>
public abstract class PrismApplicationBase : Application
{
    private readonly IContainerExtension _containerExtension;
    private Window? _mainWindow;
    private bool _initializationStarted;
    private bool _initializationCompleted;

    /// <summary>Creates the application container and publishes it through the container locator.</summary>
    protected PrismApplicationBase()
    {
        _containerExtension = CreateContainerExtension();
        ContainerLocator.SetContainerExtension(_containerExtension);
    }

    /// <summary>Gets the application container.</summary>
    public IContainerProvider Container => _containerExtension;

    /// <summary>Gets the primary window after native startup has created it.</summary>
    public Window MainWindow => _mainWindow ??
        throw new InvalidOperationException("The main window is not available before application initialization.");

    /// <summary>Gets the region manager used for shell navigation.</summary>
    protected IRegionManager RegionManager => Container.Resolve<IRegionManager>();

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

    /// <summary>Configures view-model creation using the application or view's container.</summary>
    protected virtual void ConfigureViewModelLocator() => PrismInitializationExtensions.ConfigureViewModelLocator();

    /// <summary>Creates the primary native window.</summary>
    /// <returns>The window that owns the shell and default dialog service.</returns>
    protected virtual Window CreateWindow() => new Window();

    /// <summary>Configures the primary window before its shell is created.</summary>
    /// <param name="window">The primary window.</param>
    protected virtual void ConfigureWindow(Window window) { }

    /// <summary>Registers services, prepares the shell, and schedules initialization after it loads.</summary>
    protected virtual void Initialize()
    {
        var dispatcher = DispatcherQueue.GetForCurrentThread() ??
            throw new InvalidOperationException("Prism must be initialized on the WinUI dispatcher thread.");
        // EventAggregator captures this context at construction. Resolve it here, before
        // modules or background work can create the application's singleton first.
        if (SynchronizationContext.Current is null)
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(dispatcher));

        _mainWindow = CreateWindow() ?? throw new InvalidOperationException("CreateWindow must return a window.");
        _containerExtension.RegisterInstance(_mainWindow);
        ConfigureWindow(_mainWindow);
        RegisterRequiredTypes(_containerExtension);
        RegisterTypes(_containerExtension);
        _ = Container.Resolve<IEventAggregator>();

        ConfigureModuleCatalog(Container.Resolve<IModuleCatalog>());
        ConfigureRegionAdapterMappings(Container.Resolve<RegionAdapterMappings>());
        ConfigureDefaultRegionBehaviors(Container.Resolve<IRegionBehaviorFactory>());
        RegisterFrameworkExceptionTypes();

        var shell = CreateShell() ?? throw new InvalidOperationException("CreateShell must return a shell.");
        MvvmHelpers.AutowireViewModel(shell);
        Navigation.Regions.RegionManager.SetRegionManager(shell, RegionManager);

        void CompleteInitialization()
        {
            if (_initializationCompleted)
                return;

            _initializationCompleted = true;
            Navigation.Regions.RegionManager.UpdateRegions();
            InitializeModules();
            OnInitialized();
            if (shell is ILoadableShell loadableShell)
                loadableShell.FinishLoading();
            MvvmHelpers.ViewAndViewModelAction<IActiveAware>(shell, x => x.IsActive = true);
        }

        void QueueInitialization()
        {
            // Let WinUI finish the Loaded dispatch before modules navigate the visual tree.
            if (!dispatcher.TryEnqueue(CompleteInitialization))
                throw new InvalidOperationException("Unable to queue initialization on the WinUI dispatcher.");
        }

        if (shell is FrameworkElement element)
        {
            void OnLoaded(object sender, RoutedEventArgs args)
            {
                element.Loaded -= OnLoaded;
                QueueInitialization();
            }

            if (element.IsLoaded)
                QueueInitialization();
            else
                element.Loaded += OnLoaded;
        }
        else
        {
            QueueInitialization();
        }

        _mainWindow.Content = shell;
        _mainWindow.Activate();
    }

    /// <summary>Creates the container used by this application.</summary>
    /// <returns>The container extension.</returns>
    protected abstract IContainerExtension CreateContainerExtension();

    /// <summary>Registers the services required by Prism.</summary>
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

    /// <summary>Creates the visual content of the primary window.</summary>
    /// <returns>The shell to attach to the primary window.</returns>
    protected abstract UIElement CreateShell();

    /// <summary>Runs after the shell loads and modules initialize, before loading completes.</summary>
    protected virtual void OnInitialized() { }

    /// <summary>Configures the application's module catalog.</summary>
    /// <param name="moduleCatalog">The module catalog.</param>
    protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

    /// <summary>Initializes the application's modules.</summary>
    protected virtual void InitializeModules() => PrismInitializationExtensions.RunModuleManager(Container);
}
