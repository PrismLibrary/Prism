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

- Every native window owns a loaded shell and registered `ContentControl` region
  before its initialization callback. `ILoadableShell.FinishLoading` runs once,
  after that callback returns.
- Two simultaneous windows have different providers, region managers, and scoped
  disposable dependencies, but share the root singleton and registration catalogs.
  Off-thread manager/context access and reusing another window's shell are rejected
  without disturbing that original window.
- Identical `SmokeContent` and `NestedSmokeContent` region names remain independent.
  Navigation in one window leaves the other's current view unchanged.
- Application-registered and module-registered views navigate successfully.
  Navigation view constructors, their view models, and injected container providers
  all use the target window's scope, never the application root.
- Consumer XAML resolves `ViewModelLocator.AutowireViewModel` and
  `RegionManager.RegionName` metadata. The nested region discovers default views
  and view models using its owning window's provider.
- A late named discovery registration populates both existing windows. A factory
  registration after one window closes visits only the surviving scope.
- Module type registration and initialization happen exactly once across secondary
  window creation, navigation, and closure.
- The first application-level event-aggregator resolution can happen on a worker
  thread, and a background publication reaches the captured UI dispatcher.
- A dialog service injected before the second window exists retains the original
  window after `Activate()` is called on the second. Explicit targeting through
  `target.Container.Resolve<IDialogService>()` uses the selected window instead.
- Each successful dialog is inspected inside the actual default Prism
  `ContentDialog`. Both host and content `XamlRoot` match the target window's live
  content; dialog view and view-model dependencies belong to that window's scope.
- Concurrent dialogs serialize within one window. A dialog in another window can
  complete independently while the first window's dialog is still open.
- Missing/unloaded window content produces an error callback before opening a
  dialog. Replacing it with loaded content lets the original service work again,
  without creating or disposing a window scope.
- Closing a window cancels its active and queued dialogs, calls the opened dialog's
  close hook once, disposes scoped dependencies exactly once, and removes the
  window from the manager. Retained services and closed-context access fail.
- Cleanup cannot follow another window's region manager, even if application code
  accidentally attaches it to a descendant of the closing shell. The surviving
  window retains its original regions, navigates, and discovers views afterward.
  Its scoped dependencies and the shared root singleton remain undisposed.

The cross-window dialog check is intentional: the pinned native WinUI implementation
checks existing dialogs through [per-XamlRoot dialog metadata](https://github.com/microsoft/microsoft-ui-xaml/blob/ba3a8d59e6de4e2b86ca4f61c78bc26ccf129afd/src/dxaml/xcp/dxaml/lib/ContentDialog_Partial.cpp#L700-L731),
which [belongs to each individual XamlRoot](https://github.com/microsoft/microsoft-ui-xaml/blob/ba3a8d59e6de4e2b86ca4f61c78bc26ccf129afd/src/dxaml/xcp/dxaml/lib/XamlRoot_Partial.cpp#L298-L310).
Thus requests sharing one window must serialize, while different window roots can
show dialogs independently. The desktop run verifies that behavior end to end.

The harness opens secondary windows and briefly displays several dialogs without
requiring input. It closes the extra windows and leaves the primary window showing
`RESULT: PASS` with no `FAIL` lines. Use **Open Prism dialog** for an optional manual
open/close afterward. Results also appear in the debugger and in
`%LOCALAPPDATA%\PrismNativeWinUISmoke\smoke.log`, replaced on each launch.

For a Windows desktop smoke run that closes itself and returns an exit code:

```powershell
$env:PRISM_WINUI_SMOKE_AUTOCLOSE = '1'
try {
    $process = Start-Process $app -PassThru
    if (-not $process.WaitForExit(120000)) {
        Stop-Process -Id $process.Id -Force
        throw 'Native WinUI smoke exceeded the two-minute desktop-run limit.'
    }
    if ($process.ExitCode -ne 0) { throw "Native WinUI smoke failed: $($process.ExitCode)" }
    $log = Get-Content "$env:LOCALAPPDATA\PrismNativeWinUISmoke\smoke.log"
    $log
    if ($log -notcontains 'RESULT: PASS' -or ($log -match '^FAIL:')) {
        throw 'Native WinUI smoke did not report a clean pass.'
    }
} finally {
    Remove-Item Env:PRISM_WINUI_SMOKE_AUTOCLOSE
}
```

Treat a missing `RESULT: PASS`, crash, or launch failure as a failure even if no
log is produced. Window loading/activation, navigation, event delivery, and dialog
completion each have a 10-second timeout; the operating system must still supply a
responsive UI thread.

## Check dependency isolation

```powershell
dotnet list tests/WinUI/NativeWinUISmoke/NativeWinUISmoke.csproj package --include-transitive
```

There should be no `Uno.*` package. A successful restore or Linux cross-build is
not evidence that the native window, dispatcher, regions, or dialogs work; the
interactive Windows run above is the runtime acceptance check.
