using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Navigation.Regions;

internal class RegionNavigationRegistry : ViewRegistryBase, IRegionNavigationRegistry
{
    public RegionNavigationRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Region, registrations)
    {
    }

    protected override void ConfigureView(BindableObject bindable, IContainerProvider container)
    {
    }
}
