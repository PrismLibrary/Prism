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

public sealed class PrismAppBuilder
{
    private List<Action<IContainerRegistry>> _registrations { get; }
    private List<Action<IContainerProvider>> _initializations { get; }
    private IContainerProvider _container { get; }
    private Func<IContainerProvider, INavigationService, Task> _onAppStarted;
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

        MauiBuilder = builder;
        MauiBuilder.ConfigureContainer(new PrismServiceProviderFactory(RegistrationCallback));
        MauiBuilder.ConfigureLifecycleEvents(lifecycle =>
        {
#if ANDROID
            lifecycle.AddAndroid(android =>
            {
                android.OnBackPressed(activity =>
                {
                    var root = ContainerLocator.Container;
                    if (root is null)
                        return false;

                    var app = root.Resolve<IApplication>();
                    var windows = app.Windows.OfType<PrismWindow>();
                    if (!windows.Any(x => x.IsActive))
                        return false;

                    var window = windows.First(x => x.IsActive);
                    if(window.IsRootPage && app is Application application)
                    {
                        application.Quit();
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
            if (view is not BindableObject bindable)
                return null;

            var container = bindable.GetContainerProvider();

            return container.Resolve(viewModelType);
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
        _initializations.ForEach(action => action(_container));

        if (_container.IsRegistered<IModuleCatalog>() && _container.Resolve<IModuleCatalog>().Modules.Any())
        {
            var manager = _container.Resolve<IModuleManager>();
            manager.Run();
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

    internal void OnAppStarted()
    {
        if (_onAppStarted is null)
            throw new ArgumentException("You must call OnAppStart on the PrismAppBuilder.");

        // Ensure that this is executed before we navigate.
        OnInitialized();
        var onStart = _onAppStarted(_container, _container.Resolve<INavigationService>());
        onStart.Wait();
    }

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="onAppStarted">The Navigation Delegate.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public PrismAppBuilder OnAppStart(Func<IContainerProvider, INavigationService, Task> onAppStarted)
    {
        _onAppStarted = onAppStarted;
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
        containerRegistry.TryRegisterScoped<IPageDialogService, PageDialogService>();
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
