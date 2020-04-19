using System;
using Prism.Regions;
using Prism.Regions.Adapters;
using Prism.Regions.Behaviors;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides registration and configuration helpers for Region Navigation
    /// </summary>
    public static class RegionRegistrationExtensions
    {
        /// <summary>
        /// Registers the default RegionManager
        /// </summary>
        /// <param name="containerRegistry">The current <see cref="IContainerRegistry" /></param>
        /// <returns>The current <see cref="IContainerRegistry" /></returns>
        public static IContainerRegistry RegisterRegionServices(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<RegionAdapterMappings>();
            containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
            containerRegistry.RegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
            containerRegistry.Register<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
            containerRegistry.Register<IRegionNavigationJournal, RegionNavigationJournal>();
            containerRegistry.Register<IRegionNavigationService, RegionNavigationService>();
            return containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
        }

        /// <summary>
        /// Configures the Default Behaviors and Adapters for Region Navigation
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static PrismApplicationBase InitializeRegionConfigurations(this PrismApplicationBase app) =>
            app.ConfigureDefaultRegionBehaviors()
               .ConfigureRegionAdapterMappings();

        /// <summary>
        /// Configures the <see cref="IRegionBehaviorFactory"/>. 
        /// This will be the list of default behaviors that will be added to a region. 
        /// </summary>
        public static PrismApplicationBase ConfigureDefaultRegionBehaviors(this PrismApplicationBase app, Action<IRegionBehaviorFactory> configure = null)
        {
            var regionBehaviors = app.Container.Resolve<IRegionBehaviorFactory>();
            regionBehaviors.AddIfMissing(BindRegionContextToVisualElementBehavior.BehaviorKey, typeof(BindRegionContextToVisualElementBehavior));
            regionBehaviors.AddIfMissing(RegionActiveAwareBehavior.BehaviorKey, typeof(RegionActiveAwareBehavior));
            regionBehaviors.AddIfMissing(SyncRegionContextWithHostBehavior.BehaviorKey, typeof(SyncRegionContextWithHostBehavior));
            regionBehaviors.AddIfMissing(RegionManagerRegistrationBehavior.BehaviorKey, typeof(RegionManagerRegistrationBehavior));
            regionBehaviors.AddIfMissing(RegionMemberLifetimeBehavior.BehaviorKey, typeof(RegionMemberLifetimeBehavior));
            regionBehaviors.AddIfMissing(ClearChildViewsRegionBehavior.BehaviorKey, typeof(ClearChildViewsRegionBehavior));
            regionBehaviors.AddIfMissing(AutoPopulateRegionBehavior.BehaviorKey, typeof(AutoPopulateRegionBehavior));
            regionBehaviors.AddIfMissing(DestructibleRegionBehavior.BehaviorKey, typeof(DestructibleRegionBehavior));

            configure?.Invoke(regionBehaviors);
            return app;
        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// May be overwritten in a derived class to add specific mappings required by the application.
        /// </summary>
        /// <returns>The <see cref="RegionAdapterMappings"/> instance containing all the mappings.</returns>
        public static PrismApplicationBase ConfigureRegionAdapterMappings(this PrismApplicationBase app, Action<RegionAdapterMappings> configure = null)
        {
            var container = app.Container;
            var regionAdapterMappings = container.Resolve<RegionAdapterMappings>();
            regionAdapterMappings.RegisterMapping(typeof(CarouselView), container.Resolve<CarouselViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(CollectionView), container.Resolve<CollectionViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(FlexLayout), container.Resolve<LayoutViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(StackLayout), container.Resolve<LayoutViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(ScrollView), container.Resolve<ScrollViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(ContentView), container.Resolve<ContentViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(Frame), container.Resolve<ContentViewRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(RefreshView), container.Resolve<ContentViewRegionAdapter>());

            configure?.Invoke(regionAdapterMappings);
            return app;
        }
    }
}
