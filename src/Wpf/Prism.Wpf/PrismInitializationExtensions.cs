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
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                return ContainerLocator.Container.Resolve(type);
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
            regionBehaviors.AddIfMissing<BindRegionContextToDependencyObjectBehavior>(BindRegionContextToDependencyObjectBehavior.BehaviorKey);
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
            regionAdapterMappings.RegisterMapping<Selector, SelectorRegionAdapter>();
            regionAdapterMappings.RegisterMapping<ItemsControl, ItemsControlRegionAdapter>();
            regionAdapterMappings.RegisterMapping<ContentControl, ContentControlRegionAdapter>();
#if UNO_WINUI
            regionAdapterMappings.RegisterMapping<NavigationView, NavigationViewRegionAdapter>();
#endif
        }

        internal static void RunModuleManager(IContainerProvider containerProvider)
        {
            IModuleManager manager = containerProvider.Resolve<IModuleManager>();
            manager.Run();
        }
    }
}
