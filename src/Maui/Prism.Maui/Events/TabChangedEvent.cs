using Prism.Navigation;

namespace Prism.Events;

/// <summary>Publishes selected-tab changes independently of navigation requests.</summary>
public class TabChangedEvent : PubSubEvent<TabChangedContext>
{
}
