using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Controls;
using Prism.Dialogs;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Adapters;
using Prism.Navigation.Xaml;
using Prism.Services;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism;

/// <summary>
/// A builder for Prism with .NET MAUI cross-platform applications and services.
/// </summary>
public sealed class PrismAppBuilder
{
    private readonly List<Action<IContainerRegistry>> _registrations;
    private readonly List<Action<IContainerProvider>> _initializations;
    private readonly IContainerProvider _container;
    private Func<IContainerProvider, INavigationService, Task> _createWindow;
    private Action<RegionAdapterMappings> _configureAdapters;
    private Action<IRegionBehaviorFactory> _configureBehaviors;

    internal PrismAppBuilder(IContainerExtension containerExtension, MauiAppBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(containerExtension);
        ArgumentNullException.ThrowIfNull(builder);

        _container = containerExtension;
        _registrations = [];
        _initializations = [];

        ViewModelCreationException.SetViewNameDelegate(view =>
        {
            if (view is BindableObject bindable)
                return Mvvm.ViewModelLocator.GetNavigationName(bindable);

            return $"View is not a BindableObject: '{view.GetType().FullName}";
        });

        // Ensure that the DialogStack is cleared when the Application is started.
        // This is primarily to help with Unit Tests
        IDialogContainer.DialogStack.Clear();
        MauiBuilder = builder;
        MauiBuilder.ConfigureContainer(new PrismServiceProviderFactory(RegistrationCallback));
        MauiBuilder.ConfigureLifecycleEvents(lifecycle =>
        {
#if ANDROID
            lifecycle.AddAndroid(android =>
            {
                android.OnBackPressed(activity =>
                {
                    var dialogModal = IDialogContainer.DialogStack.LastOrDefault();
                    if (dialogModal is not null)
                    {
                        if (dialogModal.Dismiss.CanExecute(null))
                            dialogModal.Dismiss.Execute(null);

                        return true;
                    }

                    var root = ContainerLocator.Container;
                    if (root is null)
                        return false;

                    var app = root.Resolve<IApplication>();
                    var window = app.Windows.OfType<PrismWindow>()
                        .FirstOrDefault(x => x.IsActive);

                    if (window is null)
                        return false;

                    if (window.CurrentPage?.Parent is NavigationPage)
                    {
                        return true;
                    }

                    if(window.IsRootPage)
                    {
                        return false;
                    }

                    window.OnSystemBack();

                    return true;
                });
            });
#endif
        });

        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(containerExtension);

        containerExtension.RegisterInstance(this);
        containerExtension.RegisterSingleton<IMauiInitializeService, PrismInitializationService>();

        ConfigureViewModelLocator();
    }

    /// <summary>
    /// Gets the associated <see cref="MauiAppBuilder"/>.
    /// </summary>
    public MauiAppBuilder MauiBuilder { get; }

    private static void ConfigureViewModelLocator()
    {
        ViewModelLocationProvider.SetDefaultViewToViewModelTypeResolver(view =>
        {
            if (view is not BindableObject bindable)
                return null;

            return bindable.GetValue(ViewModelLocator.ViewModelProperty) as Type;
        });

        ViewModelLocationProvider.SetDefaultViewModelFactory(DefaultViewModelLocator);
    }

    internal static object DefaultViewModelLocator(object view, Type viewModelType)
    {
        try
        {
            if (view is not BindableObject bindable || bindable.BindingContext is not null)
                return null;

            var container = bindable.GetContainerProvider();

            return container.Resolve(viewModelType, (typeof(IDispatcher), bindable.Dispatcher));
        }
        catch (ViewModelCreationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ViewModelCreationException(view, ex);
        }
    }

    /// <summary>
    /// Provides a Delegate to register services with the <see cref="PrismAppBuilder"/>
    /// </summary>
    /// <param name="registerTypes">The delegate to register your services.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public PrismAppBuilder RegisterTypes(Action<IContainerRegistry> registerTypes)
    {
        _registrations.Add(registerTypes);
        return this;
    }

    /// <summary>
    /// Provides a Delegate to invoke when the App is initialized.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public PrismAppBuilder OnInitialized(Action<IContainerProvider> action)
    {
        _initializations.Add(action);
        return this;
    }

