using System.Diagnostics.CodeAnalysis;
using Prism.Dialogs;
using Prism.Mvvm;

namespace Prism.Ioc;

public static class DialogRegistrationExtensions
{
    public static IContainerRegistry RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IContainerRegistry containerRegistry, string name = null)
            where TView : View =>
        containerRegistry.RegisterDialog(typeof(TView), null, name);

    public static IContainerRegistry RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IContainerRegistry containerRegistry, string name = null)
        where TView : View =>
        containerRegistry.RegisterDialog(typeof(TView), typeof(TViewModel), name);

    public static IContainerRegistry RegisterDialog(this IContainerRegistry container, Type view, Type viewModel, string name = null)
    {
        container.RegisterInstance(GetViewRegistration(view, viewModel, name))
            .Register(view);

        if (viewModel != null)
            container.Register(viewModel);
        return container;
    }

    public static IContainerRegistry RegisterDialogContainer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(this IContainerRegistry container)
        where T : class, IDialogContainer =>
        container.Register<IDialogContainer, T>();

    public static IServiceCollection RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IServiceCollection services, string name = null)
            where TView : View =>
        services.RegisterDialog(typeof(TView), null, name);

    public static IServiceCollection RegisterDialog<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IServiceCollection services, string name = null)
        where TView : View =>
        services.RegisterDialog(typeof(TView), typeof(TViewModel), name);

    public static IServiceCollection RegisterDialog(this IServiceCollection services, Type view, Type viewModel, string name = null)
    {
        services.AddSingleton(GetViewRegistration(view, viewModel, name))
            .AddTransient(view);

        if (viewModel != null)
            services.AddTransient(viewModel);
        return services;
    }

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
