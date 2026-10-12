using Prism.Navigation;

namespace Prism.Events;

/// <summary>Publishes the outcome of page navigation requests to navigation observers.</summary>
public class NavigationRequestEvent : PubSubEvent<NavigationRequestContext>
{
}

