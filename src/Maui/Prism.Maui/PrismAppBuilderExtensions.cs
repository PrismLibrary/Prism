using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Microsoft.Extensions.Logging;

namespace Prism;

public static class PrismAppBuilderExtensions
{
    private static bool s_didRegisterModules = false;

    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, IContainerExtension containerExtension, Action<PrismAppBuilder> configurePrism)
    {
        var prismBuilder = new PrismAppBuilder(containerExtension, builder);
        configurePrism(prismBuilder);
        return builder;
    }

    public static PrismAppBuilder OnInitialized(this PrismAppBuilder builder, Action action)
    {
        return builder.OnInitialized(_ => action());
    }

    /// <summary>
    /// Configures the <see cref="IModuleCatalog"/> used by Prism.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="configureCatalog">Delegate to configure the <see cref="IModuleCatalog"/>.</param>
    public static PrismAppBuilder ConfigureModuleCatalog(this PrismAppBuilder builder, Action<IModuleCatalog> configureCatalog)
    {
        if (!s_didRegisterModules)
        {
            var services = builder.MauiBuilder.Services;
            services.AddSingleton<IModuleCatalog, ModuleCatalog>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<IModuleInitializer, ModuleInitializer>();
        }

        s_didRegisterModules = true;
        return builder.OnInitialized(container =>
        {
            var moduleCatalog = container.Resolve<IModuleCatalog>();
            configureCatalog(moduleCatalog);
        });
    }

    public static PrismAppBuilder OnAppStart(this PrismAppBuilder builder, string uri) =>
        builder.OnAppStart(navigation => navigation.NavigateAsync(uri));

    public static PrismAppBuilder OnAppStart(this PrismAppBuilder builder, string uri, Action<Exception> onError) =>
        builder.OnAppStart(async navigation =>
        {
            var result = await navigation.NavigateAsync(uri);
            if (result.Exception is not null)
                onError(result.Exception);
        });

    public static PrismAppBuilder OnAppStart(this PrismAppBuilder builder, Func<IContainerProvider, INavigationService, Task> onAppStarted) =>
        builder.OnAppStart((c, n) => onAppStarted(c, n));

    public static PrismAppBuilder OnAppStart(this PrismAppBuilder builder, Func<INavigationService, Task> onAppStarted) =>
        builder.OnAppStart((_, n) => onAppStarted(n));

    public static PrismAppBuilder ConfigureServices(this PrismAppBuilder builder, Action<IServiceCollection> configureServices)
    {
        configureServices(builder.MauiBuilder.Services);
        return builder;
    }

    public static PrismAppBuilder ConfigureLogging(this PrismAppBuilder builder, Action<ILoggingBuilder> configureLogging)
    {
        configureLogging(builder.MauiBuilder.Logging);
        return builder;
    }
}
