using Prism.Behaviors;
using Prism.Events;
using Prism.Maui.Tests.Mocks;
using Prism.Navigation;
using Prism.Mvvm;
using System.Reflection;

namespace Prism.Maui.Tests.Fixtures.Behaviors;

public class TabbedPageNavigationObserverBehaviorTests
{
    [Fact]
    public void MissingNavigationMetadataIsReportedAsNull()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(changes.Add);
        var first = new ContentPage();
        var second = new ContentPage();
        var tabs = new TabbedPage();
        ViewModelLocator.SetNavigationName(first, null);
        ViewModelLocator.SetNavigationName(second, null);
        ViewModelLocator.SetNavigationName(tabs, null);
        tabs.Children.Add(first);
        tabs.Children.Add(second);
        tabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        var window = new Window(tabs);

        tabs.CurrentPage = second;

        var change = Assert.Single(changes);
        Assert.Null(change.TabbedPageName);
        Assert.Null(change.PreviousTabName);
        Assert.Null(change.CurrentTabName);
        GC.KeepAlive(window);
    }

    [Fact]
    public void UsesPageIdentityForChangesEvenWhenNavigationNamesAreIdentical()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(changes.Add);
        var tabs = new TabbedPage();
        tabs.Children.Add(CreatePage("Same"));
        tabs.Children.Add(CreatePage("Same"));
        tabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        var window = new Window(tabs);

        tabs.CurrentPage = tabs.Children[1];
        var change = Assert.Single(changes);
        Assert.Equal("Same", change.PreviousTabName);
        Assert.Equal("Same", change.CurrentTabName);
        tabs.CurrentPage = tabs.Children[1];
        Assert.Single(changes);
        GC.KeepAlive(window);
    }

    [Fact]
    public void NavigationContextContainsNullableMetadataWithoutViewTypes()
    {
        Assert.DoesNotContain(typeof(NavigationRequestContext).GetProperties(),
            property => typeof(BindableObject).IsAssignableFrom(property.PropertyType));
        var nullability = new NullabilityInfoContext();
        foreach (var property in typeof(NavigationRequestContext).GetProperties())
        {
            if (!property.PropertyType.IsValueType)
            {
                Assert.Equal(NullabilityState.Nullable, nullability.Create(property).ReadState);
                Assert.Equal(NullabilityState.Nullable, nullability.Create(property).WriteState);
            }
        }

        var context = new NavigationRequestContext();
        Assert.Null(context.Uri);
        Assert.Null(context.Parameters);
        Assert.Null(context.Result);
        Assert.False(context.Cancelled);
        Assert.Null(context.TabbedPageName);
        Assert.Null(context.PreviousTabName);
        Assert.Null(context.CurrentTabName);
    }

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
        var first = CreatePage("First");
        var second = CreatePage("Second");
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
        Assert.Equal("First", changes[1].PreviousTabName);
        Assert.Equal("Second", changes[1].CurrentTabName);
    }

    [Fact]
    public void MultipleWindowsKeepTheirTabSelectionStateIndependent()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var changes = new List<NavigationRequestContext>();
        using var token = aggregator.GetEvent<NavigationRequestEvent>().Subscribe(changes.Add);
        var firstTabs = new TabbedPage();
        ViewModelLocator.SetNavigationName(firstTabs, "FirstTabs");
        var secondTabs = new TabbedPage();
        ViewModelLocator.SetNavigationName(secondTabs, "SecondTabs");
        firstTabs.Children.Add(CreatePage("FirstA"));
        firstTabs.Children.Add(CreatePage("FirstB"));
        secondTabs.Children.Add(CreatePage("SecondA"));
        secondTabs.Children.Add(CreatePage("SecondB"));
        firstTabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        secondTabs.Behaviors.Add(new TabbedPageNavigationObserverBehavior(aggregator));
        var firstWindow = new Window(firstTabs);
        var secondWindow = new Window(secondTabs);

        firstTabs.CurrentPage = firstTabs.Children[1];
        secondTabs.CurrentPage = secondTabs.Children[1];
        firstTabs.CurrentPage = firstTabs.Children[0];

        Assert.Equal(3, changes.Count);
        Assert.Equal("FirstTabs", changes[0].TabbedPageName);
        Assert.Equal("FirstA", changes[0].PreviousTabName);
        Assert.Equal("FirstB", changes[0].CurrentTabName);
        Assert.Equal("SecondTabs", changes[1].TabbedPageName);
        Assert.Equal("SecondA", changes[1].PreviousTabName);
        Assert.Equal("SecondB", changes[1].CurrentTabName);
        Assert.Equal("FirstTabs", changes[2].TabbedPageName);
        Assert.Equal("FirstB", changes[2].PreviousTabName);
        Assert.Equal("FirstA", changes[2].CurrentTabName);
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
        var first = CreatePage("First");
        var second = CreatePage("Second");
        tabs.Children.Add(first);
        tabs.Children.Add(second);
        tabs.CurrentPage = second;
        Assert.Empty(changes);

        var window = new Window(tabs);
        tabs.CurrentPage = first;
        var change = Assert.Single(changes);
        Assert.Equal("Second", change.PreviousTabName);
        Assert.Equal("First", change.CurrentTabName);
        tabs.Behaviors.Remove(behavior);
        tabs.CurrentPage = second;
        Assert.Single(changes);

        tabs.Behaviors.Add(behavior);
        tabs.CurrentPage = first;
        Assert.Equal(2, changes.Count);
        Assert.Equal("Second", changes[1].PreviousTabName);
        Assert.Equal("First", changes[1].CurrentTabName);
        GC.KeepAlive(window);
    }

    [Fact]
    public void SubscriberCanSelectAnotherTabReentrantly()
    {
        DispatcherProvider.SetCurrent(TestDispatcher.Provider);
        var aggregator = new EventAggregator();
        var first = CreatePage("First");
        var second = CreatePage("Second");
        var third = CreatePage("Third");
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
            if (context.CurrentTabName == "Second")
                tabs.CurrentPage = third;
        });

        tabs.CurrentPage = second;

        Assert.Equal(2, changes.Count);
        Assert.Equal("First", changes[0].PreviousTabName);
        Assert.Equal("Second", changes[0].CurrentTabName);
        Assert.Equal("Second", changes[1].PreviousTabName);
        Assert.Equal("Third", changes[1].CurrentTabName);
        GC.KeepAlive(window);
    }

    private static ContentPage CreatePage(string navigationName)
    {
        var page = new ContentPage();
        ViewModelLocator.SetNavigationName(page, navigationName);
        return page;
    }

}
