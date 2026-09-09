namespace Prism.Mvvm;

/// <summary>
/// Creates views using the shared view registration pipeline.
/// </summary>
public abstract class ViewRegistryBase : ViewRegistryBase<object>
{
    /// <summary>
    /// Initializes a registry for the specified kind of view.
    /// </summary>
    /// <param name="registryType">The kind of view managed by this registry.</param>
    /// <param name="registrations">The available view registrations.</param>
    protected ViewRegistryBase(ViewType registryType, IEnumerable<ViewRegistration> registrations)
        : base(registryType, registrations)
    {
    }

    /// <inheritdoc />
    protected override void ConfigureView(object view, IContainerProvider container)
    {
    }

    /// <inheritdoc />
    protected override void Autowire(object view) => ViewModelLocator.Autowire(view);

    /// <inheritdoc />
    protected override void SetContainerProvider(object view, IContainerProvider container)
    {
        if (view is FrameworkElement element)
            element.SetValue(ViewModelLocator.ContainerProviderProperty, container);
    }

    /// <inheritdoc />
    protected override void SetNavigationNameProperty(object view, string name)
    {
        if (view is FrameworkElement element)
            ViewModelLocator.SetNavigationName(element, name);
    }

    /// <inheritdoc />
    protected override void SetViewModelProperty(object view, Type viewModelType)
    {
        if (view is FrameworkElement element)
            element.SetValue(ViewModelLocator.ViewModelProperty, viewModelType);
    }
}
