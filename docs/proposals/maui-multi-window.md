# MAUI multi-window public API proposal

**Status: design review. All proposed signatures, URI syntax, and behavior are unimplemented.**
Source baseline: [`ds/maui-multi-window` at cf8d4895](https://github.com/PrismLibrary/Prism/commit/cf8d489563b2274a6bf1c0077f3c608127aff6a5).

## Developer model

Use injected `INavigationService` for ordinary and window-directed navigation. Existing `IWindowManager` manages native windows behind those requests. Applications address stable logical names. Proposed contracts do not accept or return MAUI `Window`, a native handle, or a wrapper around one.

Relative navigation, Back, dialogs, and root replacement remain in the injected page service's owning window regardless of activation. An explicit window URI selects another owner for that request only; it does not rebind the calling service.

## Current contracts

CURRENT [`INavigationService`](../../src/Maui/Prism.Maui/Navigation/INavigationService.cs) includes:

```csharp
Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters);
Task<INavigationResult> GoBackAsync(INavigationParameters parameters);
string GetNavigationUriPath();
```

Other members cover named Back, Back to root, named-source navigation, and tab selection. Existing [string extensions](../../src/Maui/Prism.Maui/Navigation/INavigationServiceExtensions.cs) normalize routes before calling the URI overload.

Existing [`IWindowManager`](../../src/Maui/Prism.Maui/Navigation/IWindowManager.cs) already exposes MAUI windows through `Windows`, `Current`, `OpenWindow`, and `CloseWindow`. These compatibility APIs are not a model to expand. No new window lookup result, window result interface, or explicit-window service extension is proposed. Removing existing MAUI types would be a separate breaking change.

Keep existing [`INavigationResult`](../../src/Prism.Core/Navigation/INavigationResult.cs):

```csharp
public interface INavigationResult
{
    bool Success { get; }
    bool Cancelled { get; }
    Exception? Exception { get; }
    NavigationContext? Context { get; }
}
```

`Context` is the existing region context, not a window handle.

## Proposed named-window URI grammar

Candidate for review:

```text
window://{windowName}/{rootRoute}
window://{windowName}/#{relativeRoute}
```

| Proposed request | Meaning |
| --- | --- |
| `window://editor/NavigationPage/Document?documentId=42` | Replace the existing editor window's root using `/NavigationPage/Document?documentId=42` |
| `window://editor/#Details?id=42` | Navigate `Details?id=42` relative to editor's active current page |
| `window://editor/#../` | Back-relative navigation within editor |
| `window://primary/NavigationPage/Documents` | Replace only the initial window's root |

Authority is the logical name. Path navigation is root-relative within that owner; fragment navigation is relative. The explicit `/#` distinction prevents the absolute outer URI from forcing a root replacement for a relative request.

Require a name and exactly one nonempty route. Reject credentials, ports, mixed path/fragment instructions, `window://editor/`, `window://editor/#`, and `window://editor/Document#Details`. Fragment form requires exactly `/` as the outer path and no outer query; its query goes inside the fragment (`#Details?id=42`). Root form uses the outer path/query. Escape literal parameter `#` as `%23`.

Parse this envelope before ordinary URI dispatch. Preserve nested escaping, split route structure before decoding values once, and retain out-of-band objects in `INavigationParameters`. Names are not titles or focus instructions. Manager controls must not be hidden page query parameters.

## Creation versus targeting

Recommend strict targeting through existing `NavigateAsync`: a missing or closed name fails rather than silently creating a replacement. Explicit creation intent belongs on navigation. This candidate adds a mode overload:

```csharp
// PROPOSED in Prism.Navigation.
public enum WindowNavigationMode
{
    Existing,
    CreateNew
}

// PROPOSED INavigationService member.
Task<INavigationResult> NavigateAsync(
    Uri uri,
    INavigationParameters parameters,
    WindowNavigationMode windowMode);

// PROPOSED string extension.
public static Task<INavigationResult> NavigateAsync(
    this INavigationService navigationService,
    string route,
    INavigationParameters parameters,
    WindowNavigationMode windowMode);
```

`Existing` matches the proposed scheme behavior of the current overload. `CreateNew` requires `window://name/rootRoute`; it rejects fragment routes, reserved names, and ordinary page URIs. It always creates a new owner through the existing manager and returns ordinary `INavigationResult`.

This is a review candidate, not an approved signature. An additive extension with internal dispatch could avoid expanding the interface; a dedicated `CreateWindowAsync` extension on navigation is another alternative. Either needs explicit intent and the same results. Creating automatically when a name is missing is shorter but can reopen stale addresses; it is not the recommendation.

## HOW TO: create and target

PROPOSED usage with existing page registrations:

```csharp
var opened = await navigationService.NavigateAsync(
    "window://document-42/NavigationPage/Document",
    new NavigationParameters { { "documentId", 42 } },
    WindowNavigationMode.CreateNew);

if (opened.Cancelled)
{
    return;
}
if (!opened.Success)
{
    throw opened.Exception!;
}

var details = await navigationService.NavigateAsync(
    "window://document-42/#Details");
var reset = await navigationService.NavigateAsync(
    "window://document-42/NavigationPage/Documents");
var primary = await navigationService.NavigateAsync("window://primary/#Details");
```

Within a page ViewModel, ordinary calls keep their owner:

```csharp
var forward = await navigationService.NavigateAsync("Details");
var back = await navigationService.GoBackAsync();
var reset = await navigationService.NavigateAsync("/NavigationPage/Documents");
```

Inspect every result. The supplied name is the address after successful creation; no window handle is returned. Subsequent ordinary calls still belong to the calling service. `GetNavigationUriPath()` continues describing that page without parameters; window-qualified path reporting is an open question.

## Naming, results, and lifetime

- Reserve `primary` for the initial window. Keep existing [`PrismWindow.DefaultWindowName`](../../src/Maui/Prism.Maui/Navigation/PrismWindow.cs), `__PrismRootWindow`, unchanged and map the alias internally. `primary` never migrates after closure.
- Require case-insensitive URI-safe ASCII names: a letter followed by letters, digits, or hyphens. Recommend lowercase. Creation requires an explicit name to avoid needing a new result type.
- Names stay stable for the app session, independent of title/activation. Reserve successful names until session end to prevent stale addresses targeting replacements. Failed creation releases its reservation. Cross-process restoration needs a separate policy.
- Success means `Success = true, Cancelled = false`. Veto/cancellation means `Success = false, Cancelled = true`. Other failures carry useful exceptions with `Cancelled = false`; `Context` is null for MAUI operations.
- Creation succeeds only after an initialized root and successful native creation; focus is not guaranteed. Unsupported platforms, bad routes, duplicate names, and construction failures leave other owners unchanged and clean up new resources.
- Confirm the actual departing content in the selected owner for root/relative navigation. Do not confirm the caller's page in another window. A target closed while queued fails without retargeting.
- No token overload is proposed here. Use existing confirmation/native cancellation outcomes. After irreversible native commit, reconcile and report the committed outcome rather than claim rollback.
- Ordinary calls from detached/disposed page services fail without choosing the active window. Decide whether an app-level service can dispatch explicit window URIs without a page. Startup still establishes the initial owner.

Closure remains the existing manager's responsibility. A future name-based close API may avoid native handles, but this revision proposes no close signature or URI command. Closure must settle owner work, dismiss its dialogs, destroy pages once, and release lifetimes. Native destruction needs the same idempotent outcome; OS termination cannot guarantee async cleanup. Close veto/completion are open decisions.

## Existing parser incompatibility

[`UriParsingHelper`](../../src/Prism.Core/Common/UriParsingHelper.cs) converts leading `/` to absolute `http://localhost/...`, reads only `PathAndQuery`, splits `/`, removes empty segments, and unescapes segments. [`PageNavigationService`](../../src/Maui/Prism.Maui/Navigation/PageNavigationService.cs) dispatches on `IsAbsoluteUri`.

Today `window://editor/NavigationPage/Document` yields absolute page segments `NavigationPage/Document`; editor is not a target. `window://editor/#Details` does not yield a Details segment because the fragment is outside `PathAndQuery`. Neither has the proposed meaning. The envelope must be handled before generic dispatch; retain existing deep-link authority behavior for other schemes. See [parser cases](../../tests/Maui/Prism.Maui.Tests/Fixtures/Common/UriParsingHelperFixture.cs).

The earlier operation-authority syntax (`window://new/name/...`) is superseded by named authority. Explicit creation remains an API decision.

## Compatibility and review decisions

Keep ordinary startup, page strings, and existing manager APIs. Adding an abstract navigation member breaks custom implementations/test doubles; choose a breaking release or additive extension/internal dispatch. No new public contract exposes native windows.

Reserving `window://` may conflict with existing app deep links; define opt-in compatibility. External links must not create/reset windows without app-authorized dispatch.

Decisions before implementation:

1. Approve path-for-root and fragment-for-relative syntax and nested escaping.
2. Choose explicit creation overload or extension, retaining precise missing-name behavior.
3. Confirm primary naming, reservations, and manually supplied window names.
4. Define app-level/detached-page dispatch and qualified path reporting.
5. Define name-based closure, veto order, and native completion separately.

## Evidence and acceptance

Existing [navigation cases](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/MultiWindowNavigationFixture.cs) cover owner-correct replacement/Back and detached absolute navigation before allocation. [Manager cases](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/MultiWindowManagerFixture.cs) cover activation/stale references. They do not establish this scheme or overload.

Current synchronization/source state remains static, creation/restoration reuses the initial window, close lacks lifetime guarantees, and [dialog lookup](../../src/Maui/Prism.Maui/Dialogs/DialogService.cs) still falls back after detachment. Acceptance requires two owners, queued closure, activation during awaits, fragments/escaping, creation versus targeting, cancellation, stale names, failed construction, dialogs, and exactly-once disposal with Microsoft DI. Native/platform and NativeAOT qualification remains required. This revision is documentation only.
