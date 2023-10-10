using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Prism.Dialogs;

#if HAS_WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#endif

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

#if HAS_WINUI
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
        }

        internal static void RunModuleManager(IContainerProvider containerProvider)
        {
            IModuleManager manager = containerProvider.Resolve<IModuleManager>();
            manager.Run();
        }
    }
}
