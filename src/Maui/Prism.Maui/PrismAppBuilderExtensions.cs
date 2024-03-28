using Microsoft.Extensions.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Builder;

namespace Prism;

/// <summary>
/// Common extensions and overloads for the <see cref="PrismAppBuilder"/>
/// </summary>
public static class PrismAppBuilderExtensions
{
    private static bool s_didRegisterModules = false;

    /// <summary>
    /// Configures the <see cref="MauiAppBuilder"/> to use Prism with a callback for the <see cref="PrismAppBuilder"/>
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/>.</param>
    /// <param name="containerExtension">The instance of the <see cref="IContainerExtension"/> Prism should use.</param>
    /// <param name="configurePrism">A delegate callback for the <see cref="PrismAppBuilder"/></param>
    /// <returns>The <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, IContainerExtension containerExtension, Action<PrismAppBuilder> configurePrism)
    {
        var prismBuilder = new PrismAppBuilder(containerExtension, builder);
        configurePrism(prismBuilder);
        return builder;
    }

    /// <summary>
    /// Provides a Delegate to invoke when the App is initialized.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
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

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="uri">The initial Navigation Uri.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, string uri) =>
        builder.CreateWindow(navigation => navigation.NavigateAsync(uri));

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="uri">The intial Navigation Uri.</param>
    /// <param name="onError">A delegate callback if the navigation fails.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, string uri, Action<Exception> onError) =>
        builder.CreateWindow(async navigation =>
        {
            var result = await navigation.NavigateAsync(uri);
            if (result.Exception is not null)
                onError(result.Exception);
        });

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="createWindow">The Navigation Delegate.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<IContainerProvider, INavigationService, Task> createWindow) =>
        builder.CreateWindow((c, n) => createWindow(c, n));

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="createWindow">The Navigation Delegate.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<INavigationService, Task> createWindow) =>
        builder.CreateWindow((_, n) => createWindow(n));

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="createWindow">The Navigation Delegate.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<INavigationService, INavigationBuilder> createWindow) =>
        builder.CreateWindow(n => createWindow(n).NavigateAsync());

    /// <summary>
    /// When the <see cref="Application"/> is started and the native platform calls <see cref="IApplication.CreateWindow(IActivationState?)"/>
    /// this delegate will be invoked to do your initial Navigation.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="createWindow">The Navigation Delegate.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, Func<IContainerProvider, INavigationService, INavigationBuilder> createWindow) =>
        builder.CreateWindow((c, n) => createWindow(c, n).NavigateAsync());


    /// <summary>
    /// Provides a configuration delegate to add services to the <see cref="MauiAppBuilder.Services"/>
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="configureServices">Configuration Delegate</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder ConfigureServices(this PrismAppBuilder builder, Action<IServiceCollection> configureServices)
    {
        configureServices(builder.MauiBuilder.Services);
        return builder;
    }

    /// <summary>
    /// Provides a delegate to configure Logging within the Maui application
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="configureLogging"></param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
    public static PrismAppBuilder ConfigureLogging(this PrismAppBuilder builder, Action<ILoggingBuilder> configureLogging)
    {
        configureLogging(builder.MauiBuilder.Logging);
        return builder;
    }

    /// <summary>
    /// Provides a configuration Delegate to the <see cref="ViewModelLocationProvider"/> to set the
    /// DefaultViewTypeToViewModelTypeResolver.
    /// </summary>
    /// <param name="builder">The <see cref="PrismAppBuilder"/>.</param>
    /// <param name="viewModelTypeResolver">The Configuration Delegate for the Default ViewType to ViewModelType Resolver.</param>
    /// <returns>The <see cref="PrismAppBuilder"/>.</returns>
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
