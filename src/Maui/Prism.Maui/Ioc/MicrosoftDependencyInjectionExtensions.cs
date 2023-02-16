using Prism.Common;

namespace Prism.Ioc;

public static class MicrosoftDependencyInjectionExtensions
{
    public static void Populate(this IContainerExtension container, IServiceCollection services)
    {
        if (container is not IServiceCollectionAware sca)
            throw new InvalidOperationException("The instance of IContainerExtension does not implement IServiceCollectionAware");

        sca.Populate(services);
    }

    public static IServiceProvider CreateServiceProvider(this IContainerExtension container)
    {
        if (container is not IServiceCollectionAware sca)
            throw new InvalidOperationException("The instance of IContainerExtension does not implement IServiceCollectionAware");

        return sca.CreateServiceProvider();
    }

    public static IServiceCollection RegisterForNavigation<TView>(this IServiceCollection services, string name = null)
            where TView : Page =>
            services.RegisterForNavigation(typeof(TView), null, name);

    public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection services, string name = null)
        where TView : Page =>
        services.RegisterForNavigation(typeof(TView), typeof(TViewModel), name);

    public static IServiceCollection RegisterForNavigation(this IServiceCollection services, Type view, Type viewModel, string name = null)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (string.IsNullOrEmpty(name))
            name = view.Name;

        services.AddSingleton(new ViewRegistration
            {
                Type = ViewType.Page,
                Name = name,
                View = view,
                ViewModel = viewModel
            })
            .AddTransient(view);

        if (viewModel != null)
            services.AddTransient(viewModel);

        return services;
    }
}
