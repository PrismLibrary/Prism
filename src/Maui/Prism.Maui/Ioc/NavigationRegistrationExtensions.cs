using System.Diagnostics.CodeAnalysis;
using Prism.Mvvm;

namespace Prism.Ioc;

/// <summary>Registers pages and their view models for named navigation.</summary>
/// <example>
/// Register a page with a stable route name, then use that name when navigating:
/// <code>
/// container.RegisterForNavigation&lt;DetailsPage, DetailsViewModel&gt;("details");
/// await navigationService.NavigateAsync(new Uri("details?id=42", UriKind.Relative), new NavigationParameters());
/// </code>
/// </example>
public static class NavigationRegistrationExtensions
{
    /// <summary>Registers a page for navigation using an explicit name or the page type name.</summary>
    /// <typeparam name="TView">The page type to register.</typeparam>
    /// <param name="container">The container registry to update.</param>
    /// <param name="name">The navigation name; null or empty uses the page type name.</param>
    /// <returns>The same registration collection for additional registrations.</returns>
    /// <remarks>Pages and explicitly supplied view models are registered for resolution, and a page registration is added to the view registry.</remarks>
    public static IContainerRegistry RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IContainerRegistry container, string name = null)
        where TView : Page =>
        container.RegisterForNavigation(typeof(TView), null, name);

    /// <summary>Registers a page for navigation using an explicit name or the page type name.</summary>
    /// <typeparam name="TView">The page type to register.</typeparam>
    /// <typeparam name="TViewModel">The view model type associated with the page.</typeparam>
    /// <param name="container">The container registry to update.</param>
    /// <param name="name">The navigation name; null or empty uses the page type name.</param>
    /// <returns>The same registration collection for additional registrations.</returns>
    /// <remarks>Pages and explicitly supplied view models are registered for resolution, and a page registration is added to the view registry.</remarks>
    public static IContainerRegistry RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TViewModel>(this IContainerRegistry container, string name = null)
        where TView : Page =>
        container.RegisterForNavigation(typeof(TView), typeof(TViewModel), name);

    /// <summary>Registers a page for navigation using an explicit name or the page type name.</summary>
    /// <param name="container">The container registry to update.</param>
    /// <param name="view">The page type to register.</param>
    /// <param name="viewModel">The associated view model type, or null to use view model location conventions.</param>
    /// <param name="name">The navigation name; null or empty uses the page type name.</param>
    /// <returns>The same registration collection for additional registrations.</returns>
    /// <remarks>Pages and explicitly supplied view models are registered for resolution, and a page registration is added to the view registry.</remarks>
    public static IContainerRegistry RegisterForNavigation(this IContainerRegistry container, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type view, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] Type viewModel, string name = null)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (string.IsNullOrEmpty(name))
            name = view.Name;

        container.RegisterInstance(new ViewRegistration
            {
                Type = ViewType.Page,
                Name = name,
                View = view,
                ViewModel = viewModel
            })
            .Register(view);

        if (viewModel != null)
            container.Register(viewModel);

        return container;
    }
}
