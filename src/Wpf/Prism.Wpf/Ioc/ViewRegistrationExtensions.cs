using Prism.Mvvm;

namespace Prism.Ioc;

public static partial class IContainerRegistryExtensions
{
    private static IContainerRegistry RegisterView(this IContainerRegistry containerRegistry,
        Type viewType, Type viewModelType, string name, ViewType registryType)
    {
        if (viewType is null)
            throw new ArgumentNullException(nameof(viewType));
#if UNO_WINUI
        var requiresControl = true;
#else
        // Desktop regions also support ViewModels rendered through data templates.
        var requiresControl = registryType == ViewType.Dialog;
#endif
        if (requiresControl && !typeof(FrameworkElement).IsAssignableFrom(viewType))
            throw new ArgumentException("The view must inherit from FrameworkElement.", nameof(viewType));

        name = string.IsNullOrWhiteSpace(name) ? viewType.Name : name;

        // Keep named resolution and type-based ViewModel wiring available to existing applications.
        containerRegistry.Register(typeof(object), viewType, name);
        if (!containerRegistry.IsRegistered(viewType))
            containerRegistry.Register(viewType);
        if (viewModelType is not null)
        {
            if (!containerRegistry.IsRegistered(viewModelType))
                containerRegistry.Register(viewModelType);
            ViewModelLocationProvider.Register(viewType.ToString(), viewModelType);
        }

        return containerRegistry.RegisterInstance(new ViewRegistration
        {
            Type = registryType,
            Name = name,
            View = viewType,
            ViewModel = viewModelType
        }, Guid.NewGuid().ToString()); // Preserve multiple entries with both DryIoc and Unity.
    }
}
