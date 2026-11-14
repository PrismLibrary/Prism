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

- Loaded-shell and region registration ordering before `OnInitialized()`.
- Exactly one `ILoadableShell.FinishLoading()` call after initialization.
- Application-container resolution of views, view models, and dialog services.
- Navigation to a XAML view, a module-provided view, and back again.
- XAML view-model autowiring and nested-region view discovery.
- UI-thread event delivery after background publication.
- Repeated native dialogs, their XamlRoot, lifecycle and callback completion.
- Exactly one module registration and initialization.
- Navigation parameters, matching-view reuse, distinct targets, veto and deferred confirmation.
- Journal Back, Forward, and Clear, including parameter restoration.
- Active-state transitions and KeepAlive removal with view/view-model destruction.
- Missing navigation and dialog registrations returning callback errors.
- Native dialog close veto, Cancel results, and release of host content after closing.
- Native ItemsControl and Selector synchronization and NavigationView selection/back behavior.

The UI displays each assertion and a final PASS or FAIL. To exit after the checks,
set `PRISM_WINUI_SMOKE_AUTOCLOSE=1`; otherwise the dialog button remains available
for manual inspection. The process exit code is zero only when all checks pass.

These are executable checks, not a claim that the current branch has passed them.
They must be run on a Windows desktop after building the final changes.
