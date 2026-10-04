using Prism.Behaviors;
using Prism.Events;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation;

namespace Prism.Maui.Tests.Fixtures.Behaviors;

public class TabbedPageNavigationObserverBehaviorTests
{
    [Fact]
    public void InitialSelectionAndChangesOutsideAWindowAreNotReported()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(changes.Add);
        var tabs = new TabbedPage();
        tabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        var window = new Window(tabs);
        var first = new ContentPage();
        var second = new ContentPage();
        tabs.Children.Add(first);
        tabs.Children.Add(second);
        Assert.Empty(changes);

        tabs.CurrentPage = second;
        Assert.Single(changes);
        window.Page = new ContentPage();
        tabs.CurrentPage = first;
        Assert.Single(changes);

        window.Page = tabs;
        tabs.CurrentPage = second;
        Assert.Equal(2, changes.Count);
        Assert.Same(first, changes[1].PreviousTab);
        Assert.Same(second, changes[1].CurrentTab);
    }

    [Fact]
    public void MultipleWindowsKeepTheirTabSelectionStateIndependent()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(changes.Add);
        var firstTabs = new TabbedPage();
        var secondTabs = new TabbedPage();
        firstTabs.Children.Add(new ContentPage());
        firstTabs.Children.Add(new ContentPage());
        secondTabs.Children.Add(new ContentPage());
        secondTabs.Children.Add(new ContentPage());
        firstTabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        secondTabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        var firstWindow = new Window(firstTabs);
        var secondWindow = new Window(secondTabs);

        firstTabs.CurrentPage = firstTabs.Children[1];
        secondTabs.CurrentPage = secondTabs.Children[1];
        firstTabs.CurrentPage = firstTabs.Children[0];

        Assert.Equal(3, changes.Count);
        Assert.Same(firstTabs, changes[0].TabbedPage);
        Assert.Same(firstTabs.Children[0], changes[0].PreviousTab);
        Assert.Same(firstTabs.Children[1], changes[0].CurrentTab);
        Assert.Same(secondTabs, changes[1].TabbedPage);
        Assert.Same(secondTabs.Children[0], changes[1].PreviousTab);
        Assert.Same(secondTabs.Children[1], changes[1].CurrentTab);
        Assert.Same(firstTabs, changes[2].TabbedPage);
        Assert.Same(firstTabs.Children[1], changes[2].PreviousTab);
        Assert.Same(firstTabs.Children[0], changes[2].CurrentTab);
        GC.KeepAlive(firstWindow);
        GC.KeepAlive(secondWindow);
    }

    [Fact]
    public void TracksInitializationAndDetachesWithoutStaleState()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(changes.Add);
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
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(context =>
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
