using DryIoc;
using Prism;

namespace Microsoft.Maui;

/// <summary>
/// Configures the application to use Prism with the DryIoc container.
/// </summary>
public static class PrismAppExtensions
{
    /// <summary>Adds Prism to the application using DryIoc's default Prism configuration.</summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configurePrism">The callback registering services, views, modules, and startup navigation.</param>
    /// <returns>The same application builder for further configuration.</returns>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, Action<PrismAppBuilder> configurePrism)
    {
        return builder.UsePrism(new DryIocContainerExtension(), configurePrism);
    }

    /// <summary>Adds Prism to the application using customized DryIoc rules.</summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="rules">The starting container rules.</param>
    /// <param name="configurePrism">The callback registering services, views, modules, and startup navigation.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <remarks>The rules are extended to track disposable transients, select a constructor with resolvable arguments, and prefer the last registered factory.</remarks>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, Rules rules, Action<PrismAppBuilder> configurePrism)
    {
        rules = rules.WithTrackingDisposableTransients()
            .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
            .WithFactorySelector(Rules.SelectLastRegisteredFactory());
        return builder.UsePrism(new DryIocContainerExtension(rules), configurePrism);
    }
}
