using System.Diagnostics.CodeAnalysis;
using Prism.Behaviors;

namespace Prism.Ioc;

/// <summary>
/// Provides extensions for registering Behaviors to add to pages.
/// </summary>
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
    public static IContainerRegistry RegisterPageBehavior<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TBehavior>(this IContainerRegistry container)
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
    public static IContainerRegistry RegisterPageBehavior<TPage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TBehavior>(this IContainerRegistry container)
        where TPage : Page
        where TBehavior : Behavior =>
        container
            .Register<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) =>
            {
                if (p is TPage)
                    p.Behaviors.Add(c.Resolve<TBehavior>());
            });

    /// <summary>
    /// Registers an <see cref="Action{Page}"/> delegate to execute on a given Page instance. This could apply a <see cref="Behavior"/>
    /// or it could add attached properties such Platform Specifics.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="pageBehaviorFactory">The delegate action to perform on the <see cref="Page"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterPageBehaviorFactory(this IServiceCollection services, Action<Page> pageBehaviorFactory) =>
        services.AddSingleton<IPageBehaviorFactory>(new DelegatePageBehaviorFactory(pageBehaviorFactory));

    /// <summary>
    /// Registers an <see cref="Action{IContainerProvider,Page}"/> delegate to execute on a given Page instance. This could apply a <see cref="Behavior"/>
    /// or it could add attached properties such Platform Specifics.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="pageBehaviorFactory">The delegate action to perform on the <see cref="Page"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterPageBehaviorFactory(this IServiceCollection services, Action<IContainerProvider, Page> pageBehaviorFactory) =>
        services.AddScoped<IPageBehaviorFactory>(c => new DelegateContainerPageBehaviorFactory(pageBehaviorFactory, c.GetRequiredService<IContainerProvider>()));

    /// <summary>
    /// Registers a given <see cref="Behavior"/> for all Pages.
    /// </summary>
    /// <typeparam name="TBehavior">The <see cref="Behavior"/> type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterPageBehavior<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TBehavior>(this IServiceCollection services)
        where TBehavior : Behavior =>
        services
            .AddTransient<TBehavior>()
            .RegisterPageBehaviorFactory((c, p) => p.Behaviors.Add(c.Resolve<TBehavior>()));

    /// <summary>
    /// Registers a given <see cref="Behavior"/> for the specified type of <see cref="Page"/>
    /// </summary>
    /// <typeparam name="TPage">The <see cref="Page"/> type.</typeparam>
    /// <typeparam name="TBehavior">The <see cref="Behavior"/> type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection RegisterPageBehavior<TPage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TBehavior>(this IServiceCollection services)
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
