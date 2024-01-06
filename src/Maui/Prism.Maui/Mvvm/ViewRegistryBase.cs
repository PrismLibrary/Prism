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

    protected override void Autowire(BindableObject view)
    {
        if (view.BindingContext is not null)
            return;

        ViewModelLocator.Autowire(view);
    }

    protected override void SetContainerProvider(BindableObject view, IContainerProvider container)
    {
        view.SetContainerProvider(container);
    }

    protected override void SetNavigationNameProperty(BindableObject view, string name)
    {
        view.SetValue(ViewModelLocator.NavigationNameProperty, name);
    }

    protected override void SetViewModelProperty(BindableObject view, Type viewModelType)
    {
        view.SetValue(ViewModelLocator.ViewModelProperty, viewModelType);
    }
}
