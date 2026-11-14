using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Prism.Ioc;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public static class SmokeAdapterChecks
{
    public static void RegisterTypes(IContainerRegistry registry)
    {
        registry.RegisterForNavigation<AdapterFirstView>();
        registry.RegisterForNavigation<AdapterSecondView>();
    }

    public static async Task RunAsync(IContainerProvider container, Window window, SmokeState state)
    {
        var shell = (SmokeShell)window.Content;
        var fixture = new AdapterFixture();
        var manager = container.Resolve<IRegionManager>().CreateRegionManager();
        RegionManager.SetRegionManager(fixture, manager);
        Grid.SetRowSpan(fixture, 4);
        shell.Children.Add(fixture);
        try
        {
            await WaitUntilAsync(() => fixture.IsLoaded);
            RegionManager.UpdateRegions();
            await WaitUntilAsync(() => manager.Regions.Count() == 3);
            var items = manager.Regions["AdapterItems"];
            state.Check(items.Views.Contains(fixture.OriginalItem) && fixture.Items.Items.Contains(fixture.OriginalItem),
                "ItemsControl adapter preserves its existing XAML item.");
            object item = "Adapter-added item";
            items.Add(item);
            await WaitUntilAsync(() => fixture.Items.Items.Contains(item));
            state.Check(items.ActiveViews.Contains(item), "ItemsControl treats added views as active.");
            object lastItem = "Zulu adapter item";
            items.SortComparison = (left, right) => StringComparer.Ordinal.Compare(left.ToString(), right.ToString());
            items.Add(lastItem);
            await WaitUntilAsync(() => items.Views.SequenceEqual(fixture.Items.Items.Cast<object>()));
            var ascending = fixture.Items.Items.Cast<object>().ToArray();
            state.Check(ascending.Length == 3 && Equals(ascending[^1], lastItem),
                "ItemsControl insertion follows the region's sorted view order.");
            items.SortComparison = (left, right) => StringComparer.Ordinal.Compare(right.ToString(), left.ToString());
            await WaitUntilAsync(() => items.Views.SequenceEqual(fixture.Items.Items.Cast<object>()));
            state.Check(!ascending.SequenceEqual(fixture.Items.Items.Cast<object>()) &&
                Equals(fixture.Items.Items[0], lastItem),
                "Changing region sort order resets the native ItemsControl projection.");
            items.Remove(lastItem);
            items.Remove(item);
            await WaitUntilAsync(() => !fixture.Items.Items.Contains(item));
            state.Check(!items.Views.Contains(item) && !items.ActiveViews.Contains(item),
                "ItemsControl removal updates both its items and region membership.");

            var selection = manager.Regions["AdapterSelection"];
            object first = "First selection item";
            object second = "Second selection item";
            selection.Add(first);
            selection.Add(second);
            await WaitUntilAsync(() => fixture.Selection.Items.Count == 2);
            fixture.Selection.SelectedItem = first;
            await WaitUntilAsync(() => selection.ActiveViews.Contains(first));
            state.Check(selection.ActiveViews.Count() == 1, "Selector selection activates exactly the selected view.");
            selection.Activate(second);
            await WaitUntilAsync(() => Equals(fixture.Selection.SelectedItem, second));
            state.Check(!selection.ActiveViews.Contains(first) && selection.ActiveViews.Contains(second),
                "Region activation updates Selector selection and deactivates its prior view.");
            fixture.Selection.SelectedItem = null;
            await WaitUntilAsync(() => !selection.ActiveViews.Any());
            state.Check(!selection.ActiveViews.Any(), "Clearing Selector selection deactivates the region view.");
            selection.Remove(second);
            await WaitUntilAsync(() => !fixture.Selection.Items.Contains(second));
            state.Check(fixture.Selection.Items.Contains(first), "Selector removal preserves its other registered item.");

            var navigation = manager.Regions["AdapterNavigation"];
            fixture.Navigation.SelectedItem = fixture.First;
            await WaitUntilAsync(() => fixture.Navigation.Content is AdapterFirstView);
            var firstView = fixture.Navigation.Content;
            state.Check(navigation.ActiveViews.Contains(firstView),
                "NavigationView selection navigates to its tagged view and synchronizes content.");
            fixture.Navigation.SelectedItem = fixture.Second;
            await WaitUntilAsync(() => fixture.Navigation.Content is AdapterSecondView);
            state.Check(navigation.NavigationService.Journal.CanGoBack,
                "NavigationView selection records history in the region journal.");

            fixture.Navigation.ApplyTemplate();
            var backButton = Descendants(fixture.Navigation).OfType<Button>()
                .FirstOrDefault(button => button.Name == "NavigationViewBackButton" ||
                    AutomationProperties.GetAutomationId(button) == "NavigationViewBackButton");
            if (backButton is null)
                throw new InvalidOperationException("The native NavigationView back button was not found; BackRequested coverage could not run.");
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(backButton) ?? new ButtonAutomationPeer(backButton);
            if (peer.GetPattern(PatternInterface.Invoke) is not IInvokeProvider invoke)
                throw new InvalidOperationException("The native back button does not expose its Invoke pattern.");
            invoke.Invoke();
            await WaitUntilAsync(() => ReferenceEquals(fixture.Navigation.Content, firstView));
            state.Check(navigation.NavigationService.Journal.CanGoForward,
                "Invoking the native NavigationView back button raises BackRequested and restores the previous view.");
        }
        finally
        {
            shell.Children.Remove(fixture);
            foreach (var region in manager.Regions.ToArray())
                manager.Regions.Remove(region.Name);
        }
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            yield return child;
            foreach (var descendant in Descendants(child))
                yield return descendant;
        }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var watch = Stopwatch.StartNew();
        while (!condition())
        {
            if (watch.Elapsed > TimeSpan.FromSeconds(10))
                throw new TimeoutException("Native adapter state did not reach the expected condition.");
            await Task.Delay(25);
        }
    }
}
