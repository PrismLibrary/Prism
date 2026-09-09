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
                var container = (view as DependencyObject)?.GetValue(ViewModelLocator.ContainerProviderProperty)
                    as IContainerProvider ?? ContainerLocator.Container;
                return container.Resolve(type);
            });
        }

#if UNO_WINUI
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
            containerRegistry.TryRegisterSingleton<IDialogService, DialogService>();
            containerRegistry.TryRegisterSingleton<IModuleInitializer, ModuleInitializer>();
            containerRegistry.TryRegisterSingleton<IModuleManager, ModuleManager>();
            containerRegistry.TryRegisterSingleton<RegionAdapterMappings>();
            containerRegistry.TryRegisterSingleton<IRegionManager, RegionManager>();
            containerRegistry.TryRegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.TryRegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.TryRegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
            containerRegistry.TryRegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
            containerRegistry.TryRegister<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
            containerRegistry.TryRegister<IRegionNavigationJournal, RegionNavigationJournal>();
            containerRegistry.TryRegister<IRegionNavigationService, RegionNavigationService>();
            containerRegistry.TryRegister<IDialogWindow, DialogWindow>(); //default dialog host
        }

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
#if UNO_WINUI
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
