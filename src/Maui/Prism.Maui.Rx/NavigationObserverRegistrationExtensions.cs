namespace Prism.Navigation;

/// <summary>Registers the global navigation observer and configures subscriptions during application initialization.</summary>
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

    /// <summary>Configures a subscription to navigation outcomes when Prism initializes.</summary>
    /// <param name="builder">The Prism application builder.</param>
    /// <param name="addObservable">The callback receiving the navigation outcome stream.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <remarks>Keep and dispose subscription handles when the subscriber's lifetime ends.</remarks>
    public static PrismAppBuilder AddGlobalNavigationObserver(this PrismAppBuilder builder, Action<IObservable<NavigationRequestContext>> addObservable) =>
        builder.RegisterGlobalNavigationObserver()
        .OnInitialized(c =>
        {
            addObservable(c.Resolve<IGlobalNavigationObserver>().NavigationRequest);
        });

    /// <summary>Configures a subscription to navigation outcomes with access to the initialized container.</summary>
    /// <param name="builder">The Prism application builder.</param>
    /// <param name="addObservable">The callback receiving the application container and navigation outcome stream.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <remarks>Keep and dispose subscription handles when the subscriber's lifetime ends.</remarks>
    public static PrismAppBuilder AddGlobalNavigationObserver(this PrismAppBuilder builder, Action<IContainerProvider, IObservable<NavigationRequestContext>> addObservable) =>
        builder.RegisterGlobalNavigationObserver()
        .OnInitialized(c =>
        {
            addObservable(c, c.Resolve<IGlobalNavigationObserver>().NavigationRequest);
        });
}
