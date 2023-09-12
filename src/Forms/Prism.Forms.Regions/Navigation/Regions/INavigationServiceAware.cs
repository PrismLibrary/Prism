namespace Prism.Navigation.Regions
{
    internal interface INavigationServiceAware
    {
        INavigationService NavigationService { get; set; }
    }
}
