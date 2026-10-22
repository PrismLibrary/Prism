# Native WinUI 3

`Prism.WinUI` and `Prism.DryIoc.WinUI` target native Windows App SDK applications
(`net10.0-windows10.0.19041.0`). Neither uses Uno.Sdk, Uno Extensions, or references
an Uno package/project. Shared source is linked from the WPF and Uno implementations;
UI metadata remains in the UI assembly rather than Prism.Core.

The native projects and playground use the current stable packages verified on
October 21, 2026: [Windows App SDK 2.5.1](https://www.nuget.org/packages/Microsoft.WindowsAppSDK/2.5.1),
[C#/WinRT 2.3.1](https://www.nuget.org/packages/Microsoft.Windows.CsWinRT/2.3.1),
and [WinUI managed behaviors 3.0.1](https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.WinUI.Managed/3.0.1).
These versions are centrally managed only for native WinUI projects; existing Uno
targets retain their own package settings.

## Shared implementation and remaining adapters

The native package links the same WPF/Uno view registry, view-model metadata,
default service/region registration, design-mode helper and navigation-view adapter.
ContentDialog host selection, placement parsing and presentation properties are also
shared with Uno; owner-specific selection and cancellation stay in the lifecycle adapters.
`Prism.DryIoc.WinUI` has no independent C# implementation: its application class is
linked from the same WPF source used by Uno. `RegionBehaviorFactory` in Prism.Core
owns behavior registrations for all platforms; its explicit-provider overload lets
the native region adapter resolve behaviors through the owning window without
copying the factory or putting any XAML/window types in Core.

The remaining native files are **not yet** a PrismApplicationBase-only layout:

- `PrismApplicationBase` owns Windows App SDK startup rather than Uno Hosting.
- `Windows/IWindowContext`, `IWindowManager`, `WindowContext` and `WindowManager`
  expose native WinUI windows and implement dispatcher access, loaded-shell startup,
  `Window.Closed` disposal and dialog cancellation. WPF's window/event types differ;
  existing Uno startup registers a singleton window and has no equivalent scoped
  window-manager contract. Moving these files alone would not add Uno support.
- `Navigation/Regions/NativeRegionScope` enforces native window-provider ownership,
  retained-view rejection and per-window region cleanup. Uno/WPF currently retain
  their existing region-provider defaults; enabling this policy there is behavioral
  work, not just source linking.
- `Dialogs/DialogService` is bound to that explicit context and serializes/cancels
  dialogs with its owner. Uno's existing service resolves its singleton window;
  WPF uses modal/nonmodal Window APIs rather than ContentDialog/XamlRoot. Their
  constructors and lifecycle defaults must not be changed by a source-only refactor.
- `Properties/NativeResources.*` localizes those native lifetime diagnostics.

Shared source uses narrowly guarded native ownership hooks (`WINUI && !UNO_WINUI`)
so the existing Uno/WPF defaults and public signatures remain intact. Cross-platform
multi-window activation and Windows runtime acceptance remain tracked in
[issue #3451](https://github.com/PrismLibrary/Prism/issues/3451).

## Application startup

Derive your App from `Prism.DryIoc.PrismApplication`, using
`xmlns:prism="using:Prism.DryIoc"` in App.xaml. Implement `RegisterTypes` and
`CreateShell(IContainerProvider container)` (return a `UIElement`, typically a Page
or UserControl, resolved using that window-scoped provider).

The default `CreateWindow` creates a native `Microsoft.UI.Xaml.Window`. Override it
for a custom initial window and use `ConfigureWindow` for title, sizing, and setup.
There is no globally registered `Window`, `MainWindow`, ambient active window, or
implicit focus-based dialog routing.

Startup registers shared application services, captures the UI synchronization
context for the shared EventAggregator, configures modules and regions, and opens
the initial window. After the shell's Loaded dispatch finishes, Prism updates its
regions, initializes the application's modules once, calls
`OnInitialized(IWindowContext window)`, calls `ILoadableShell.FinishLoading`, and
marks the shell and its view model active. Navigate using `window.RegionManager`
from that callback. Module initialization registers shared services and views;
window-specific initial navigation belongs in the window callback, not a module's
application-wide `OnInitialized`.

## Window ownership and dialogs

`Prism.Windows.IWindowManager` is a shared service, also exposed as the application's
`Windows` property. Call `OpenWindow(window, scope => scope.Resolve<Shell>(),
context => { /* Initial navigation */ })` to create another independently scoped
window. Opening another window does not run modules again. `Windows` provides a
snapshot of open contexts and `GetWindow(window)` selects an exact target.

Each `IWindowContext` exposes its `Window`, `Container`, `RegionManager`, and
`IsClosed`. Every shell, navigation view, view model, region manager, and dialog
service resolves through its owning window's scope. App-level singleton services,
the module catalog, view registrations, and the EventAggregator remain shared.
Window-owned services resolved from the application container are rejected rather
than silently using the latest container scope. Keep window-dependent view models
transient or scoped; application singletons must not capture window-owned services.

An injected `IDialogService` remains bound to its originating window even when
another window receives focus. For intentional cross-window dialogs, resolve the
service through the selected target: `target.Container.Resolve<IDialogService>()`.
The service reads the current `Window.Content.XamlRoot` immediately before showing,
rejects missing/unloaded content, serializes concurrent dialogs within one window,
and allows independent windows to show dialogs concurrently. Custom dialog hosts
must derive from `ContentDialog`. Failure and owner-close cancellation are returned
through `DialogCallback`/`IDialogResult.Exception`, once per request.

`Window.Closed` is the lifetime boundary: queued/active dialogs are canceled,
handlers and regions are detached, and the window scope is disposed exactly once.
Transient shell `Unloaded` events do not dispose the scope. Context service access
and retained dialog-service calls fail after closure. Failed window startup closes
the window and releases its scope. Retained raw providers or scoped service
references must not be used after their owner's closure.

All managed windows, navigation, and dialog calls must use the original WinUI UI
thread. Multi-dispatcher windows are not supported. Cross-thread publishers can
still use the shared EventAggregator and its existing UI-thread subscription
option; no additional dispatcher or global event routing is introduced.

There is deliberately no `Host`, `ConfigureHost`, `ConfigureApp`, or Uno
`IApplicationBuilder`. Register application services using `RegisterTypes`.
Native markup helpers and NativeAOT support are not part of this addition.

## API playground

Open `e2e/WinUI/HelloWorld.slnx` for the manual API exercise application.
It follows the existing HelloWorld / Playground.Module structure, with native
WinUI region, navigation, journal, dialog, and modularity examples.

## Build and test

On Windows with the .NET 10 SDK, Windows SDK / WinUI build tools, and the
repository's Prism container package feed configured, build
`PrismLibrary_WinUI.slnf` using the normal solution/MSBuild workflow:

```powershell
msbuild .\PrismLibrary_WinUI.slnf -restore -p:Configuration=Release -p:Platform=x64
```

Use `e2e/WinUI/HelloWorld.slnx` for the manual playground. See
`tests/WinUI/NativeWinUISmoke` for the focused Windows runtime checks and their
build/run instructions. A successful managed-source check or package restore
does not establish native XAML compilation, packaging, or Windows UI behavior.
