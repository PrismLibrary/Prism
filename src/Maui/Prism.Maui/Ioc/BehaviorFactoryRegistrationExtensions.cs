using Prism.Behaviors;

namespace Prism.Ioc;

public static class BehaviorFactoryRegistrationExtensions
{
    /// <summary>
    /// Registers a provided action to apply a behavior or other logic to configure your <see cref="Page"/>
    /// </summary>
    /// <param name="container"></param>
    /// <param name="pageBehaviorFactory"></param>
    /// <returns>The <see cref="IContainerRegistry"/>.</returns>
    public static IContainerRegistry RegisterPageBehaviorFactory(this IContainerRegistry container, Action<Page> pageBehaviorFactory) => 
        container.RegisterInstance<IPageBehaviorFactory>(new DelegatePageBehaviorFactory(pageBehaviorFactory));

    /// <summary>
    /// Registers a provided action to apply a behavior or other logic to configure your <see cref="Page"/>
    /// </summary>
    /// <param name="container"></param>
    /// <param name="pageBehaviorFactory"></param>
    /// <returns>The <see cref="IContainerRegistry"/>.</returns>
    public static IContainerRegistry RegisterPageBehaviorFactory(this IContainerRegistry container, Action<IContainerProvider, Page> pageBehaviorFactory) =>
        container.RegisterScoped<IPageBehaviorFactory>(c => new DelegateContainerPageBehaviorFactory(pageBehaviorFactory, c));

    /// <summary>
    /// Adds a specified <typeparamref name="TBehavior"/> to all <see cref="Page"/>'s that are created.
    /// </summary>
    /// <typeparam name="TBehavior"></typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static IContainerRegistry RegisterPageBehavior<TBehavior>(this IContainerRegistry container)
        where TBehavior : Behavior =>
        container
            .Register<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) => p.Behaviors.Add(c.Resolve<TBehavior>()));

    /// <summary>
    /// This will apply the <typeparamref name="TBehavior"/> to the <see cref="Page"/>, when it is a <typeparamref name="TPage"/>.
    /// </summary>
    /// <typeparam name="TPage">The type of Page</typeparam>
    /// <typeparam name="TBehavior">The type of Behavior</typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static IContainerRegistry RegisterPageBehavior<TPage, TBehavior>(this IContainerRegistry container)
        where TPage : Page
        where TBehavior : Behavior =>
        container
            .Register<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) =>
            {
                if (p is TPage)
                    p.Behaviors.Add(c.Resolve<TBehavior>());
            });

    public static IServiceCollection RegisterPageBehaviorFactory(this IServiceCollection services, Action<Page> pageBehaviorFactory) =>
        services.AddSingleton<IPageBehaviorFactory>(new DelegatePageBehaviorFactory(pageBehaviorFactory));

    public static IServiceCollection RegisterPageBehaviorFactory(this IServiceCollection services, Action<IContainerProvider, Page> pageBehaviorFactory) =>
        services.AddScoped<IPageBehaviorFactory>(c => new DelegateContainerPageBehaviorFactory(pageBehaviorFactory, c.GetRequiredService<IContainerProvider>()));

    public static IServiceCollection RegisterPageBehavior<TBehavior>(this IServiceCollection services)
        where TBehavior : Behavior =>
        services
            .AddTransient<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) => p.Behaviors.Add(c.Resolve<TBehavior>()));

    public static IServiceCollection RegisterPageBehavior<TPage, TBehavior>(this IServiceCollection services)
        where TPage : Page
        where TBehavior : Behavior =>
        services
            .AddTransient<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) =>
            {
                if (p is TPage)
                    p.Behaviors.Add(c.Resolve<TBehavior>());
            });
}
