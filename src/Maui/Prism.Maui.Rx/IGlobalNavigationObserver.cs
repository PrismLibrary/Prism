namespace Prism.Navigation;


public interface IGlobalNavigationObserver
{
    IObservable<NavigationRequestContext> NavigationRequest { get; }
}
