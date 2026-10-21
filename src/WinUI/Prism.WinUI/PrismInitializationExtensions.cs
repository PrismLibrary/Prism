using Prism.Properties;
using Prism.Dialogs;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Prism.Windows;

namespace Prism;

internal static class PrismInitializationExtensions
{
    internal static void ConfigureViewModelLocator()
    {
        ViewModelCreationException.SetViewNameDelegate(view => view is DependencyObject obj
            ? ViewModelLocator.GetNavigationName(obj)
            : view.GetType().Name);
        ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
        {
            var provider = (view as DependencyObject)?.GetValue(ViewModelLocator.ContainerProviderProperty) as IContainerProvider ??
                throw new InvalidOperationException(NativeResources.ViewModelCreationRequiresWindowContainer);
            _ = provider.Resolve<IWindowContext>().Container;
            return provider.Resolve(type);
        });
    }

    internal static void RegisterRequiredTypes(this IContainerRegistry registry)
    {
        registry.TryRegisterSingleton<IModuleCatalog, ModuleCatalog>();
        registry.TryRegisterSingleton<IModuleInitializer, ModuleInitializer>();
        registry.TryRegisterSingleton<IModuleManager, ModuleManager>();
        registry.TryRegisterSingleton<IEventAggregator, EventAggregator>();
        if (!registry.IsRegistered<IWindowManager>())
            registry.RegisterSingleton<IWindowManager>(container => new WindowManager(container));
        if (!registry.IsRegistered<IWindowContext>())
            registry.RegisterScoped<IWindowContext>(container => container is IScopedProvider ? new WindowContext() :
                throw new InvalidOperationException(NativeResources.WindowServicesCannotResolveFromRootContainer));
        registry.TryRegisterScoped<IDialogService, DialogService>();
        if (!registry.IsRegistered<IRegionManager>())
            registry.RegisterScoped<IRegionManager>(container =>
            {
                _ = container.Resolve<IWindowContext>().Container;
                return new RegionManager(container);
            });
        if (!registry.IsRegistered<IRegionNavigationContentLoader>())
            registry.RegisterScoped<IRegionNavigationContentLoader>(container =>
            {
                _ = container.Resolve<IWindowContext>().Container;
                return new RegionNavigationContentLoader(container);
            });
        registry.TryRegisterSingleton<RegionAdapterMappings>();
        registry.TryRegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
        if (!registry.IsRegistered<IRegionBehaviorFactory>())
            registry.RegisterSingleton<IRegionBehaviorFactory>(container => new NativeRegionBehaviorFactory(container));
        if (!registry.IsRegistered<IRegionNavigationRegistry>())
            registry.RegisterSingleton<IRegionNavigationRegistry>(container => new RegionNavigationRegistry(GetViewRegistrations(container)));
        if (!registry.IsRegistered<IDialogViewRegistry>())
            registry.RegisterSingleton<IDialogViewRegistry>(container => new DialogViewRegistry(GetViewRegistrations(container)));
        registry.TryRegister<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
        registry.TryRegister<IRegionNavigationJournal, RegionNavigationJournal>();
        if (!registry.IsRegistered<IRegionNavigationService>())
            registry.Register<IRegionNavigationService>(container => new RegionNavigationService(container,
                container.Resolve<IRegionNavigationContentLoader>(), container.Resolve<IRegionNavigationJournal>()));
        registry.TryRegister<IDialogWindow, DialogWindow>();
    }

    internal static void RegisterDefaultRegionBehaviors(this IRegionBehaviorFactory behaviors)
    {
        behaviors.AddIfMissing<BindRegionContextToDependencyObjectBehavior>(BindRegionContextToDependencyObjectBehavior.BehaviorKey);
        behaviors.AddIfMissing<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey);
        behaviors.AddIfMissing<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey);
        behaviors.AddIfMissing<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);
        behaviors.AddIfMissing<RegionMemberLifetimeBehavior>(RegionMemberLifetimeBehavior.BehaviorKey);
        behaviors.AddIfMissing<ClearChildViewsRegionBehavior>(ClearChildViewsRegionBehavior.BehaviorKey);
        behaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
        behaviors.AddIfMissing<DestructibleRegionBehavior>(DestructibleRegionBehavior.BehaviorKey);
    }

    internal static void RegisterDefaultRegionAdapterMappings(this RegionAdapterMappings mappings)
    {
        mappings.RegisterMapping<Selector, SelectorRegionAdapter>();
        mappings.RegisterMapping<ItemsControl, ItemsControlRegionAdapter>();
        mappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
        mappings.RegisterMapping<NavigationView, NavigationViewRegionAdapter>();
    }

    private static IEnumerable<ViewRegistration> GetViewRegistrations(IContainerProvider container)
    {
        // Resolve fresh registrations for every enumeration, including modules loaded later.
        foreach (var registration in container.Resolve<IEnumerable<ViewRegistration>>())
            yield return registration;
    }

    internal static void RunModuleManager(IContainerProvider container) => container.Resolve<IModuleManager>().Run();
}
