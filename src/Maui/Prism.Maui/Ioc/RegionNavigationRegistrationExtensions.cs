using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls.Compatibility;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Adapters;
using Prism.Navigation.Regions.Behaviors;
using Prism.Navigation.Regions.Navigation;

namespace Prism.Ioc;

/// <summary>
/// Provides common extensions for Service Registrations for Prism.Maui
/// </summary>
public static class RegionNavigationRegistrationExtensions
{
    /// <summary>
    /// Registers a <see cref="View"/> for region navigation.
    /// </summary>
    /// <typeparam name="TView">The Type of <see cref="View"/> to register</typeparam>
    /// <param name="containerRegistry"><see cref="IContainerRegistry"/> used to register type for Navigation.</param>
    /// <param name="name">The unique name to register with the View</param>
    public static IContainerRegistry RegisterForRegionNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IContainerRegistry containerRegistry, string name = null)
        where TView : View =>
        containerRegistry.RegisterForNavigationWithViewModel(typeof(TView), null, name);

    /// <summary>
    /// Registers a <see cref="View"/> for region navigation.
    /// </summary>
    /// <typeparam name="TView">The Type of <see cref="View" />to register</typeparam>
    /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the View</typeparam>
    /// <param name="name">The unique name to register with the View</param>
    /// <param name="containerRegistry"></param>
    public static IContainerRegistry RegisterForRegionNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IContainerRegistry containerRegistry, string name = null)
        where TView : View
        where TViewModel : class =>
        containerRegistry.RegisterForNavigationWithViewModel(typeof(TView), typeof(TViewModel), name);

    public static IContainerRegistry RegisterForRegionNavigation(this IContainerRegistry containerRegistry, Type viewType, Type viewModelType, string name = null)
        => containerRegistry.RegisterForNavigationWithViewModel(viewType, viewModelType, name);

    private static IContainerRegistry RegisterForNavigationWithViewModel(this IContainerRegistry containerRegistry, Type viewType, Type viewModelType, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = viewType.Name;

        if (viewModelType is not null)
            containerRegistry.Register(viewModelType);

        containerRegistry.Register(viewType)
            .RegisterInstance(new ViewRegistration
            {
                Name = name,
                Type = ViewType.Region,
                View = viewType,
                ViewModel = viewModelType
            });

        return containerRegistry;
    }

    /// <summary>
    /// Registers a <see cref="View"/> for region navigation.
    /// </summary>
    /// <typeparam name="TView">The Type of <see cref="View"/> to register</typeparam>
    /// <param name="services"><see cref="IServiceCollection"/> used to register type for Navigation.</param>
    /// <param name="name">The unique name to register with the View</param>
    public static IServiceCollection RegisterForRegionNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IServiceCollection services, string name = null)
        where TView : View =>
        services.RegisterForNavigationWithViewModel(typeof(TView), null, name);

    /// <summary>
    /// Registers a <see cref="View"/> for region navigation.
    /// </summary>
    /// <typeparam name="TView">The Type of <see cref="View" />to register</typeparam>
    /// <typeparam name="TViewModel">The ViewModel to use as the BindingContext for the View</typeparam>
    /// <param name="name">The unique name to register with the View</param>
    /// <param name="services"></param>
    public static IServiceCollection RegisterForRegionNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IServiceCollection services, string name = null)
        where TView : View
        where TViewModel : class =>
        services.RegisterForNavigationWithViewModel(typeof(TView), typeof(TViewModel), name);

    public static IServiceCollection RegisterForRegionNavigation(this IServiceCollection services, Type viewType, Type viewModelType, string name = null)
        => services.RegisterForNavigationWithViewModel(viewType, viewModelType, name);

    private static IServiceCollection RegisterForNavigationWithViewModel(this IServiceCollection services, Type viewType, Type viewModelType, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = viewType.Name;

        if (viewModelType is not null)
            services.AddTransient(viewModelType);

        services.AddTransient(viewType)
            .AddSingleton(new ViewRegistration
            {
                Name = name,
                Type = ViewType.Region,
                View = viewType,
                ViewModel = viewModelType
            });

        return services;
    }

    internal static IContainerRegistry RegisterRegionServices(this IContainerRegistry containerRegistry, Action<RegionAdapterMappings> configureAdapters = null, Action<IRegionBehaviorFactory> configureBehaviors = null)
    {
        containerRegistry.TryRegister<IRegionNavigationRegistry, RegionNavigationRegistry>();
        containerRegistry.RegisterSingleton<RegionAdapterMappings>(p =>
        {
            var regionAdapterMappings = new RegionAdapterMappings();
            configureAdapters?.Invoke(regionAdapterMappings);

            regionAdapterMappings.RegisterDefaultMapping<CarouselView, CarouselViewRegionAdapter>();
            // TODO: CollectionView is buggy with only last View showing despite multiple Active Views
            // BUG: iOS Crash with CollectionView https://github.com/xamarin/Xamarin.Forms/issues/9970
            //regionAdapterMappings.RegisterDefaultMapping<CollectionView, CollectionViewRegionAdapter>();
            regionAdapterMappings.RegisterDefaultMapping<Layout<View>, LayoutViewRegionAdapter>();
            regionAdapterMappings.RegisterDefaultMapping<ScrollView, ScrollViewRegionAdapter>();
            regionAdapterMappings.RegisterDefaultMapping<ContentView, ContentViewRegionAdapter>();
            return regionAdapterMappings;
        });

        containerRegistry.TryRegisterSingleton<IRegionManager, RegionManager>();
        containerRegistry.TryRegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
        containerRegistry.TryRegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
        containerRegistry.TryRegister<RegionBehaviorFactory>();
        containerRegistry.RegisterSingleton<IRegionBehaviorFactory>(p =>
        {
            var regionBehaviors = p.Resolve<RegionBehaviorFactory>();
            regionBehaviors.AddIfMissing<BindRegionContextToVisualElementBehavior>(BindRegionContextToVisualElementBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<RegionMemberLifetimeBehavior>(RegionMemberLifetimeBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<ClearChildViewsRegionBehavior>(ClearChildViewsRegionBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
            regionBehaviors.AddIfMissing<DestructibleRegionBehavior>(DestructibleRegionBehavior.BehaviorKey);
            configureBehaviors?.Invoke(regionBehaviors);
            return regionBehaviors;
        });
        containerRegistry.TryRegister<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
        containerRegistry.TryRegister<IRegionNavigationJournal, RegionNavigationJournal>();
        containerRegistry.TryRegister<IRegionNavigationService, RegionNavigationService>();
        //containerRegistry.RegisterManySingleton<RegionResolverOverrides>(typeof(IResolverOverridesHelper), typeof(IActiveRegionHelper));
        return containerRegistry.TryRegisterSingleton<IRegionManager, RegionManager>();
    }
}
