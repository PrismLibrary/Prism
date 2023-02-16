using Prism.Common;
using Prism.Services;

namespace Prism.Ioc;

public static class DialogRegistrationExtensions
{
    public static IContainerRegistry RegisterDialog<TView>(this IContainerRegistry containerRegistry, string name = null)
            where TView : View =>
        containerRegistry.RegisterDialog(typeof(TView), null, name);

    public static IContainerRegistry RegisterDialog<TView, TViewModel>(this IContainerRegistry containerRegistry, string name = null)
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

    public static IContainerRegistry RegisterDialogContainer<T>(this IContainerRegistry container)
        where T : class, IDialogContainer =>
        container.Register<IDialogContainer, T>();

    public static IServiceCollection RegisterDialog<TView>(this IServiceCollection services, string name = null)
            where TView : View =>
        services.RegisterDialog(typeof(TView), null, name);

    public static IServiceCollection RegisterDialog<TView, TViewModel>(this IServiceCollection services, string name = null)
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

    public static IServiceCollection RegisterDialogContainer<T>(this IServiceCollection services)
        where T : class, IDialogContainer =>
        services.AddTransient<IDialogContainer, T>();

    private static ViewRegistration GetViewRegistration(Type view, Type viewModel, string name)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

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
