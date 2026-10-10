using System.Diagnostics.CodeAnalysis;
using Prism.Mvvm;

namespace Prism.Ioc;

/// <summary>
/// Navigation Extensions for working with the <see cref="IServiceCollection"/>
/// </summary>
public static class MicrosoftDependencyInjectionExtensions
{
#if !UNO_WINUI
    private static readonly Type PageType = typeof(Page);

    /// <summary>Registers a page for navigation using an explicit name or the page type name.</summary>
    /// <typeparam name="TView">The page type to register.</typeparam>
    /// <param name="services">The service collection to update.</param>
    /// <param name="name">The navigation name; null or empty uses the page type name.</param>
    /// <returns>The same registration collection for additional registrations.</returns>
    /// <remarks>Pages and explicitly supplied view models are registered for resolution, and a page registration is added to the view registry.</remarks>
    public static IServiceCollection RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IServiceCollection services, string name = null)
            where TView : Page =>
            services.RegisterForNavigation(typeof(TView), null, name);

    /// <summary>Registers a page for navigation using an explicit name or the page type name.</summary>
    /// <typeparam name="TView">The page type to register.</typeparam>
    /// <typeparam name="TViewModel">The view model type associated with the page.</typeparam>
    /// <param name="services">The service collection to update.</param>
    /// <param name="name">The navigation name; null or empty uses the page type name.</param>
    /// <returns>The same registration collection for additional registrations.</returns>
    /// <remarks>Pages and explicitly supplied view models are registered for resolution, and a page registration is added to the view registry.</remarks>
    public static IServiceCollection RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TViewModel>(this IServiceCollection services, string name = null)
        where TView : Page =>
        services.RegisterForNavigation(typeof(TView), typeof(TViewModel), name);

    /// <summary>Registers a page for navigation using an explicit name or the page type name.</summary>
    /// <param name="services">The service collection to update.</param>
    /// <param name="view">The page type to register.</param>
    /// <param name="viewModel">The associated view model type, or null to use view model location conventions.</param>
    /// <param name="name">The navigation name; null or empty uses the page type name.</param>
    /// <returns>The same registration collection for additional registrations.</returns>
    /// <remarks>Pages and explicitly supplied view models are registered for resolution, and a page registration is added to the view registry.</remarks>
    public static IServiceCollection RegisterForNavigation(this IServiceCollection services, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type view, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] Type viewModel, string name = null)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (!view.IsAssignableTo(PageType))
            throw new InvalidOperationException($"The view type '{view.FullName}' is not a type of Page.");

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
#endif
}
