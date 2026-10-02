using Prism.Mvvm;

namespace Prism.Navigation.Regions;

internal record RegionPageRegistration : ViewRegistration
{
    public string RegionName { get; init; }
}
