# MAUI multi-window public API proposal

**Status: design review. All signatures and usage marked PROPOSED are unimplemented.**
Baseline: [`ds/maui-multi-window` at cf8d4895](https://github.com/PrismLibrary/Prism/commit/cf8d489563b2274a6bf1c0077f3c608127aff6a5).

## Developer model

Keep page navigation on `INavigationService`. Make window creation and closure explicit asynchronous operations on the window manager. A page's navigation and dialog services stay with its owning window when another window activates. An absolute page route replaces that owner's root; opening a window is a separate operation.

Use MAUI `Window` as the explicit handle. No public window context, dispatcher abstraction, or scope tree is proposed.

## Current contracts

CURRENT at the baseline, with comments omitted:

```csharp
namespace Prism.Navigation;

public interface IWindowManager
{
    IReadOnlyList<Window> Windows { get; }
    Window? Current { get; }
    void OpenWindow(Window window);
    void CloseWindow(Window window);
}
```

[`IWindowManagerExtensions`](../../src/Maui/Prism.Maui/Navigation/IWindowManagerExtensions.cs) exposes `GetCurrentNavigationService()` and `GetCurrentDialogService()`. These resolve the current page at lookup time. They do not address a selected window. `Current` falls back to the initial window when no active window is recorded; it is not a strict foreground guarantee. There is no public focus method or asynchronous route-based opening method.

The existing result contract is:

```csharp
public interface INavigationResult
{
    bool Success { get; }
    bool Cancelled { get; }
    Exception? Exception { get; }
    NavigationContext? Context { get; }
}
```

`NavigationContext` is the existing region-navigation context. See [window interface](../../src/Maui/Prism.Maui/Navigation/IWindowManager.cs), [manager](../../src/Maui/Prism.Maui/Navigation/PrismWindowManager.cs), and [result interface](../../src/Prism.Core/Navigation/INavigationResult.cs).

## Proposed interface changes

PROPOSED additions to `IWindowManager`, retaining all current members:

```csharp
Task<IWindowNavigationResult> OpenWindowAsync(
    string route,
    INavigationParameters? parameters = null,
    string? name = null,
    CancellationToken cancellationToken = default);

Task<INavigationResult> CloseWindowAsync(
    Window window,
    CancellationToken cancellationToken = default);

Window? FindWindow(string name);
```

PROPOSED new interface in `Prism.Navigation`:

```csharp
public interface IWindowNavigationResult : INavigationResult
{
    Window? Window { get; }
}
```

PROPOSED additions to `IWindowManagerExtensions`:

```csharp
public static INavigationService GetNavigationService(
    this IWindowManager windowManager, Window window);

public static IDialogService GetDialogService(
    this IWindowManager windowManager, Window window);
```

These extensions resolve the selected window's current page service. A foreign, closed, or rootless window is rejected without falling back to `Current`. Keep the existing `GetCurrent...` conveniences. Retain the `Window` and resolve its current service again after root replacement; do not cache the old page's service.

No members are added to `INavigationService`, `IDialogService`, or `INavigationResult`. The new result provides only the opened window handle.

## HOW TO: open and target a window

PROPOSED usage, assuming existing registrations for `NavigationPage`, `Documents`, `Document`, and `Details`:

```csharp
var opened = await windowManager.OpenWindowAsync(
    "/NavigationPage/Document",
    new NavigationParameters { { "documentId", 42 } },
    name: "document-42",
    cancellationToken: cancellationToken);

if (opened.Cancelled)
{
    return;
}
if (!opened.Success)
{
    throw opened.Exception!;
}

var documentWindow = opened.Window!;
var result = await windowManager
    .GetNavigationService(documentWindow)
    .NavigateAsync("Details");
```

Inside a page ViewModel, inject `INavigationService` as usual:

```csharp
// Existing calls stay in the page's owning window.
var forward = await navigationService.NavigateAsync("Details");
var back = await navigationService.GoBackAsync();
var reset = await navigationService.NavigateAsync("/NavigationPage/Documents");
```

An application service can explicitly select the initial window (PROPOSED lookup):

```csharp
var primary = windowManager.FindWindow("primary")
    ?? throw new InvalidOperationException("The primary window is closed.");
var result = await windowManager
    .GetNavigationService(primary)
    .NavigateAsync("Details");
var closed = await windowManager.CloseWindowAsync(documentWindow, cancellationToken);
```

Inspect every operation's result. The string navigation and parameterless Back calls are [existing extensions](../../src/Maui/Prism.Maui/Navigation/INavigationServiceExtensions.cs); window lookup/open/close and explicit-window service lookup are proposed.

## Opening, cancellation, and closure

- `OpenWindowAsync` requires a root route beginning with `/` and always creates a new window. It never targets an existing name or replaces the caller's root. Empty routes, duplicate names, unsupported platforms, and unknown registrations fail without changing other windows.
- Success requires an initialized managed root and successful native-window creation. It does not promise focus. `Window` is non-null only on success. Failed or cancelled opening releases newly allocated resources and its address reservation.
- Success means `Success = true, Cancelled = false`. Cancellation means `Success = false, Cancelled = true`. Other failures mean `Success = false, Cancelled = false` with a useful exception. Cancellation may carry a cause in `Exception`; inspect `Cancelled` first. `Context` is null for these MAUI operations.
- Honor tokens while queued, during cancellable setup, and before native commit. After an irreversible native transition, finish reconciliation and report the committed outcome. Existing `INavigationService` has no token overloads; this proposal does not claim to cancel an ongoing `NavigateAsync` through a wrapper.
- `CloseWindowAsync` confirms departing content through Prism's existing confirmation model, closes only the specified owner, and completes after managed teardown. A veto returns cancellation and preserves the window. An already closed window owned by the manager may return success; a foreign window fails.
- Closure stops new work for that owner, settles pending work, dismisses its dialogs, destroys owned pages once, releases cached roots and scopes, and removes live tracking. Native destruction must have the same idempotent outcome. OS termination cannot guarantee asynchronous confirmation or cleanup.

Review decision: can all retained pages veto closure, or only active content? Recommend the active page and visible dialogs in a documented order; do not infer a veto from every cached page.

## Stable window identity

PROPOSED addressing rules:

- Reserve logical name `primary` for the initial window. Do not introduce a competing `default` alias.
- Keep existing `PrismWindow.DefaultWindowName = "__PrismRootWindow"` unchanged for source compatibility. Map the initial window to `primary`; do not repurpose the constant.
- Explicit names are case-insensitive, nonempty URI-safe ASCII identifiers: a letter followed by letters, digits, or hyphens. Recommend lowercase. Generate an address when `name` is omitted.
- Names are stable for the app session, independent of title and activation. Reserve successfully used names until session end so stale addresses cannot target replacement windows. A closed name resolves to null; failed opening releases its reservation.
- `primary` never migrates when the initial window closes. Persistence across process restarts requires a separate restoration policy.

The public [`PrismWindow.Name` and constructor default](../../src/Maui/Prism.Maui/Navigation/PrismWindow.cs) already exist; uniqueness, aliases, and reservation rules are proposals. No separate public ID is required initially: the immutable logical name is the session address, and `Window` is the in-process handle.

Page services remain pinned to their window. Back, absolute navigation, dialogs, and lifecycle callbacks cannot cross to another active window. Detached/disposed page services fail rather than retarget. Shared application singletons remain possible; pending work and page lifetime belong to the owner.

## Evaluate `window://` syntax

### Current URI semantics

[`UriParsingHelper`](../../src/Prism.Core/Common/UriParsingHelper.cs) converts `/NavigationPage/Documents` to absolute `http://localhost/...`. It reads `PathAndQuery`, splits on `/`, removes empty segments, then unescapes each segment. Scheme and authority do not select a window. [`PageNavigationService`](../../src/Maui/Prism.Maui/Navigation/PageNavigationService.cs) selects absolute navigation using `uri.IsAbsoluteUri`.

| Input | Current parser interpretation |
| --- | --- |
| `Details` | Relative page navigation |
| `/NavigationPage/Documents` | Absolute/root replacement |
| `window://editor/NavigationPage/Document` | Absolute page route `NavigationPage/Document`; `editor` is ignored for addressing |
| `window://new/NavigationPage/Document` | Same page route; does not command window creation |
| `//home/details` | Absolute route with segments `home`, `details`; double slash has no separate segment meaning |

Therefore passing `window://editor/...` to current navigation may replace the caller's owning root; it does not target `editor`. A pathless URI is not a creation command. These conclusions follow from source and a .NET URI parsing check, not execution of a proposed scheme. [Existing parser tests](../../tests/Maui/Prism.Maui.Tests/Fixtures/Common/UriParsingHelperFixture.cs) characterize the deep-link behavior.

### Candidate grammar

UNIMPLEMENTED alternative, invalid as window addressing on the current branch:

```text
window://new/editor/NavigationPage/Document?documentId=42
window://existing/editor/navigate/Details
window://existing/editor/reset/NavigationPage/Documents
window://existing/primary/navigate/Details
```

Authority is an operation category (`new` or `existing`); the first path component is a name. `navigate` means relative page navigation in that window; `reset` means root replacement there. Creation with a used name fails. Targeting a missing name fails. Neither operation falls back to the active window or silently performs the other operation. Query values are page parameters; object parameters remain out of band in `INavigationParameters`.

`window://editor/Details` is shorter but leaves creation versus lookup and relative versus root navigation ambiguous. A future opt-in parser must preserve nested route semantics, validate escaping and names, and reject credentials, ports, fragments, or unknown operations before generic absolute dispatch. External deep links must not create/reset windows merely because they contain this scheme; application-authorized dispatch is a separate boundary.

Recommend approving the explicit service contract first and deferring scheme syntax to a thin opt-in adapter. `INavigationResult` has no window handle, so a future scheme exposed through `NavigateAsync` must define result handling or require an explicit name for subsequent lookup. Do not change every page call's result type for this convenience.

## Compatibility and decisions

Adding abstract members to `IWindowManager` breaks third-party implementations and test doubles. Use the direct-interface shape only in a deliberate breaking release. For an additive release, place the same three members on PROPOSED `IAsyncWindowManager : IWindowManager`, registering the built-in manager as both. The new result and explicit-window extensions remain additive. Choose one release model.

Retain synchronous `OpenWindow(Window)` and `CloseWindow(Window)` as low-level compatibility APIs without awaitable result or cancellation guarantees. Synchronous closure is not evidence that teardown has completed. Existing single-window startup and page-navigation strings remain valid.

Decisions before implementation:

1. Breaking `IWindowManager` expansion or additive `IAsyncWindowManager`?
2. Confirm `primary`, session reservations, and handling of manually supplied windows.
3. Confirm close-veto order and native close completion on each supported platform.
4. Defer `window://`, or approve its precise opt-in grammar and result policy?

## Evidence and acceptance criteria

Baseline [navigation cases](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/MultiWindowNavigationFixture.cs) cover secondary root replacement, owner-correct Back, and detached absolute navigation before scope allocation. [Manager cases](../../tests/Maui/Prism.Maui.Tests/Fixtures/Navigation/MultiWindowManagerFixture.cs) cover activation and stale-reference removal. These existing regression cases do not establish the proposed APIs.

The proposal remains unimplemented. Current navigation synchronization/source state is static; creation/restoration reuses the initial window; close lacks the proposed lifetime guarantees; [dialog lookup](../../src/Maui/Prism.Maui/Dialogs/DialogService.cs) still falls back to the active window after detachment.

Acceptance must cover two-window opening/closing, activation during awaits, veto/cancellation, duplicate and stale names, failed root construction, native close/restoration, dialog ownership, and exactly-once disposal with Microsoft DI. Platform and NativeAOT runtime qualification remains required. No builds or device tests were run for this documentation proposal.
