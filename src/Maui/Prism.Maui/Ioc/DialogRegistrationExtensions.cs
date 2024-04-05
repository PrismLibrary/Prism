using System.Diagnostics.CodeAnalysis;
using Prism.Dialogs;
using Prism.Mvvm;

namespace Prism.Ioc;

/// <summary>
/// Provides extension methods for registering dialogs in the Prism container.
/// </summary>
public static class DialogRegistrationExtensions
{
    /// <summary>
    /// Registers a dialog with the specified view type in the container.
    /// </summary>
    /// <typeparam name="TView">The type of the dialog view.</typeparam>
    /// <param name="containerRegistry">The container registry.</param>
    /// <param name="name">The name of the dialog.</param>
    /// <returns>The container registry.</returns>
    /// <remarks>
    /// The view type must inherit from <see cref="View"/>.
    /// </remarks>
    public static IContainerRegistry RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IContainerRegistry containerRegistry, string name = null)
            where TView : View =>
        containerRegistry.RegisterDialog(typeof(TView), null, name);

    /// <summary>
    /// Registers a dialog with the specified view and view model types in the container.
    /// </summary>
    /// <typeparam name="TView">The type of the dialog view.</typeparam>
    /// <typeparam name="TViewModel">The type of the dialog view model.</typeparam>
    /// <param name="containerRegistry">The container registry.</param>
    /// <param name="name">The name of the dialog.</param>
    /// <returns>The container registry.</returns>
    /// <remarks>
    /// The view type must inherit from <see cref="View"/>.
    /// </remarks>
    public static IContainerRegistry RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IContainerRegistry containerRegistry, string name = null)
        where TView : View =>
        containerRegistry.RegisterDialog(typeof(TView), typeof(TViewModel), name);

    /// <summary>
    /// Registers a dialog with the specified view and view model types in the container.
    /// </summary>
    /// <param name="container">The container registry.</param>
    /// <param name="view">The type of the dialog view.</param>
    /// <param name="viewModel">The type of the dialog view model.</param>
    /// <param name="name">The name of the dialog.</param>
    /// <returns>The container registry.</returns>
    /// <remarks>
    /// The view type must inherit from <see cref="View"/>.
    /// </remarks>
    public static IContainerRegistry RegisterDialog(this IContainerRegistry container, Type view, Type viewModel, string name = null)
    {
        container.RegisterInstance(GetViewRegistration(view, viewModel, name))
            .Register(view);

        if (viewModel != null)
            container.Register(viewModel);
        return container;
    }

    /// <summary>
    /// Registers a dialog container in the container.
    /// </summary>
    /// <typeparam name="T">The type of the dialog container.</typeparam>
    /// <param name="container">The container registry.</param>
    /// <returns>The container registry.</returns>
    public static IContainerRegistry RegisterDialogContainer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(this IContainerRegistry container)
        where T : class, IDialogContainer =>
        container.Register<IDialogContainer, T>();

    /// <summary>
    /// Registers a dialog with the specified view type in the service collection.
    /// </summary>
    /// <typeparam name="TView">The type of the dialog view.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name of the dialog.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// The view type must inherit from <see cref="View"/>.
    /// </remarks>
    public static IServiceCollection RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IServiceCollection services, string name = null)
            where TView : View =>
        services.RegisterDialog(typeof(TView), null, name);

    /// <summary>
    /// Registers a dialog with the specified view and view model types in the service collection.
    /// </summary>
    /// <typeparam name="TView">The type of the dialog view.</typeparam>
    /// <typeparam name="TViewModel">The type of the dialog view model.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name of the dialog.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// The view type must inherit from <see cref="View"/>.
    /// </remarks>
    public static IServiceCollection RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IServiceCollection services, string name = null)
        where TView : View =>
        services.RegisterDialog(typeof(TView), typeof(TViewModel), name);

    /// <summary>
    /// Registers a dialog with the specified view and view model types in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="view">The type of the dialog view.</param>
    /// <param name="viewModel">The type of the dialog view model.</param>
    /// <param name="name">The name of the dialog.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// The view type must inherit from <see cref="View"/>.
    /// </remarks>
    public static IServiceCollection RegisterDialog(this IServiceCollection services, Type view, Type viewModel, string name = null)
    {
        services.AddSingleton(GetViewRegistration(view, viewModel, name))
            .AddTransient(view);

        if (viewModel != null)
            services.AddTransient(viewModel);
        return services;
    }

    /// <summary>
    /// Registers a dialog container in the service collection.
    /// </summary>
    /// <typeparam name="T">The type of the dialog container.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection RegisterDialogContainer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(this IServiceCollection services)
        where T : class, IDialogContainer =>
        services.AddTransient<IDialogContainer, T>();

    private static ViewRegistration GetViewRegistration(Type view, Type viewModel, string name)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (!view.IsAssignableTo(typeof(View)))
            throw new InvalidOperationException($"The Dialog '{view.FullName}' must inherit from Microsoft.Maui.Controls.View");

        if (string.IsNullOrEmpty(name))
            name = view.Name;

        return new ViewRegistration
        {
            Type = ViewType.Dialog,
            Name = name,
            View = view,
            ViewModel = viewModel
        };
    }
}
