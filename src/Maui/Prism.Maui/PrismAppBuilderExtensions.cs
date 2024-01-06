using Microsoft.Extensions.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Builder;

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

    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, string uri) =>
        builder.CreateWindow(navigation => navigation.NavigateAsync(uri));

    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, string uri, Action<Exception> onError) =>
        builder.CreateWindow(async navigation =>
        {
            var result = await navigation.NavigateAsync(uri);
            if (result.Exception is not null)
                onError(result.Exception);
        });

    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<IContainerProvider, INavigationService, Task> CreateWindowed) =>
        builder.CreateWindow((c, n) => CreateWindowed(c, n));

    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<INavigationService, Task> CreateWindowed) =>
        builder.CreateWindow((_, n) => CreateWindowed(n));

    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<INavigationService, INavigationBuilder> CreateWindowed) =>
        builder.CreateWindow(n => CreateWindowed(n).NavigateAsync());

    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<IContainerProvider, INavigationService, INavigationBuilder> CreateWindowed) =>
        builder.CreateWindow((c, n) => CreateWindowed(c, n).NavigateAsync());

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

    public static PrismAppBuilder ConfigureViewTypeToViewModelTypeResolver(this PrismAppBuilder builder, Func<Type, Type> viewModelTypeResolver)
    {
        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewModelTypeResolver);
        return builder;
    }

    /// <summary>
    /// Registers an <see cref="AppAction"/> with a callback that will be invoked on the UI thread for the specified <see cref="AppAction"/>.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="appAction">An <see cref="AppAction"/></param>
    /// <param name="callback">The callback to invoke when the <see cref="AppAction"/> is triggered.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder RegisterAppAction(this PrismAppBuilder builder, AppAction appAction, Func<IContainerProvider, INavigationService, AppAction, Task> callback)
    {
        builder.MauiBuilder.ConfigureEssentials(essentials =>
        {
            essentials.AddAppAction(appAction)
                .OnAppAction(async action =>
                {
                    if (appAction.Id != action.Id)
                        return;

                    var app = Application.Current;
                    if (app?.Handler?.MauiContext?.Services is null || app.Dispatcher is null)
                        return;

                    var container = app.Handler.MauiContext.Services.GetRequiredService<IContainerProvider>();
                    var navigation = container.Resolve<INavigationService>();
                    await app.Dispatcher.DispatchAsync(() =>
                    {
                        return callback(container, navigation, action);
                    });
                });
        });
        return builder;
    }

    /// <summary>
    /// Registers an <see cref="AppAction"/> with a callback that will be invoked on the UI thread for the specified <see cref="AppAction"/>.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="appAction">An <see cref="AppAction"/></param>
    /// <param name="callback">The callback to invoke when the <see cref="AppAction"/> is triggered.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder RegisterAppAction(this PrismAppBuilder builder, AppAction appAction, Func<INavigationService, AppAction, Task> callback)
    {
        builder.MauiBuilder.ConfigureEssentials(essentials =>
        {
            essentials.AddAppAction(appAction)
                .OnAppAction(async action =>
                {
                    if (appAction.Id != action.Id)
                        return;

                    var app = Application.Current;
                    if (app?.Handler?.MauiContext?.Services is null || app.Dispatcher is null)
                        return;

                    var navigation = app.Handler.MauiContext.Services.GetRequiredService<INavigationService>();
                    await app.Dispatcher.DispatchAsync(() =>
                    {
                        return callback(navigation, action);
                    });
                });
        });
        return builder;
    }
}
