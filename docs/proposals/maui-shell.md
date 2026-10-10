# MAUI Shell public API proposal

**Status: design review. All signatures and behavior marked PROPOSED are unimplemented.**
Baseline: [`ds/maui-shell` at e06b8ee8](https://github.com/PrismLibrary/Prism/commit/e06b8ee8b93d4cd9e7c739241b8e0258cada438e). This branch contains test prototypes, not a production Shell backend.

## Developer model

Make Shell an explicit application opt-in. Reuse Prism page/ViewModel registrations, injected `INavigationService`, lifecycle interfaces, and `INavigationResult`. Add three helpers: select a Shell host, map detail routes to Prism registrations, and create scoped root templates.

Shell owns its visual hierarchy and stacks. Prism supplies page construction, ViewModel wiring, confirmation, navigation callbacks, and disposal. Applications do not implement factories or manage page scopes. The initial proposal supports an explicit navigation subset; unsupported operations fail before mutation.

## Current public contracts

CURRENT at the baseline, with comments omitted:

```csharp
namespace Prism.Navigation;

public interface INavigationService
{
    string GetNavigationUriPath();
    Task<INavigationResult> GoBackAsync(INavigationParameters parameters);
    Task<INavigationResult> GoBackToAsync(string viewName, INavigationParameters parameters);
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters);
    Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters);
    Task<INavigationResult> NavigateFromAsync(
        string viewName, Uri uri, INavigationParameters parameters);
    Task<INavigationResult> SelectTabAsync(
        string name, Uri uri, INavigationParameters parameters);
}

public interface INavigationResult
{
    bool Success { get; }
    bool Cancelled { get; }
    Exception? Exception { get; }
    NavigationContext? Context { get; }
}
```

`Context` is the existing region-navigation context. `Cancelled` already exists. See [navigation interface](../../src/Maui/Prism.Maui/Navigation/INavigationService.cs) and [result interface](../../src/Prism.Core/Navigation/INavigationResult.cs).

Existing integration declarations used below:

```csharp
// PrismAppBuilder instance member
public PrismAppBuilder RegisterTypes(Action<IContainerRegistry> registerTypes);

// PrismAppBuilderExtensions
public static PrismAppBuilder CreateWindow(this PrismAppBuilder builder, string uri);

// INavigationServiceExtensions
public static Task<INavigationResult> NavigateAsync(
    this INavigationService navigationService,
    string name, INavigationParameters parameters);
public static Task<INavigationResult> GoBackAsync(
    this INavigationService navigationService);
```

Existing `RegisterForNavigation<TView, TViewModel>(name)` has `where TView : Page`. Its constructor/property preservation annotations remain relevant. Reuse this registry. See [builder](../../src/Maui/Prism.Maui/PrismAppBuilder.cs), [startup extensions](../../src/Maui/Prism.Maui/PrismAppBuilderExtensions.cs), [navigation extensions](../../src/Maui/Prism.Maui/Navigation/INavigationServiceExtensions.cs), and [registrations](../../src/Maui/Prism.Maui/Ioc/NavigationRegistrationExtensions.cs).

## Proposed public additions

PROPOSED extension declarations, absent from the baseline. Suggested namespaces: `Prism` for builder extensions and `Prism.Navigation.Xaml` for the template helper. No new interface is required.

```csharp
public static PrismAppBuilder UseShell<
    [DynamicallyAccessedMembers(
        DynamicallyAccessedMemberTypes.PublicConstructors |
        DynamicallyAccessedMemberTypes.NonPublicConstructors)] TShell>(
    this PrismAppBuilder builder)
    where TShell : Microsoft.Maui.Controls.Shell;

public static PrismAppBuilder RegisterShellRoute(
    this PrismAppBuilder builder, string route, string navigationKey);

public static DataTemplate CreatePrismContentTemplate(
    this Microsoft.Maui.Controls.Shell shell, string navigationKey);
```

These are contract declarations, not complete extension-class implementations. Annotations use `System.Diagnostics.CodeAnalysis`; `DataTemplate` is MAUI's type.

- `UseShell<TShell>` resolves the host through the container and selects the Shell navigation backend. Each Shell window gets a fresh host instance and owner-bound services; reject singleton/reused or already attached instances. Existing `CreateWindow` supplies the initial route after host construction/attachment.
- `RegisterShellRoute` maps a detail-route name to an existing Prism registration. Validate unknown keys and destination types before navigation. Initially support `ContentPage`; reject `NavigationPage`, `TabbedPage`, `FlyoutPage`, and Shell as routed content.
- `CreatePrismContentTemplate` creates a lazy root template bound to the supplied Shell. Resolve the owning host on materialization using the detail-route construction rules. Creating the template in the Shell constructor does not require attachment.

A missing/detached owner fails without using `Shell.Current`, another active window, or a global container. Explicit registrations assist trimming but do not establish NativeAOT support.

## HOW TO: configure an opt-in Shell app

PROPOSED usage within an existing `UsePrism` configuration (`Prism.Ioc` supplies registration extensions):

```csharp
prism
    .RegisterTypes(container =>
    {
        container.RegisterForNavigation<HomePage, HomeViewModel>("homePage");
        container.RegisterForNavigation<DetailPage, DetailViewModel>("detailPage");
    })
    .UseShell<AppShell>()
    .RegisterShellRoute("details", "detailPage")
    .CreateWindow("//main/home/start");
```

Give hierarchy routes explicit names so generated wrappers do not become application contracts. The proposed template extension requires `using Prism.Navigation.Xaml;`:

```csharp
public sealed class AppShell : Microsoft.Maui.Controls.Shell
{
    public AppShell()
    {
        var content = new ShellContent
        {
            Route = "start",
            ContentTemplate = this.CreatePrismContentTemplate("homePage")
        };
        var section = new ShellSection { Route = "home" };
        section.Items.Add(content);
        var item = new FlyoutItem { Route = "main" };
        item.Items.Add(section);
        Items.Add(item);
    }
}
```

An equivalent XAML Shell can name its `ShellContent` and assign the helper in its constructor. A plain `DataTemplate(typeof(HomePage))` does not guarantee Prism integration. No custom Shell base class, base page, or ViewModel base class is required.

## HOW TO: navigate, query, and return values

Inject `INavigationService` into page ViewModels. PROPOSED Shell behavior with existing call shapes:

```csharp
var result = await navigationService.NavigateAsync(
    "details?id=42",
    new NavigationParameters { { "item", selectedItem } });
if (result.Cancelled)
{
    return;
}
if (!result.Success)
{
    throw result.Exception!;
}
```

From the detail ViewModel, send a fresh back payload:

```csharp
var back = await navigationService.GoBackAsync(
    new NavigationParameters { { "saved", true } });
var home = await navigationService.NavigateAsync("//main/home/start");
```

Inspect each result. `details` is a Shell route; `detailPage` is its mapped Prism key. Receive `id` and the object through `OnNavigatedTo(INavigationParameters)` using `GetValue<string>("id")` and `GetValue<Item>("item")` (where `Item` is the app's type). The returning page receives `saved` through the same Prism callback with Back navigation mode. It must not receive the old forward `item` payload again. These examples require the proposed backend and lifecycle bridge; they are not runnable on the investigation branch.

## Proposed behavior of existing interfaces

Keep `INavigationService` and `INavigationResult` signatures:

| Operation | Proposed initial Shell contract |
| --- | --- |
| `NavigateAsync` | Explicit `//` hierarchy routes, registered relative detail routes, validated leading back-relative routes; remain in the owning Shell/window |
| `GoBackAsync` | Pop one detail page from the active owning section and deliver back parameters; fail before mutation without a supported target |
| `GoBackToRootAsync` | Pop details to the active section's retained root; preserve other sections and their cached roots |
| `GetNavigationUriPath` | Associated page's absolute hierarchy/detail path, stopping at that page and excluding parameters; empty before attachment; inactive roots/sections retain their own path. A host/startup service without a page may report its owning Shell's current path |
| `GoBackToAsync` | Deferred; unsupported result before mutation |
| `NavigateFromAsync` | Deferred pending a named-source transactional contract; unsupported before mutation |
| `SelectTabAsync` | Deferred; current contract is for `TabbedPage`; use an explicit Shell hierarchy route |

Unsupported results have `Success = false`, `Cancelled = false`, a useful `NotSupportedException`, and null `Context`. Prism container-building URI syntax, implicit modal rules, and cross-window routing are outside this initial contract. Full interface parity requires expanded acceptance criteria.

In the first version, relative navigation and Back from an inactive or covered page fail before mutation rather than act on a selected sibling. An explicit absolute hierarchy route may select another section within the owner Shell, confirming the actual departing content. Path reporting still describes the service's own page. See [current page-path ownership](../../src/Maui/Prism.Maui/Navigation/PageNavigationPath.cs).

### Route normalization

Current string extensions are not transparent Shell pass-through:

- `//main/home/start` becomes `http://localhost//main/home/start`.
- A leading `../` becomes `__RemovePage/` before the service's URI overload.
- The current whole-string `Replace` can also rewrite an unescaped `../` in a query when the route starts with `../`.
- Generic parsing removes empty slash components and unescapes whole segments, losing meaningful `//` syntax.

PROPOSED compatibility behavior: dispatch Shell string calls before ordinary page normalization without changing the classic backend. Accept documented synthetic-localhost and leading remove-token forms from existing URI callers. Preserve `//`, decode URI query values once, and reject arbitrary URL schemes. A real external URL must not become an authorized Shell route merely because its path matches. Already-normalized inputs cannot always recover original query text; escaping and reserved-token collisions require explicit tests.

No public overload is added. `CreateWindow(string)` uses the same helpers, so Shell startup depends on this normalization work. See [string normalization](../../src/Maui/Prism.Maui/Navigation/INavigationServiceExtensions.cs) and [parser](../../src/Prism.Core/Common/UriParsingHelper.cs).

### Parameters, outcomes, and cancellation

Preserve object identity, null values, and literal dictionary strings. Do not serialize or decode `INavigationParameters` strings. Copy the caller's bag into a fresh single-use transfer for each forward/back operation. Reject repeated keys before mutation, including URI-query/bag collisions. This restriction differs from ordinary Prism bags, which support repeated keys.

A completed `Shell.GoToAsync` task does not prove success: cancellation can complete normally. Return `Success = false, Cancelled = true` for a veto or Shell cancellation. Unchanged location alone is not cancellation evidence; navigating to an already-current route can be valid.

Current [`NavigationResult.Cancelled`](../../src/Prism.Core/Navigation/NavigationResult.cs) recognizes only the `IConfirmNavigationReturnedFalse` navigation exception. `new NavigationResult(false)` does not report cancellation. The bridge must map Shell cancellation deliberately while retaining the public interface. Error results carry the originating exception and null `Context`. No navigation token overload is proposed.

### Lifecycle and ownership

Reuse `IInitialize`, `IInitializeAsync`, `IConfirmNavigation`, `IConfirmNavigationAsync`, `INavigatedAware`/`INavigationAware`, and `IDestructible`. Also retain `IPageLifecycleAware.OnAppearing/OnDisappearing` for visibility and `IApplicationLifecycleAware.OnResume/OnSleep` for the owner window's application lifecycle. Do not duplicate these interfaces for Shell.

PROPOSED sequence: validate, confirm departure, construct scoped destination, initialize once, commit navigation, issue committed navigation callbacks, and dispose removed pages. Async confirmation uses a Shell deferral where cancellation is available. A ViewModel can implement the existing signatures:

```csharp
public Task<bool> CanNavigateAsync(INavigationParameters parameters)
{
    return Task.FromResult(!HasUnsavedChanges);
}

public void OnNavigatedTo(INavigationParameters parameters)
{
    if (parameters.ContainsKey("saved"))
    {
        Saved = parameters.GetValue<bool>("saved");
    }
}
```

These are illustrative members of an application ViewModel implementing `IConfirmNavigationAsync` and `INavigatedAware`; `HasUnsavedChanges` and `Saved` are application properties. The callback bridge is proposed.

Retain cached root pages/scopes while Shell retains them. Switching tabs or disappearing is not destruction. Popping a detail releases its scope once. Closing the owner destroys cached roots and remaining details and releases owner state. Root-created DI scopes need explicit lifetime ownership.

Awaiting asynchronous initialization before visibility remains unresolved at synchronous `RouteFactory`/`DataTemplate` boundaries. Startup must safely establish owner context or stage a root that materializes during Shell construction before window attachment. Applications must not supply an `IServiceProvider` to make this work. Current [`PrismAppBuilder.OnCreateWindow`](../../src/Maui/Prism.Maui/PrismAppBuilder.cs) blocks on the startup callback with `.Wait()`; redesign that startup flow before promising safe asynchronous Shell initialization.

Define post-commit callback/disposal failures: a failed result cannot imply rollback of a committed Shell stack. Native Back, swipe, tab/flyout selection, and programmatic navigation need the same outcomes wherever cancellation is exposed.

## Compatibility and review decisions

Apps without `UseShell` retain ordinary Prism navigation. Registration/interface signatures remain source-compatible. Opted-in apps accept Shell syntax, duplicate-key rejection, and deferred operations. Services remain owner-bound rather than tracking the active Shell.

Shell's global route registry must reject unrelated collisions, safely reuse the same Prism mapping across owned hosts, and retain mappings needed by another host. Page materialization uses actual owner services. Applications do not manage registry lifetime.

Shell and multi-window investigations are separate branches; neither establishes combined behavior. Mixed Shell/classic windows, restoration, dialogs, native lifecycle, and NativeAOT require joint qualification.

Decisions before implementation:

1. Approve these three helpers and existing startup configuration, or choose a separate Shell startup helper?
2. Accept the operation subset, or require named Back/source navigation and tab parity?
3. Establish asynchronous initialization timing and post-commit failure results.
4. Confirm duplicate-key rejection and global route lifetime/collision rules.

## Prototype evidence and acceptance

The baseline has 21 Shell investigation tests, not a finished feature's acceptance suite:

- [Construction tests](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/Shell/ShellRouteConstructionFixture.cs) characterize separate scoped pages/ViewModels, failed construction cleanup, and cached templates.
- [Parameter tests](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/Shell/ShellParameterTransferFixture.cs) characterize object/null/literal transfer, repeated-key rejection, and fresh Back parameters.
- [Compatibility tests](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/Shell/ShellCompatibilityFixture.cs) show current traversal rejects Shell, teardown misses cached roots, query delivery omits Prism initialization/navigation callbacks, and cancellation can complete normally or use a deferral.
- [Back test](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/Shell/ShellBackNavigationFixture.cs) characterizes managed push/back transfer while the departing Prism detail scope stays alive and callbacks are absent.

The [internal test factory](../../tests/Maui/Prism.Maui.Tests/Mocks/Shell/ShellRouteFactoryPrototype.cs) leaves confirmation, initialization, callbacks, and disposal to future work. Acceptance additionally requires startup, supported routes, two independent hosts, collision handling, failure/cancellation cleanup, cached-root teardown, native interactions, and NativeAOT. No new test run or production-readiness claim accompanies this document.
