namespace Prism.Navigation;


public interface IGlobalNavigationObserver
{
    IObservable<NavigationRequestContext> NavigationRequest { get; }
    /// <summary>Observes selected-tab changes, including tabs containing navigation pages.</summary>
    IObservable<TabChangedContext> TabChanged { get; }
}
