using Prism.Properties;
using System.Threading;
using Microsoft.UI.Dispatching;
using Prism.Events;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Prism.Common;
using Prism.Mvvm;
using Application = Microsoft.UI.Xaml.Application;

namespace Prism;

/// <summary>Initializes a native WinUI application with Prism services, regions and modules.</summary>
public abstract class PrismApplicationBase : Application
{
    private readonly IContainerExtension _containerExtension;
    private bool _initializationStarted;


    /// <summary>Creates the shared application container.</summary>
    protected PrismApplicationBase()
    {
        _containerExtension = CreateContainerExtension();
        ContainerLocator.SetContainerExtension(_containerExtension);
    }

    /// <summary>Gets the application container.</summary>
    public IContainerProvider Container => _containerExtension;

    /// <summary>Gets the region manager used to navigate the shell.</summary>
    protected IRegionManager RegionManager => field ??= Container.Resolve<IRegionManager>();

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

    /// <summary>Configures view-model creation using the application's container.</summary>
    protected virtual void ConfigureViewModelLocator() => PrismInitializationExtensions.ConfigureViewModelLocator();

    /// <summary>Creates the initial native window on the application's UI thread.</summary>
    /// <returns>An unused window whose content has not been assigned.</returns>
    protected virtual Window CreateWindow() => new Window();

    /// <summary>Configures the initial window before its shell is created.</summary>
    /// <param name="window">The initial window.</param>
    protected virtual void ConfigureWindow(Window window) { }

    /// <summary>Registers services and initializes the application window and shell.</summary>
    protected virtual void Initialize()
    {
        var dispatcher = DispatcherQueue.GetForCurrentThread() ??
            throw new InvalidOperationException(NativeResources.InitializationRequiresDispatcherThread);
        if (SynchronizationContext.Current is null)
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(dispatcher));

        var window = CreateWindow() ?? throw new InvalidOperationException(NativeResources.CreateWindowMustReturnWindow);
        ConfigureWindow(window);
        _containerExtension.RegisterInstance(window);
        RegisterRequiredTypes(_containerExtension);
        RegisterTypes(_containerExtension);
        // Construct these on the startup UI thread, before modules or background work.
        _ = Container.Resolve<IEventAggregator>();
        ConfigureModuleCatalog(Container.Resolve<IModuleCatalog>());
        ConfigureRegionAdapterMappings(Container.Resolve<RegionAdapterMappings>());
        ConfigureDefaultRegionBehaviors(Container.Resolve<IRegionBehaviorFactory>());
        RegisterFrameworkExceptionTypes();

        var shell = CreateShell();
        if (shell is null)
            return;

        MvvmHelpers.AutowireViewModel(shell);
        var initialized = false;
        var closed = false;
        var element = shell as FrameworkElement;

        void OnClosed(object sender, WindowEventArgs args)
        {
            closed = true;
            window.Closed -= OnClosed;
            if (element is not null)
                element.Loaded -= OnLoaded;
            MvvmHelpers.ViewAndViewModelAction<IActiveAware>(shell, aware => aware.IsActive = false);
        }

        void CompleteInitialization()
        {
            if (initialized || closed)
                return;
            initialized = true;
            Navigation.Regions.RegionManager.SetRegionManager(shell, RegionManager);
            Navigation.Regions.RegionManager.UpdateRegions();
            InitializeModules();
            OnInitialized();
            if (closed)
                return;
            if (shell is ILoadableShell loadableShell)
                loadableShell.FinishLoading();
            MvvmHelpers.ViewAndViewModelAction<IActiveAware>(shell, aware => aware.IsActive = true);
        }

        void QueueInitialization()
        {
            if (!dispatcher.TryEnqueue(CompleteInitialization))
                throw new InvalidOperationException(NativeResources.CannotQueueWindowInitialization);
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            element!.Loaded -= OnLoaded;
            QueueInitialization();
        }

        window.Closed += OnClosed;
        if (element is not null && !element.IsLoaded)
            element.Loaded += OnLoaded;
        window.Content = shell;
        window.Activate();
        if (element is null || element.IsLoaded)
            QueueInitialization();
    }

    /// <summary>Creates the shared container used by this application.</summary>
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

    /// <summary>Creates the application shell.</summary>
    /// <returns>The shell to attach to the application window.</returns>
    protected abstract UIElement CreateShell();

    /// <summary>Runs after the shell loads and modules initialize.</summary>
    protected virtual void OnInitialized() { }

    /// <summary>Configures the application's shared module catalog.</summary>
    /// <param name="moduleCatalog">The module catalog.</param>
    protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) { }

    /// <summary>Initializes the application's modules once using the application container.</summary>
    protected virtual void InitializeModules() => PrismInitializationExtensions.RunModuleManager(Container);
}
