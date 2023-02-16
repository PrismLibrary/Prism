using Prism.AppModel;
using Prism.Behaviors;
using Prism.Common;
using Prism.Controls;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Prism.Regions.Adapters;
using Prism.Regions.Behaviors;
using Prism.Services;
using Microsoft.Maui.LifecycleEvents;
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
        if (containerExtension is null)
            throw new ArgumentNullException(nameof(containerExtension));

        _container = containerExtension;
        _registrations = new List<Action<IContainerRegistry>>();
        _initializations = new List<Action<IContainerProvider>>();

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
                        return true;

                    var app = root.Resolve<IApplication>();
                    var windows = app.Windows.OfType<PrismWindow>();
                    if (!windows.Any(x => x.IsActive))
                        return true;

                    var window = windows.First(x => x.IsActive);
                    var currentPage = window.CurrentPage;
                    var container = currentPage.GetContainerProvider();
                    if(currentPage is IDialogContainer dialogContainer)
                    {
                        if (dialogContainer.Dismiss.CanExecute(null))
                            dialogContainer.Dismiss.Execute(null);
                    }
                    else
                    {
                        var navigation = container.Resolve<INavigationService>();
                        navigation.GoBackAsync();
                    }

                    return false;
                });
            });
#endif
        });

        ContainerLocator.ResetContainer();
        ContainerLocator.SetContainerExtension(() => containerExtension);

        containerExtension.RegisterInstance(this);
        containerExtension.RegisterSingleton<IMauiInitializeService, PrismInitializationService>();

        ConfigureViewModelLocator();
    }

    public MauiAppBuilder MauiBuilder { get; }

    private void ConfigureViewModelLocator()
    {
        ViewModelLocationProvider2.SetDefaultViewToViewModelTypeResolver(view =>
        {
            if (view is not BindableObject bindable)
                return null;

            return bindable.GetValue(ViewModelLocator.ViewModelProperty) as Type;
        });

        ViewModelLocationProvider2.SetDefaultViewModelFactory(DefaultViewModelLocator);
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

    public PrismAppBuilder RegisterTypes(Action<IContainerRegistry> registerTypes)
    {
        _registrations.Add(registerTypes);
        return this;
    }

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
        ViewModelLocationProvider2.SetDefaultViewModelFactory((view, type) =>
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

    public PrismAppBuilder ConfigureRegionAdapters(Action<RegionAdapterMappings> configureMappings)
    {
        _configureAdapters = configureMappings;
        return this;
    }

    public PrismAppBuilder ConfigureRegionBehaviors(Action<IRegionBehaviorFactory> configureBehaviors)
    {
        _configureBehaviors = configureBehaviors;
        return this;
    }

    private void RegisterDefaultRequiredTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
        containerRegistry.RegisterSingleton<IKeyboardMapper, KeyboardMapper>();
        containerRegistry.RegisterScoped<IPageDialogService, PageDialogService>();
        containerRegistry.RegisterScoped<IDialogService, DialogService>();
        containerRegistry.Register<IDialogViewRegistry, DialogViewRegistry>();
        containerRegistry.RegisterDialogContainer<DialogContainerPage>();
        //containerRegistry.RegisterSingleton<IDeviceService, DeviceService>();
        containerRegistry.RegisterScoped<IPageAccessor, PageAccessor>();
        containerRegistry.RegisterScoped<INavigationService, PageNavigationService>();
        containerRegistry.Register<INavigationRegistry, NavigationRegistry>();
        containerRegistry.Register<IWindowManager>(c =>
        {
            var app = c.Resolve<IApplication>();
            if (app is PrismApplication prismApp)
                return prismApp;

            throw new InvalidOperationException("The registered application does not inherit from PrismApplication.");
        });

        containerRegistry.RegisterPageBehavior<NavigationPage, NavigationPageSystemGoBackBehavior>();
        containerRegistry.RegisterPageBehavior<NavigationPage, NavigationPageActiveAwareBehavior>();
        containerRegistry.RegisterPageBehavior<TabbedPage, TabbedPageActiveAwareBehavior>();
        containerRegistry.RegisterPageBehavior<PageLifeCycleAwareBehavior>();
        containerRegistry.RegisterPageBehavior<PageScopeBehavior>();
        containerRegistry.RegisterPageBehavior<RegionCleanupBehavior>();
        containerRegistry.RegisterRegionServices(_configureAdapters, _configureBehaviors);
    }
}
