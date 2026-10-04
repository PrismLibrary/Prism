namespace Prism.Navigation;

public static class NavigationObserverRegistrationExtensions
{
    private static PrismAppBuilder RegisterGlobalNavigationObserver(this PrismAppBuilder builder)
    {
        return builder.RegisterTypes(c => 
        {
            if (!c.IsRegistered<IGlobalNavigationObserver>())
                c.RegisterSingleton<IGlobalNavigationObserver, GlobalNavigationObserver>();
        });
    }

    /// <summary>Observes tab selection changes after a tabbed page is displayed.</summary>
    public static PrismAppBuilder AddGlobalTabChangedObserver(this PrismAppBuilder builder, Action<IObservable<TabChangedContext>> addObservable) =>
        builder.RegisterGlobalNavigationObserver()
        .OnInitialized(c => addObservable(c.Resolve<IGlobalNavigationObserver>().TabChanged));

    public static PrismAppBuilder AddGlobalNavigationObserver(this PrismAppBuilder builder, Action<IObservable<NavigationRequestContext>> addObservable) =>
        builder.RegisterGlobalNavigationObserver()
        .OnInitialized(c =>
        {
            addObservable(c.Resolve<IGlobalNavigationObserver>().NavigationRequest);
        });

    public static PrismAppBuilder AddGlobalNavigationObserver(this PrismAppBuilder builder, Action<IContainerProvider, IObservable<NavigationRequestContext>> addObservable) =>
        builder.RegisterGlobalNavigationObserver()
        .OnInitialized(c =>
        {
            addObservable(c, c.Resolve<IGlobalNavigationObserver>().NavigationRequest);
        });
}
