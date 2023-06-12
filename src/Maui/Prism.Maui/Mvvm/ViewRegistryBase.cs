using Prism.Ioc;
using Prism.Navigation.Xaml;

namespace Prism.Mvvm;

public abstract class ViewRegistryBase : ViewRegistryBase<BindableObject>
{
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
