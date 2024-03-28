namespace Prism.Navigation;

public static class NavigationObserverRegistrationExtensions
{
    private static bool s_IsRegistered;

    private static PrismAppBuilder RegisterGlobalNavigationObserver(this PrismAppBuilder builder)
    {
        if (s_IsRegistered)
            return builder;

        s_IsRegistered = true;
        return builder.RegisterTypes(c => 
            c.RegisterSingleton<IGlobalNavigationObserver, GlobalNavigationObserver>());
    }

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
