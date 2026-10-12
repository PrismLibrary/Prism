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
    /// <summary>
    /// Adds Prism to the application using the supplied container and configuration callback.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="containerExtension">The container to use if Prism is not yet configured for this builder.</param>
    /// <param name="configurePrism">The callback configuring the shared Prism builder.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/>, <paramref name="containerExtension"/>, or
    /// <paramref name="configurePrism"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// The first call selects the container owned by the application. Subsequent calls retain that container
    /// and execute their callbacks on the same Prism builder. A later supplied container is not
    /// used or disposed and remains owned by the caller. Complete configuration before calling
    /// <see cref="MauiAppBuilder.Build"/>.
    /// </remarks>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, IContainerExtension containerExtension, Action<PrismAppBuilder> configurePrism)
    {
        ArgumentNullException.ThrowIfNull(containerExtension);
        return builder.UsePrism(() => containerExtension, configurePrism);
    }

    /// <summary>
    /// Adds Prism to the application, creating its container only on the first call for this builder.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="containerFactory">The factory creating the non-null container owned by the application.</param>
    /// <param name="configurePrism">The callback configuring the shared Prism builder.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/>, <paramref name="containerFactory"/>, or
    /// <paramref name="configurePrism"/> is <see langword="null"/>, or the factory returns
    /// <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// Subsequent calls reuse the first Prism builder and do not invoke the container factory.
    /// Configuration callbacks run in call order. Pending library configuration runs before
    /// this call's callback. Separate application builders retain separate Prism builders.
    /// Complete configuration before calling <see cref="MauiAppBuilder.Build"/>.
    /// </remarks>
    public static MauiAppBuilder UsePrism(this MauiAppBuilder builder, Func<IContainerExtension> containerFactory, Action<PrismAppBuilder> configurePrism)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(containerFactory);
        ArgumentNullException.ThrowIfNull(configurePrism);

        var properties = builder.Properties;
        if (!properties.TryGetValue(typeof(PrismAppBuilder), out var value) || value is not PrismAppBuilder prismBuilder)
        {
            prismBuilder = new PrismAppBuilder(containerFactory(), builder);
            properties[typeof(PrismAppBuilder)] = prismBuilder;

            if (properties.Remove(PendingConfigurationKey, out var pending))
            {
                foreach (var configure in (List<Action<PrismAppBuilder>>)pending)
                    configure(prismBuilder);
            }
        }

        configurePrism(prismBuilder);
        return builder;
    }

    private static readonly object PendingConfigurationKey = new();

    /// <summary>
    /// Contributes library configuration to the application's Prism builder without creating a container.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configurePrism">The callback contributing services, modules, or initialization behavior.</param>
    /// <returns>The same application builder for further configuration.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> or <paramref name="configurePrism"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// Complete configuration before calling <see cref="MauiAppBuilder.Build"/>.
    /// Before UsePrism, callbacks are deferred until the application selects its container.
    /// After UsePrism, callbacks run immediately. Calling ConfigurePrism alone does not enable Prism.
    /// Registration and initialization callbacks accumulate in call order. Initialization runs
    /// once for the shared Prism builder. CreateWindow replaces the previous window callback,
    /// allowing the application to choose its initial navigation.
    /// </remarks>
    /// <example>
    /// A library can contribute a module before the application selects its container. This
    /// example assumes that <c>MapsModule</c> implements <see cref="IModule"/> and that the
    /// application's pages are registered for navigation.
    /// <code>
    /// builder.ConfigurePrism(prism => prism
    ///     .ConfigureModuleCatalog(catalog => catalog.AddModule&lt;MapsModule&gt;()));
    /// builder.UsePrism(container, prism => prism.CreateWindow("NavigationPage/MainPage"));
    /// </code>
    /// </example>
    public static MauiAppBuilder ConfigurePrism(this MauiAppBuilder builder, Action<PrismAppBuilder> configurePrism)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configurePrism);

        var properties = builder.Properties;
        if (properties.TryGetValue(typeof(PrismAppBuilder), out var value) && value is PrismAppBuilder prismBuilder)
        {
            configurePrism(prismBuilder);
        }
        else
        {
            if (!properties.TryGetValue(PendingConfigurationKey, out var pending))
            {
                pending = new List<Action<PrismAppBuilder>>();
                properties[PendingConfigurationKey] = pending;
            }

            ((List<Action<PrismAppBuilder>>)pending).Add(configurePrism);
        }

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
        builder.RegisterTypes(container =>
        {
            container.TryRegisterSingleton<IModuleCatalog, ModuleCatalog>();
            container.TryRegisterSingleton<IModuleManager, ModuleManager>();
            container.TryRegisterSingleton<IModuleInitializer, ModuleInitializer>();
        });

        return builder.AddModuleCatalogAction(container =>
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
    /// Provides a delegate to configure application logging.
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