    private bool _initialized;
    internal void OnInitialized()
    {
        if (_initialized)
            return;

        _initialized = true;
        var logger = _container.Resolve<ILogger<PrismAppBuilder>>();
        var errors = new List<Exception>();

        _initializations.ForEach(action =>
        {
            try
            {
                action(_container);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing Initialization Delegate.");
                errors.Add(ex);
            }
        });

        if (errors.Count == 1)
        {
            throw new PrismInitializationException("An error was encountered while invoking the OnInitialized Delegates", errors[0]);
        }
        else if (errors.Count > 1)
        {
            throw new AggregateException("One or more errors were encountered while executing the OnInitialized Delegates", [.. errors]);
        }

        if (_container.IsRegistered<IModuleCatalog>() && _container.Resolve<IModuleCatalog>().Modules.Any())
        {
            try
            {
                var manager = _container.Resolve<IModuleManager>();
                manager.Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error ocurred while initializing the Modules.");
                throw new PrismInitializationException("An error occurred while initializing the Modules.", ex);
            }
        }

        var navRegistry = _container.Resolve<INavigationRegistry>();
        if (!navRegistry.IsRegistered(nameof(NavigationPage)))
        {
            var registry = _container as IContainerRegistry;
            registry
                .Register<PrismNavigationPage>(() => new PrismNavigationPage())
                .RegisterInstance(new ViewRegistration
                {
                    Name = nameof(NavigationPage),
                    View = typeof(PrismNavigationPage),
                    Type = ViewType.Page
                });
        }

        if (!navRegistry.IsRegistered(nameof(TabbedPage)))
        {
            var registry = _container as IContainerRegistry;
            registry.RegisterForNavigation<TabbedPage>();
        }
    }

    internal void OnCreateWindow()
    {
        if (_createWindow is null)
            throw new ArgumentException("You must call CreateWindow on the PrismAppBuilder.");

        // Ensure that this is executed before we navigate.
        OnInitialized();
        var onStart = _createWindow(_container, _container.Resolve<INavigationService>());
        onStart.Wait();
    }

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="createWindow">The Navigation Delegate.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public PrismAppBuilder CreateWindow(Func<IContainerProvider, INavigationService, Task> createWindow)
    {
        _createWindow = createWindow;
        return this;
    }

    /// <summary>
    /// Configures the <see cref="ViewModelLocator"/> used by Prism.
    /// </summary>
    public PrismAppBuilder ConfigureDefaultViewModelFactory(Func<IContainerProvider, object, Type, object> viewModelFactory)
    {
        ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
        {
            if (view is not BindableObject bindable)
                return null;

            var container = bindable.GetContainerProvider();
            return viewModelFactory(container, view, type);
        });

        return this;
    }

    private void RegistrationCallback(IContainerExtension container)
    {
        RegisterDefaultRequiredTypes(container);

        _registrations.ForEach(action => action(container));
    }

    /// <summary>
    /// Configures <see cref="RegionAdapterMappings"/> for Region Navigation with the <see cref="IRegionManager"/>.
    /// </summary>
    /// <param name="configureMappings">Delegate to configure the <see cref="RegionAdapterMappings"/>.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public PrismAppBuilder ConfigureRegionAdapters(Action<RegionAdapterMappings> configureMappings)
    {
        _configureAdapters = configureMappings;
        return this;
    }

    /// <summary>
    /// Configures the <see cref="IRegionBehaviorFactory"/>.
    /// </summary>
    /// <param name="configureBehaviors">Delegate to configure the <see cref="IRegionBehaviorFactory"/>.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public PrismAppBuilder ConfigureRegionBehaviors(Action<IRegionBehaviorFactory> configureBehaviors)
    {
        _configureBehaviors = configureBehaviors;
        return this;
    }

    private void RegisterDefaultRequiredTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.TryRegisterSingleton<IEventAggregator, EventAggregator>();
        containerRegistry.TryRegisterSingleton<IKeyboardMapper, KeyboardMapper>();
        containerRegistry.TryRegisterSingleton<IPageDialogService, PageDialogService>();
        containerRegistry.TryRegisterScoped<IDialogService, DialogService>();
        containerRegistry.TryRegister<IDialogViewRegistry, DialogViewRegistry>();
        containerRegistry.RegisterDialogContainer<DialogContainerPage>();
        //containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
        containerRegistry.TryRegisterScoped<IPageAccessor, PageAccessor>();
        containerRegistry.TryRegisterScoped<INavigationService, PageNavigationService>();
        containerRegistry.TryRegister<INavigationRegistry, NavigationRegistry>();
        containerRegistry.RegisterManySingleton<PrismWindowManager>();
        containerRegistry.RegisterPageBehavior<NavigationPage, NavigationPageSystemGoBackBehavior>();
        containerRegistry.RegisterPageBehavior<NavigationPage, NavigationPageActiveAwareBehavior>();
        containerRegistry.RegisterPageBehavior<NavigationPage, NavigationPageTabbedParentBehavior>();
        containerRegistry.RegisterPageBehavior<TabbedPage, TabbedPageActiveAwareBehavior>();
        containerRegistry.RegisterPageBehavior<PageLifeCycleAwareBehavior>();
        containerRegistry.RegisterPageBehavior<PageScopeBehavior>();
        containerRegistry.RegisterPageBehavior<RegionCleanupBehavior>();
        containerRegistry.RegisterRegionServices(_configureAdapters, _configureBehaviors);
    }
}
