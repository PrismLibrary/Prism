#if WINUI && !UNO_WINUI
using Prism.Properties;
using Prism.Windows;
#endif
using Prism.Dialogs;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;

namespace Prism
{
    internal static class PrismInitializationExtensions
    {
        internal static void ConfigureViewModelLocator()
        {
            ViewModelCreationException.SetViewNameDelegate(view => view is DependencyObject obj
                ? ViewModelLocator.GetNavigationName(obj)
                : view.GetType().Name);
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
#if WINUI && !UNO_WINUI
                var container = (view as DependencyObject)?.GetValue(ViewModelLocator.ContainerProviderProperty) as IContainerProvider ??
                    throw new InvalidOperationException(NativeResources.ViewModelCreationRequiresWindowContainer);
                _ = container.Resolve<IWindowContext>().Container;
#else
                var container = (view as DependencyObject)?.GetValue(ViewModelLocator.ContainerProviderProperty)
                    as IContainerProvider ?? ContainerLocator.Container;
#endif
                return container.Resolve(type);
            });
        }

#if UNO_WINUI || WINUI
        internal static void RegisterRequiredTypes(this IContainerRegistry containerRegistry)
        {
            containerRegistry.TryRegisterSingleton<IModuleCatalog, ModuleCatalog>();
#else
        internal static void RegisterRequiredTypes(this IContainerRegistry containerRegistry, IModuleCatalog moduleCatalog)
        {
            containerRegistry.TryRegisterInstance(moduleCatalog);
#endif
            if (!containerRegistry.IsRegistered<IRegionNavigationRegistry>())
                containerRegistry.RegisterSingleton<IRegionNavigationRegistry>(c => new RegionNavigationRegistry(GetViewRegistrations(c)));
            if (!containerRegistry.IsRegistered<IDialogViewRegistry>())
                containerRegistry.RegisterSingleton<IDialogViewRegistry>(c => new DialogViewRegistry(GetViewRegistrations(c)));
#if WINUI && !UNO_WINUI
            RegisterWindowServices(containerRegistry);
#else
            containerRegistry.TryRegisterSingleton<IDialogService, DialogService>();
            containerRegistry.TryRegisterSingleton<IRegionManager, RegionManager>();
            containerRegistry.TryRegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.TryRegister<IRegionNavigationService, RegionNavigationService>();
#endif
            containerRegistry.TryRegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.TryRegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.TryRegisterSingleton<RegionAdapterMappings>();
            containerRegistry.TryRegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.TryRegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
            containerRegistry.TryRegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
            containerRegistry.TryRegister<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
            containerRegistry.TryRegister<IRegionNavigationJournal, RegionNavigationJournal>();
            containerRegistry.TryRegister<IDialogWindow, DialogWindow>(); //default dialog host
        }

#if WINUI && !UNO_WINUI
        private static void RegisterWindowServices(IContainerRegistry registry)
        {
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
            if (!registry.IsRegistered<IRegionNavigationService>())
                registry.Register<IRegionNavigationService>(container => new RegionNavigationService(container,
                    container.Resolve<IRegionNavigationContentLoader>(), container.Resolve<IRegionNavigationJournal>()));
        }
#endif

        internal static void RegisterDefaultRegionBehaviors(this IRegionBehaviorFactory regionBehaviors)
        {
#if AVALONIA
            regionBehaviors.AddIfMissing<BindRegionContextToAvaloniaObjectBehavior>(BindRegionContextToAvaloniaObjectBehavior.BehaviorKey);
#else
            regionBehaviors.AddIfMissing<BindRegionContextToDependencyObjectBehavior>(BindRegionContextToDependencyObjectBehavior.BehaviorKey);
#endif
            regionBehaviors.AddIfMissing<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionMemberLifetimeBehavior>(RegionMemberLifetimeBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<ClearChildViewsRegionBehavior>(ClearChildViewsRegionBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<DestructibleRegionBehavior>(DestructibleRegionBehavior.BehaviorKey);
        }

        internal static void RegisterDefaultRegionAdapterMappings(this RegionAdapterMappings regionAdapterMappings)
        {
#if !AVALONIA
            regionAdapterMappings.RegisterMapping<Selector, SelectorRegionAdapter>();
#endif
            regionAdapterMappings.RegisterMapping<ItemsControl, ItemsControlRegionAdapter>();
            regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
#if UNO_WINUI || WINUI
            regionAdapterMappings.RegisterMapping<NavigationView, NavigationViewRegionAdapter>();
#endif
        }

        // Enumerate on each lookup so registries resolved before modules load see their registrations.
        private static IEnumerable<ViewRegistration> GetViewRegistrations(IContainerProvider container)
        {
            foreach (var registration in container.Resolve<IEnumerable<ViewRegistration>>())
                yield return registration;
        }

        internal static void RunModuleManager(IContainerProvider containerProvider)
        {
            IModuleManager manager = containerProvider.Resolve<IModuleManager>();
            manager.Run();
        }
    }
}
