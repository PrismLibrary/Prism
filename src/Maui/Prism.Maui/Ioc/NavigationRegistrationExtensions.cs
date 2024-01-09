using System.Diagnostics.CodeAnalysis;
using Prism.Mvvm;

namespace Prism.Ioc;

public static class NavigationRegistrationExtensions
{
    public static IContainerRegistry RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView>(this IContainerRegistry container, string name = null)
        where TView : Page =>
        container.RegisterForNavigation(typeof(TView), null, name);

    public static IContainerRegistry RegisterForNavigation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TView, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TViewModel>(this IContainerRegistry container, string name = null)
        where TView : Page =>
        container.RegisterForNavigation(typeof(TView), typeof(TViewModel), name);

    public static IContainerRegistry RegisterForNavigation(this IContainerRegistry container, Type view, Type viewModel, string name = null)
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
