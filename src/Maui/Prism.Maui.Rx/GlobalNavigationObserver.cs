using System.Reactive.Subjects;
using Prism.Events;

namespace Prism.Navigation;

internal class GlobalNavigationObserver : IGlobalNavigationObserver, IDisposable
{
    private readonly Subject<NavigationRequestContext> _subject;
    private SubscriptionToken _token;

    public GlobalNavigationObserver(IEventAggregator eventAggregator)
    {
        _subject = new Subject<NavigationRequestContext>();
        _token = eventAggregator.GetEvent<NavigationRequestEvent>().Subscribe(context => _subject.OnNext(context));
    }

    public IObservable<NavigationRequestContext> NavigationRequest => _subject;

    public void Dispose()
    {
        if (_token is null)
            return;

        _token.Dispose();
        _token = null;
        _subject.Dispose();
    }
}
