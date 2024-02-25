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

    public static IServiceCollection RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IServiceCollection services, string name = null)
            where TView : Page =>
            services.RegisterForNavigation(typeof(TView), null, name);

    public static IServiceCollection RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IServiceCollection services, string name = null)
        where TView : Page =>
        services.RegisterForNavigation(typeof(TView), typeof(TViewModel), name);

    public static IServiceCollection RegisterForNavigation(this IServiceCollection services, Type view, Type viewModel, string name = null)
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
