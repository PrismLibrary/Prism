# Native WinUI 3

`Prism.WinUI` and `Prism.DryIoc.WinUI` target native Windows App SDK applications
(`net10.0-windows10.0.19041.0`). Neither uses Uno.Sdk, Uno Extensions, or references
an Uno package/project. Shared source is linked from the WPF and Uno implementations;
UI metadata remains in the UI assembly rather than Prism.Core.

## Application startup

Derive your App from `Prism.DryIoc.PrismApplication`, using
`xmlns:prism="using:Prism.DryIoc"` in App.xaml. Implement `RegisterTypes` and
`CreateShell` (return a `UIElement`, typically a Page or UserControl).

The default `CreateWindow` creates a native `Microsoft.UI.Xaml.Window`. Override it
for a custom window and use `ConfigureWindow` for title, sizing, and other setup.
`MainWindow` and the container's registered `Window` identify the primary window;
the default dialog service uses that window's content/XamlRoot. There is no
ambient `Window.Current` or implicit multi-window routing.

Startup configures view-model creation and the container, resolves the event
aggregator on the UI thread, configures modules and regions, and attaches the
shell. After the shell's Loaded dispatch finishes, Prism updates regions,
initializes modules, calls `OnInitialized`, calls `ILoadableShell.FinishLoading`,
and marks the shell and its view model active. Navigate from `OnInitialized`
when you need the shell's regions to be available.

There is deliberately no `Host`, `ConfigureHost`, `ConfigureApp`, or Uno
`IApplicationBuilder`. Register application services using `RegisterTypes`.
Native markup helpers and NativeAOT support are not part of this addition.

## Validation

On Windows with the .NET 10 SDK and Windows SDK installed, and the repository's
Prism container package feed configured, run:

```powershell
powershell -File .\build\Validate-NativeWinUI.ps1
```

The script builds Release/x64, inspects both restored dependency closures for
Uno packages, packs both libraries, and checks their nuspec dependencies. See
`e2e/WinUI/NativeWinUISmoke` for runtime lifecycle, navigation, UI dispatch, and
dialog smoke checks. A successful package build does not substitute for running
that native UI application on Windows.
