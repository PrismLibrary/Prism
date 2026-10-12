using DryIoc;
using Prism;

namespace Microsoft.Maui;

/// <summary>
/// Configures the application to use Prism with the DryIoc container.
/// </summary>
public static class PrismAppExtensions
{
    /// <summary>
    /// Adds Prism to the application using DryIoc's default Prism configuration.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configurePrism">The callback registering services, views, modules, and startup navigation.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> or <paramref name="configurePrism"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// Configure Prism before calling <see cref="MauiAppBuilder.Build"/>. The first call selects
    /// the application container. Later calls reuse the same Prism builder and container without
    /// constructing another container. Configuration callbacks execute in call order.
    /// </remarks>
    /// <example>
    /// This example assumes that the application's pages are registered for navigation.
    /// <code>
    /// builder.ConfigurePrism(prism => prism
    ///     .OnInitialized(() => Console.WriteLine("Library initialized")));
    /// builder.UsePrism(prism => prism
    ///     .CreateWindow("NavigationPage/MainPage"));
    /// </code>
    /// </example>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, Action<PrismAppBuilder> configurePrism)
    {
        return builder.UsePrism(() => new DryIocContainerExtension(), configurePrism);
    }

    /// <summary>
    /// Adds Prism to the application using customized DryIoc rules.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="rules">The starting rules used when creating the first application container.</param>
    /// <param name="configurePrism">The callback registering services, views, modules, and startup navigation.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/>, <paramref name="rules"/>, or <paramref name="configurePrism"/>
    /// is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// The rules are extended to track disposable transients, select a constructor with resolvable
    /// arguments, and prefer the last registered factory. If Prism is already configured for this
    /// builder, the selected container is reused and these rules do not replace its configuration.
    /// No additional container is constructed. Complete configuration before calling
    /// <see cref="MauiAppBuilder.Build"/>.
    /// </remarks>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, Rules rules, Action<PrismAppBuilder> configurePrism)
    {
        ArgumentNullException.ThrowIfNull(rules);
        return builder.UsePrism(() =>
        {
            var containerRules = rules.WithTrackingDisposableTransients()
                .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                .WithFactorySelector(Rules.SelectLastRegisteredFactory());
            return new DryIocContainerExtension(containerRules);
        }, configurePrism);
    }
}
