using Prism.Navigation.Xaml;

namespace Prism.Mvvm;

/// <summary>
/// The Base class for .NET Maui's ViewModel Registry
/// </summary>
public abstract class ViewRegistryBase : ViewRegistryBase<BindableObject>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewRegistryBase"/>
    /// </summary>
    /// <param name="registryType">The Registry Type</param>
    /// <param name="registrations">The ViewRegistration collection</param>
    protected ViewRegistryBase(ViewType registryType, IEnumerable<ViewRegistration> registrations)
        : base(registryType, registrations)
    {
    }

    /// <summary>Locates a view model for a view whose binding context has not already been assigned.</summary>
    /// <param name="view">The view to autowire.</param>
    protected override void Autowire(BindableObject view)
    {
        if (view.BindingContext is not null)
            return;

        ViewModelLocator.Autowire(view);
    }

    /// <summary>Associates the view with its container provider for service resolution.</summary>
    /// <param name="view">The registered view.</param>
    /// <param name="container">The container provider associated with the view.</param>
    protected override void SetContainerProvider(BindableObject view, IContainerProvider container)
    {
        view.SetContainerProvider(container);
    }

    /// <summary>Stores the registration name on the view for navigation path generation.</summary>
    /// <param name="view">The registered view.</param>
    /// <param name="name">The view's navigation registration name.</param>
    protected override void SetNavigationNameProperty(BindableObject view, string name)
    {
        view.SetValue(ViewModelLocator.NavigationNameProperty, name);
    }

    /// <summary>Stores the explicitly registered view model type on the view.</summary>
    /// <param name="view">The registered view.</param>
    /// <param name="viewModelType">The registered view model type.</param>
    protected override void SetViewModelProperty(BindableObject view, Type viewModelType)
    {
        view.SetValue(ViewModelLocator.ViewModelProperty, viewModelType);
    }
}
