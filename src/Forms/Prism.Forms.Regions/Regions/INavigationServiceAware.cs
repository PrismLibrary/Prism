using Prism.Navigation;

namespace Prism.Regions
{
    internal interface INavigationServiceAware
    {
        INavigationService NavigationService { get; set; }
    }
}
