using System.Reactive.Linq;
using Prism.Behaviors;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class TabChangedObserverTests : TestBase
{
    public TabChangedObserverTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("MockViewB")]
    [InlineData("NavigationPage%2FMockViewB")]
    public async Task ObservesTabChangesThroughPublicRegistration(string secondTab)
    {
        // Each app must register its own observer, regardless of previous builders.
        for (var appIndex = 0; appIndex < 2; appIndex++)
        {
            var changes = new List<NavigationRequestContext>();
            var requests = new List<NavigationRequestContext>();
            var otherSubscriber = new List<NavigationRequestContext>();
            IGlobalNavigationObserver observer = null;
            var selectedTab = secondTab.Replace("%2F", "|");
            using var app = CreateBuilder(prism => prism
                .AddGlobalNavigationObserver((c, observable) =>
                {
                    observer = c.Resolve<IGlobalNavigationObserver>();
                    observable.Subscribe(context =>
                    {
                        requests.Add(context);
                        if (context.Type == NavigationRequestType.TabChanged)
                            changes.Add(context);
                    });
                })
                .AddGlobalNavigationObserver(observable => observable.Subscribe(otherSubscriber.Add))
                .CreateWindow($"TabbedPage?createTab=MockViewA&createTab={secondTab}&selectedTab={selectedTab}"))
                .Build();
            var window = GetWindow(app);
            var tabs = Assert.IsType<TabbedPage>(window.Page);
            Assert.Empty(changes);
            Assert.Single(requests);
            Assert.Equal(NavigationRequestType.Navigate, requests[0].Type);
            Assert.Null(requests[0].TabbedPage);
            Assert.Null(requests[0].PreviousTab);
            Assert.Null(requests[0].CurrentTab);

            var first = tabs.Children[0];
            var second = tabs.Children[1];
            Assert.Same(second, tabs.CurrentPage);
            tabs.CurrentPage = first;
            var change = Assert.Single(changes);
            Assert.Single(tabs.Behaviors.OfType<TabbedPageNavigationObserverBehavior>());
            Assert.Same(tabs, change.TabbedPage);
            Assert.Same(second, change.PreviousTab);
            Assert.Same(first, change.CurrentTab);
            Assert.Equal(NavigationRequestType.TabChanged, change.Type);
            Assert.True(change.Result.Success);
            Assert.Null(change.Result.Exception);
            Assert.False(change.Cancelled);
            Assert.Null(change.Uri);
            Assert.Empty(change.Parameters);
            Assert.Equal(2, requests.Count);
            Assert.Equal(requests, otherSubscriber);

            tabs.CurrentPage = first;
            Assert.Single(changes);
            var navigation = Prism.Navigation.Xaml.Navigation.GetNavigationService(first);
            var result = await navigation.SelectTabAsync("MockViewB");
            Assert.True(result.Success);
            Assert.Equal(2, changes.Count);
            Assert.Same(first, changes[1].PreviousTab);
            Assert.Same(second, changes[1].CurrentTab);
            Assert.Equal(3, requests.Count);
            Assert.Equal(requests, otherSubscriber);

            var disposable = Assert.IsAssignableFrom<IDisposable>(observer);
            disposable.Dispose();
            disposable.Dispose();
            tabs.CurrentPage = first;
            Assert.Equal(2, changes.Count);
            Assert.Equal(3, requests.Count);
            Assert.Equal(requests, otherSubscriber);
        }
    }
}
