# Native WinUI smoke app

This is a focused, unpackaged, Windows-only WinUI 3 regression harness.
For the interactive API exercise application, use `e2e/WinUI/HelloWorld`.
This harness is not the end-to-end playground. It references
`Prism.DryIoc.WinUI` and the centrally versioned `Microsoft.WindowsAppSDK` directly.
It does not reference Uno, Uno Hosting, Uno Toolkit, or Uno extensions.

## Run on Windows

Prerequisites: Windows 10 2004 (build 19041) or newer, the .NET 10 SDK, and the
Windows SDK / WinUI build tools (for example, Visual Studio with the WinUI
application development workload). Use an interactive desktop, not a headless
Linux runner. Windows App SDK runtime files are included in the output by
`WindowsAppSDKSelfContained`; the .NET 10 runtime is still required.

From the repository root, in PowerShell:

```powershell
dotnet build tests/WinUI/NativeWinUISmoke/NativeWinUISmoke.csproj -c Debug -p:Platform=x64 -r win-x64
$app = 'tests/WinUI/NativeWinUISmoke/bin/x64/Debug/net10.0-windows10.0.19041.0/win-x64/NativeWinUISmoke.exe'
& $app
```

For an ARM64 machine, substitute `ARM64` and `win-arm64`. If your build overrides
output paths, use the executable in the build's reported output directory.

The app automatically checks:

- A native window owns a loaded shell before `OnInitialized`.
- The module initializes and the `ContentControl` region registers before that hook.
- `ILoadableShell.FinishLoading` runs exactly once, after `OnInitialized` returns.
- Navigation activates both the application-registered and module-registered views.
- Consumer XAML resolves `ViewModelLocator.AutowireViewModel` and
  `RegionManager.RegionName` metadata: `HomeView` receives its explicitly registered
  `HomeViewModel`, and its named nested `ContentControl` registers `NestedSmokeContent`.
- The first application-level event aggregator resolution can happen on a worker
  thread, and a background publication still reaches a `ThreadOption.UIThread`
  subscriber on the UI dispatcher.
- Prism's default `ContentDialog` opens with the correct native `XamlRoot`, closes
  via `IDialogAware.RequestClose`, and invokes lifecycle hooks and the result callback.

The dialog appears briefly and closes automatically. The window should end with
`RESULT: PASS` and no `FAIL` lines. Use **Open Prism dialog** to verify an interactive
open/close afterward. Results also appear in the debugger and in
`%LOCALAPPDATA%\PrismNativeWinUISmoke\smoke.log`, replaced on each launch.

For a Windows desktop smoke run that closes itself and returns an exit code:

```powershell
$env:PRISM_WINUI_SMOKE_AUTOCLOSE = '1'
$process = Start-Process $app -PassThru -Wait
Remove-Item Env:PRISM_WINUI_SMOKE_AUTOCLOSE
if ($process.ExitCode -ne 0) { throw "Native WinUI smoke failed: $($process.ExitCode)" }
Get-Content "$env:LOCALAPPDATA\PrismNativeWinUISmoke\smoke.log"
```

Treat a missing `RESULT: PASS`, crash, or launch failure as a failure even if no
log is produced. Navigation, event delivery, and dialog completion each have a
10-second timeout; the operating system must still supply a responsive UI thread.

## Check dependency isolation

```powershell
dotnet list tests/WinUI/NativeWinUISmoke/NativeWinUISmoke.csproj package --include-transitive
```

There should be no `Uno.*` package. A successful restore or Linux cross-build is
not evidence that the native window, dispatcher, regions, or dialogs work; the
interactive Windows run above is the runtime acceptance check.
