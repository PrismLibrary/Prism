namespace Prism.Navigation;


/// <summary>Exposes completed Prism page navigation requests as an observable stream.</summary>
public interface IGlobalNavigationObserver
{
    /// <summary>Gets the stream of navigation outcomes, including failed and cancelled requests.</summary>
    IObservable<NavigationRequestContext> NavigationRequest { get; }
}
