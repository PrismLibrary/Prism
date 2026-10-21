# Native WinUI 3

`Prism.WinUI` and `Prism.DryIoc.WinUI` target native Windows App SDK applications
(`net10.0-windows10.0.19041.0`). Neither uses Uno.Sdk, Uno Extensions, or references
an Uno package/project. Shared source is linked from the WPF and Uno implementations;
UI metadata remains in the UI assembly rather than Prism.Core.

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

## Validation

On Windows with the .NET 10 SDK and Windows SDK installed, and the repository's
Prism container package feed configured, run:

```powershell
powershell -File .\build\Validate-NativeWinUI.ps1
```

The script builds Release/x64, inspects both restored dependency closures for
Uno packages, packs both libraries, and checks their nuspec dependencies. See
`tests/WinUI/NativeWinUISmoke` for runtime lifecycle, navigation, UI dispatch, and
dialog smoke checks. A successful package build does not substitute for running
that native UI application on Windows.
