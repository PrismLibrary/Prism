using System.Reactive.Linq;
using Prism.Behaviors;
using Prism.Controls;
using Prism.DryIoc.Maui.Tests.Mocks.Views;
using Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

#nullable enable

namespace Prism.DryIoc.Maui.Tests.Fixtures.Navigation;

public class TabChangedObserverTests : TestBase
{
    public TabChangedObserverTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task ExistingNavigationRequestsKeepTheirValuesAndLeaveTabNamesAbsent()
    {
        var requests = new List<NavigationRequestContext>();
        using var app = CreateBuilder(prism => prism
            .AddGlobalNavigationObserver(observable => observable.Subscribe(requests.Add))
            .CreateWindow("NavigationPage/MockViewA/MockViewB"))
            .Build();
        var window = GetWindow(app);
        var navigate = Assert.Single(requests);
        Assert.Equal(NavigationRequestType.Navigate, navigate.Type);
        Assert.NotNull(navigate.Uri);

        var navigation = Prism.Navigation.Xaml.Navigation.GetNavigationService(window.CurrentPage);
        Assert.True((await navigation.GoBackAsync()).Success);
        Assert.Equal(2, requests.Count);
        Assert.Equal(NavigationRequestType.GoBack, requests[1].Type);
        Assert.Null(requests[1].Uri);
        foreach (var request in requests)
        {
            Assert.IsAssignableFrom<INavigationParameters>(request.Parameters);
            Assert.True(Assert.IsAssignableFrom<INavigationResult>(request.Result).Success);
            Assert.Null(request.TabbedPageName);
            Assert.Null(request.PreviousTabName);
            Assert.Null(request.CurrentTabName);
        }
    }

    [Theory]
    [InlineData("Alternate", "Alternate")]
    [InlineData("Shell%2FAlternate", "Shell|Alternate")]
    public void PreservesNavigatedAliasesForViewsRegisteredUnderMultipleNames(string secondTab, string secondTabName)
    {
        var changes = new List<NavigationRequestContext>();
        using var app = CreateBuilder(prism => prism
            .RegisterTypes(c => c
                .RegisterForNavigation<MockViewA, MockViewAViewModel>("Primary")
                .RegisterForNavigation<MockViewA, MockViewAViewModel>("Alternate")
                .RegisterForNavigation<PrismNavigationPage>("Shell")
                .RegisterForNavigation<TabbedPage>("Tabs"))
            .AddGlobalNavigationObserver(observable => observable
                .Where(context => context.Type == NavigationRequestType.TabChanged)
                .Subscribe(changes.Add))
            .CreateWindow($"Tabs?createTab=Primary&createTab={secondTab}"))
            .Build();
        var tabs = Assert.IsType<TabbedPage>(GetWindow(app).Page);
        Assert.Empty(changes);
        Assert.IsType<MockViewA>(tabs.Children[0]);
        Assert.IsType<MockViewA>(tabs.Children[1] is NavigationPage navigation ? navigation.RootPage : tabs.Children[1]);

        tabs.CurrentPage = tabs.Children[1];
        var change = Assert.Single(changes);
        Assert.Equal("Tabs", change.TabbedPageName);
        Assert.Equal("Primary", change.PreviousTabName);
        Assert.Equal(secondTabName, change.CurrentTabName);

        tabs.CurrentPage = tabs.Children[0];
        Assert.Equal(2, changes.Count);
        Assert.Equal(secondTabName, changes[1].PreviousTabName);
        Assert.Equal("Primary", changes[1].CurrentTabName);
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
            IGlobalNavigationObserver? observer = null;
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
            Assert.Null(requests[0].TabbedPageName);
            Assert.Null(requests[0].PreviousTabName);
            Assert.Null(requests[0].CurrentTabName);

            var first = tabs.Children[0];
            var second = tabs.Children[1];
            Assert.Same(second, tabs.CurrentPage);
            tabs.CurrentPage = first;
            var change = Assert.Single(changes);
            Assert.Single(tabs.Behaviors.OfType<TabbedPageNavigationObserverBehavior>());
            Assert.Equal("TabbedPage", change.TabbedPageName);
            Assert.Equal(selectedTab, change.PreviousTabName);
            Assert.Equal("MockViewA", change.CurrentTabName);
            Assert.Equal(NavigationRequestType.TabChanged, change.Type);
            var selectionResult = Assert.IsAssignableFrom<INavigationResult>(change.Result);
            Assert.True(selectionResult.Success);
            Assert.Null(selectionResult.Exception);
            Assert.False(change.Cancelled);
            Assert.Null(change.Uri);
            Assert.Empty(Assert.IsAssignableFrom<INavigationParameters>(change.Parameters));
            Assert.Equal(2, requests.Count);
            Assert.Equal(requests, otherSubscriber);

            tabs.CurrentPage = first;
            Assert.Single(changes);
            var navigation = Prism.Navigation.Xaml.Navigation.GetNavigationService(first);
            var result = await navigation.SelectTabAsync("MockViewB");
            Assert.True(result.Success);
            Assert.Equal(2, changes.Count);
            Assert.Equal("MockViewA", changes[1].PreviousTabName);
            Assert.Equal(selectedTab, changes[1].CurrentTabName);
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
