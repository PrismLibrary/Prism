using Prism.Behaviors;
using Prism.Events;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation;

namespace Prism.Maui.Tests.Fixtures.Behaviors;

public class TabbedPageNavigationObserverBehaviorTests
{
    [Fact]
    public void TracksInitializationAndDetachesWithoutStaleState()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<TabChangedContext>();
        using var token = aggregator.GetEvent<TabChangedEvent>().Subscribe(changes.Add);
        var behavior = new TabbedPageNavigationObserverBehavior(aggregator);
        var tabs = new TabbedPage();
        tabs.Behaviors.Add(behavior);
        var first = new ContentPage();
        var second = new ContentPage();
        tabs.Children.Add(first);
        tabs.Children.Add(second);
        tabs.CurrentPage = second;
        Assert.Empty(changes);

        var window = new Window(tabs);
        tabs.CurrentPage = first;
        var change = Assert.Single(changes);
        Assert.Same(second, change.PreviousTab);
        Assert.Same(first, change.CurrentTab);
        tabs.Behaviors.Remove(behavior);
        tabs.CurrentPage = second;
        Assert.Single(changes);

        tabs.Behaviors.Add(behavior);
        tabs.CurrentPage = first;
        Assert.Equal(2, changes.Count);
        Assert.Same(second, changes[1].PreviousTab);
        Assert.Same(first, changes[1].CurrentTab);
        GC.KeepAlive(window);
    }

    [Fact]
    public void SubscriberCanSelectAnotherTabReentrantly()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var first = new ContentPage();
        var second = new ContentPage();
        var third = new ContentPage();
        var tabs = new TabbedPage();
        tabs.Children.Add(first);
        tabs.Children.Add(second);
        tabs.Children.Add(third);
        tabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        var window = new Window(tabs);
        var changes = new List<TabChangedContext>();
        using var token = aggregator.GetEvent<TabChangedEvent>().Subscribe(context =>
        {
            changes.Add(context);
            if (context.CurrentTab == second)
                tabs.CurrentPage = third;
        });

        tabs.CurrentPage = second;

        Assert.Equal(2, changes.Count);
        Assert.Same(first, changes[0].PreviousTab);
        Assert.Same(second, changes[0].CurrentTab);
        Assert.Same(second, changes[1].PreviousTab);
        Assert.Same(third, changes[1].CurrentTab);
        GC.KeepAlive(window);
    }
}
