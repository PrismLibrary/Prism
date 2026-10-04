using System.Reactive.Subjects;
using Prism.Events;

namespace Prism.Navigation;

internal class GlobalNavigationObserver : IGlobalNavigationObserver, IDisposable
{
    private readonly Subject<NavigationRequestContext> _subject;
    private SubscriptionToken _token;
    private readonly Subject<TabChangedContext> _tabChanged = new();
    private SubscriptionToken _tabChangedToken;

    public GlobalNavigationObserver(IEventAggregator eventAggregator)
    {
        _subject = new Subject<NavigationRequestContext>();
        _token = eventAggregator.GetEvent<NavigationRequestEvent>().Subscribe(context => _subject.OnNext(context));
        _tabChangedToken = eventAggregator.GetEvent<TabChangedEvent>().Subscribe(context => _tabChanged.OnNext(context));
    }

    public IObservable<NavigationRequestContext> NavigationRequest => _subject;
    public IObservable<TabChangedContext> TabChanged => _tabChanged;

    public void Dispose()
    {
        if (_token is null)
            return;

        _token.Dispose();
        _token = null;
        _tabChangedToken.Dispose();
        _tabChangedToken = null;
        _tabChanged.Dispose();
        _subject.Dispose();
    }
}
