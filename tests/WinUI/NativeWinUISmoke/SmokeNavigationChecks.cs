using Microsoft.UI.Xaml;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public sealed class SmokeNavigationChecks(IContainerProvider container, Window window, SmokeState state)
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private SmokeShell Shell => (SmokeShell)window.Content;
    private IRegion Region => container.Resolve<IRegionManager>().Regions[SmokeShell.RegionName];
    private SmokeNavigationView Current => (SmokeNavigationView)Shell.RegionHost.Content;
    private static SmokeNavigationViewModel Model(SmokeNavigationView view) => (SmokeNavigationViewModel)view.DataContext;

    public static void RegisterTypes(IContainerRegistry registry) =>
        registry.RegisterForNavigation<SmokeNavigationView, SmokeNavigationViewModel>();

    public async Task RunAsync()
    {
        Region.NavigationService.Journal.Clear();
        var payload = new object();
        await NavigateAsync("first", payload);
        var first = Current;
        var firstModel = Model(first);
        state.Check(firstModel.Key == "first" && firstModel.Query == "encoded value" && ReferenceEquals(firstModel.Payload, payload),
            "Navigation delivers URI query and object parameters to the view model.");
        state.Check(firstModel.IsActive && firstModel.NavigatedToCalls == 1,
            "Navigation activates IActiveAware and calls OnNavigatedTo.");

        await NavigateAsync("second");
        var second = Current;
        var secondModel = Model(second);
        state.Check(!ReferenceEquals(first, second) && !firstModel.IsActive && firstModel.NavigatedFromCalls == 1,
            "IsNavigationTarget false creates a distinct view and deactivates the previous view model.");
        state.Check(Region.Views.Contains(first) && first.DestroyCalls == 0 && firstModel.DestroyCalls == 0,
            "KeepAlive true retains inactive views without destroying them.");

        var journal = Region.NavigationService.Journal;
        state.Check(journal.CanGoBack && !journal.CanGoForward, "Journal records successful navigation.");
        await JournalAsync(journal.GoBack);
        state.Check(ReferenceEquals(Current, first) && firstModel.IsActive && ReferenceEquals(firstModel.Payload, payload),
            "Journal Back reuses the first view and restores object parameters.");
        state.Check(journal.CanGoForward, "Journal Back exposes Forward.");
        await JournalAsync(journal.GoForward);
        state.Check(ReferenceEquals(Current, second) && secondModel.IsActive && !firstModel.IsActive,
            "Journal Forward restores the second view and active state.");

        secondModel.AllowNavigation = false;
        var entry = journal.CurrentEntry;
        var count = Region.Views.Count();
        var fromCalls = secondModel.NavigatedFromCalls;
        var denied = await RequestAsync("denied").WaitAsync(Timeout);
        state.Check(!denied.Success && ReferenceEquals(Current, second) && ReferenceEquals(journal.CurrentEntry, entry) &&
            Region.Views.Count() == count && secondModel.NavigatedFromCalls == fromCalls,
            "Navigation veto preserves active content, journal, views, and OnNavigatedFrom count.");

        secondModel.AllowNavigation = true;
        secondModel.DeferNavigation = true;
        var deferred = RequestAsync("deferred");
        state.Check(!deferred.IsCompleted && secondModel.PendingConfirmation is not null && ReferenceEquals(Current, second),
            "Deferred confirmation leaves navigation pending and current content unchanged.");
        secondModel.CompleteConfirmation(true);
        var resumed = await deferred.WaitAsync(Timeout);
        state.Check(resumed.Success && Model(Current).Key == "deferred", "Deferred confirmation resumes navigation when allowed.");

        var transient = Current;
        var transientModel = Model(transient);
        transientModel.KeepAlive = false;
        await NavigateAsync("first", payload);
        state.Check(ReferenceEquals(Current, first), "IsNavigationTarget true reuses the matching retained view.");
        state.Check(!Region.Views.Contains(transient) && transient.DestroyCalls == 1 && transientModel.DestroyCalls == 1,
            "KeepAlive false removes the view and calls Destroy once on both view and view model.");
        state.Check(!transientModel.IsActive && firstModel.ActivationChanges >= 3,
            "Lifetime removal deactivates the old view model and retained views reactivate.");

        var failure = await RequestTargetAsync("UnregisteredSmokeNavigationTarget", new NavigationParameters()).WaitAsync(Timeout);
        state.Check(!failure.Success && failure.Exception is not null && ReferenceEquals(Current, first),
            "Unregistered navigation target reports an exception without replacing active content.");
        journal.Clear();
        state.Check(!journal.CanGoBack && !journal.CanGoForward && journal.CurrentEntry is null,
            "Journal Clear removes current, back, and forward history.");
        Region.Remove(second);
        state.Check(second.DestroyCalls == 1 && secondModel.DestroyCalls == 1,
            "Explicit removal calls Destroy once on a retained inactive view and view model.");
        state.Check(transient.DestroyCalls == 1 && transientModel.DestroyCalls == 1,
            "Subsequent navigation and removal do not destroy an already removed view again.");
    }

    private async Task NavigateAsync(string key, object? payload = null)
    {
        var result = await RequestAsync(key, payload).WaitAsync(Timeout);
        if (!result.Success) throw new InvalidOperationException($"Navigation to {key} failed.", result.Exception);
    }

    private Task<NavigationResult> RequestAsync(string key, object? payload = null) =>
        RequestTargetAsync($"{nameof(SmokeNavigationView)}?query=encoded%20value",
            new NavigationParameters { { "key", key }, { "payload", payload! } });

    private Task<NavigationResult> RequestTargetAsync(string target, INavigationParameters parameters)
    {
        var completion = new TaskCompletionSource<NavigationResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        Region.NavigationService.RequestNavigate(new Uri(target, UriKind.Relative), result => completion.TrySetResult(result), parameters);
        return completion.Task;
    }

    private async Task JournalAsync(Action navigate)
    {
        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        void Navigated(object? sender, RegionNavigationEventArgs args) => completion.TrySetResult(true);
        Region.NavigationService.Navigated += Navigated;
        try
        {
            navigate();
            await completion.Task.WaitAsync(Timeout);
        }
        finally { Region.NavigationService.Navigated -= Navigated; }
    }
}

